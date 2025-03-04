using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PhysicsEngine : MonoBehaviour
{
    private enum CollisionType
    {
        NONE,
        POINT_TO_RECT,
        POINT_TO_CIRCLE,
        CIRCLE_TO_CIRCLE,
        CIRCLE_TO_RECT,
        RECT_TO_RECT,
        RAY_INTERSECT,
        RAY_TO_RECT,
        RAY_TO_CIRCLE
    }

    private void Update()
    {
        UpdateVelocities();
        UpdatePositions();
        CheckCollisions();
    }

    /* 
        Updates the velocity for each object in the scene that is a physics body.
        Velocity is updated by acceleration and gravity.
    */
    private void UpdateVelocities() 
    {
        PhysicsBody[] bodies = FindObjectsByType<PhysicsBody>(FindObjectsSortMode.None);
        foreach (PhysicsBody body in bodies) 
        {
            body.velocity += body.acceleration * Time.deltaTime;
            body.velocity += body.gravity * Time.deltaTime;
        }
    }

    /*
        Updates the positions of all objects in the scene that are a physics body.
        The new velocity value calculated in 'UpdateVelocities' is added to the current
        object's world position.
    */
    private void UpdatePositions() 
    {
        PhysicsBody[] bodies = FindObjectsByType<PhysicsBody>(FindObjectsSortMode.None);
        foreach (PhysicsBody b in bodies)
        {
            b.transform.position += b.velocity * Time.deltaTime;
        }
    }

    /*
        Checks for collisions between all objects in the scene that are marked for collisions.
        
        Currently, each object checks its world position against all other collidable objects in
        the scene. If an object's position is within the bounds of another collidable object, a
        collision is logged.
        The object's position is first checked against the x-axis of the other objects bounds, if this
        is true, the position is then checked against the other object's y-axis bounds. If this is true,
        then a collision has occured.

        Current types of collisions: point/axis-aligned rectangle
    */

    //research impluse and velocity physics instead of penetration depth
    private void CheckCollisions() 
    {
        Collider[] colliders = FindObjectsByType<Collider>(FindObjectsSortMode.None);

        for (int i = 0; i < colliders.Length - 1; i++) 
        {
            Collider a = colliders[i];

            for (int j = i + 1; j < colliders.Length; j++) 
            {
                Collider b = colliders[j];

                switch (a.type, b.type) 
                {
                    //POINT TO RECTANGLE (AxisAligned)
                    case (Collider.Type.POINT, Collider.Type.AXIS_ALIGNED_RECTANGLE):
                    case (Collider.Type.AXIS_ALIGNED_RECTANGLE, Collider.Type.POINT):
                        {
                            Collider point = (a.type == Collider.Type.POINT) ? a : b;
                            Collider rectangle = (a.type == Collider.Type.AXIS_ALIGNED_RECTANGLE) ? a : b;

                            float width = rectangle.transform.localScale.x;
                            float height = rectangle.transform.localScale.y;

                            float lhBound = rectangle.transform.localPosition.x - (width / 2f);
                            float rhBound = rectangle.transform.localPosition.x + (width / 2f);
                            float topBound = rectangle.transform.localPosition.y + (height / 2f);
                            float botBound = rectangle.transform.localPosition.y - (height / 2f);

                            bool onLHS = (point.transform.localPosition.x < lhBound);
                            bool onRHS = (point.transform.localPosition.x > rhBound);
                            bool onTop = (point.transform.localPosition.y > topBound);
                            bool onBot = (point.transform.localPosition.y < botBound);

                            if (onLHS || onRHS || onTop || onBot) continue;

                            HandleCollision();
                            print("Point To Rect: Collision");
                            break;
                        }

                    //POINT TO CIRCLE
                    case (Collider.Type.POINT, Collider.Type.CIRCLE):
                    case (Collider.Type.CIRCLE, Collider.Type.POINT):
                        {
                            Collider point = (a.type == Collider.Type.POINT) ? a : b;
                            Collider circle = (a.type == Collider.Type.CIRCLE) ? a : b;

                            float radius = circle.transform.localScale.x / 2f;
                            float magnitude = (point.transform.position - circle.transform.position).magnitude;
                            float distance = magnitude - radius;

                            if (distance <= radius)
                            {
                                HandleCollision();
                                print("Point To Circle: Collision");
                            }
                            break;
                        }

                    //CIRCLE TO CIRCLE
                    case (Collider.Type.CIRCLE, Collider.Type.CIRCLE):
                        {
                            //radius
                            float combinedRadius = (a.transform.localScale.x / 2f) + (b.transform.localScale.x / 2f);

                            //a to b vector
                            Vector3 AtoB = b.transform.position - a.transform.position;
                            Vector3 AtoBNorm = AtoB.normalized;
                            float distanceAtoB = AtoB.magnitude;

                            //b to vector
                            Vector3 BtoA = a.transform.position - b.transform.position;
                            Vector3 BtoANorm = BtoA.normalized;
                            float distanceBtoA = BtoA.magnitude;

                            //mass
                            float massA = a.GetComponent<PhysicsBody>().mass;
                            float massB = b.GetComponent<PhysicsBody>().mass;
                            float massCombined = massA + massB;

                            if (distanceAtoB <= combinedRadius)
                            {
                                //HandleCollision(); abstract later
                                a.transform.position += BtoANorm * (combinedRadius - distanceBtoA) * (massB / massCombined);
                                b.transform.position += AtoBNorm * (combinedRadius - distanceAtoB) * (massA / massCombined);
                                print("Circle To Circle: Collision");
                            }
                            break;
                        }

                    //RECTANGLE (AxisAligned) TO CIRCLE
                    case (Collider.Type.AXIS_ALIGNED_RECTANGLE, Collider.Type.CIRCLE):
                    case (Collider.Type.CIRCLE, Collider.Type.AXIS_ALIGNED_RECTANGLE):
                        {
                            Collider rectangle = (a.type == Collider.Type.AXIS_ALIGNED_RECTANGLE) ? a : b;
                            Collider circle = (a.type == Collider.Type.CIRCLE) ? a : b;

                            float circleRadius = circle.transform.localScale.x / 2f;
                            float rectHalfWidth = rectangle.transform.localScale.x / 2f;
                            float rectHalfHeight = rectangle.transform.localScale.y / 2f;
                            float clampX = Mathf.Clamp(circle.transform.position.x, 
                                                       rectangle.transform.position.x - rectHalfWidth, 
                                                       rectangle.transform.position.x + rectHalfWidth);
                            float clampY = Mathf.Clamp(circle.transform.position.y,
                                                       rectangle.transform.position.y - rectHalfHeight,
                                                       rectangle.transform.position.y + rectHalfHeight);

                            Vector3 circleToClosestPoint = new Vector3(clampX, clampY) - circle.transform.position;
                            float distance = circleToClosestPoint.magnitude;

                            //research fast inverse square root for mag calc (ssquare magnitude)
                            if(distance < circleRadius) 
                            {
                                HandleCollision();
                                //calc restitution so that a rectangle can either be immovable (circles bounce on it without moving it) or
                                //both circle and rectangle move during a collision
                                print("Rect To Circle: Collision");
                            }
                            break;
                        }

                    //RECTANGLE (AxisAligned) TO RECTANGLE (AxisAligned)
                    case (Collider.Type.AXIS_ALIGNED_RECTANGLE, Collider.Type.AXIS_ALIGNED_RECTANGLE):
                        {
                            HandleCollision();
                            print("Rect To Rect: Collision");
                            break;
                        }

                    //RAY INTERSECTION
                    case (Collider.Type.RAY, Collider.Type.RAY):
                        {
                            HandleCollision();
                            print("Ray Intersection");
                            break;
                        }

                    //RAY TO RECTANGLE (AxisAligned)
                    case (Collider.Type.RAY, Collider.Type.AXIS_ALIGNED_RECTANGLE):
                        {
                            Collider ray = (a.type == Collider.Type.RAY) ? a : b;
                            Collider rectangle = (a.type == Collider.Type.AXIS_ALIGNED_RECTANGLE) ? a : b;

                            HandleCollision();
                            print("Ray To Rectangle: Collision");
                            break;
                        }

                    //RAY TO CIRCLE
                    case (Collider.Type.RAY, Collider.Type.CIRCLE):
                        {
                            Collider ray = (a.type == Collider.Type.RAY) ? a : b;
                            Collider circle = (a.type == Collider.Type.CIRCLE) ? a : b;

                            HandleCollision();
                            print("Ray To Circle: Collision");
                            break;
                        }

                    //NO COLLISION DETECTED
                    default:
                        break;
                }
            }
        }
    }

    private void HandleCollision() 
    {
        
    }
}
