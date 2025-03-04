using UnityEngine;

public class PhysicsBody : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector3 gravity;
    public float mass = 1f;
    public float restitution = 1f; //research calcing restitution manually rather than use set value
}
