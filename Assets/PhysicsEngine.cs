using UnityEngine;

public class PhysicsEngine : MonoBehaviour
{
    private void Update()
    {
        UpdateVelocities();
        UpdatePositions();
        CheckCollisions();
    }

    void UpdateVelocities() 
    {
        PhysicsBody[] bodies = FindObjectsByType<PhysicsBody>(FindObjectsSortMode.None);
        foreach (PhysicsBody b in bodies) 
        {
            b.velocity += b.acceleration * Time.deltaTime;
            b.velocity += b.gravity * Time.deltaTime;
        }
    }

    void UpdatePositions() 
    {
        PhysicsBody[] bodies = FindObjectsByType<PhysicsBody>(FindObjectsSortMode.None);
        foreach (PhysicsBody b in bodies)
        {
            b.transform.position += b.velocity * Time.deltaTime;
        }
    }

    void CheckCollisions() 
    {
        Collider[] colliders = FindObjectsByType<Collider>(FindObjectsSortMode.None);

        for (int i = 0; i < colliders.Length - 1; i++) 
        {
            Collider a = colliders[i];

            for (int j = i + 1; j < colliders.Length; j++) 
            {
                Collider b = colliders[j];

                bool pointToRect = (a.type == Collider.Type.POINT && b.type == Collider.Type.AXIS_ALIGNED_RECTANGLE ||
                    b.type == Collider.Type.POINT && a.type == Collider.Type.AXIS_ALIGNED_RECTANGLE);

                if (pointToRect)
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

                    print("Collision");
                }
            }
        }
    }
}
