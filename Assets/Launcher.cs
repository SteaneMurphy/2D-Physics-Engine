using UnityEngine;

public class Launcher : MonoBehaviour
{
    public float launchAngle;
    public float launchPower;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        DisplayProjectilePath();
    }

    private void DisplayProjectilePath() 
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        Vector3 endPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        lineRenderer.SetPosition(1, endPosition);

        //find angle
        Vector3 directionToEndpoint = (endPosition - transform.position).normalized;
        launchAngle = -Mathf.Atan2(directionToEndpoint.x, directionToEndpoint.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, launchAngle);

        //find initial velocity
    }
}
