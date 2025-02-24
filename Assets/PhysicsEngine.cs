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
                    //POINT TO RECTANGLE (AA)
                    case (Collider.Type.POINT, Collider.Type.AXIS_ALIGNED_RECTANGLE):
                    case (Collider.Type.AXIS_ALIGNED_RECTANGLE, Collider.Type.POINT):

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
                        print("PTR: Collision");
                        break;

                    //POINT TO CIRCLE
                    case (Collider.Type.POINT, Collider.Type.CIRCLE):
                    case (Collider.Type.CIRCLE, Collider.Type.POINT):
                        break;

                    //CIRCLE TO CIRCLE
                    case (Collider.Type.CIRCLE, Collider.Type.CIRCLE):
                        break;

                    //RECTANGLE (AA) TO CIRCLE
                    case (Collider.Type.AXIS_ALIGNED_RECTANGLE, Collider.Type.CIRCLE):
                    case (Collider.Type.CIRCLE, Collider.Type.AXIS_ALIGNED_RECTANGLE):
                        break;

                    //RECTANGLE (AA) TO RECTANGLE (AA)
                    case (Collider.Type.AXIS_ALIGNED_RECTANGLE, Collider.Type.AXIS_ALIGNED_RECTANGLE):
                        break;

                    //RAY INTERSECTION
                    case (Collider.Type.RAY, Collider.Type.RAY):
                        break;

                    //RAY TO RECTANGLE (AA)
                    case (Collider.Type.RAY, Collider.Type.AXIS_ALIGNED_RECTANGLE):
                        break;

                    //RAY TO CIRCLE
                    case (Collider.Type.RAY, Collider.Type.CIRCLE):
                        break;

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
