using UnityEngine;

public class Collider : MonoBehaviour
{
    public enum Type 
    {
        POINT,
        AXIS_ALIGNED_RECTANGLE,
        CIRCLE,
        RAY
    }

    public Type type;
}
