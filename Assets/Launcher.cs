using UnityEngine;

public class Launcher : MonoBehaviour
{
    public float launchAngle;
    public float launchPower;
    public Vector3 launchVelocity;

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
        Vector3 endPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));

        //find angle
        Vector3 directionToEndpoint = (endPosition - transform.position).normalized;
        launchAngle = Mathf.Atan2(directionToEndpoint.x, directionToEndpoint.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, -launchAngle); //negative launch angle due to sprite default position

        //calculate magnitude from cannon origin for force value
        //explicity set z to 0f as it was messing up the magnitude calc
        launchPower = (new Vector3(endPosition.x, endPosition.y, 0f) - new Vector3(transform.position.x, transform.position.y, 0f)).magnitude;

        //find initial velocity
        launchVelocity = new Vector3(directionToEndpoint.x * Mathf.Cos(Mathf.Deg2Rad * launchAngle), directionToEndpoint.y * Mathf.Sin(Mathf.Deg2Rad * launchAngle));
        launchVelocity *= launchPower;

        //interpolate through kinematic equation and display using line renderer
        // Loop to calculate the trajectory path and display it
        lineRenderer.positionCount = 0;

        for (float t = 0; t < 100; t++)
        {
            // Calculate position based on kinematic equations (x = x0 + v0x * t, y = y0 + v0y * t + 0.5 * a * t^2)
            float x = transform.position.x + launchVelocity.x * t;
            float y = transform.position.y + launchVelocity.y * t + 0.5f * -9.81f * Mathf.Pow(t, 2);

            // Add the new calculated position to the line renderer
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, new Vector3(x, y, 0f));

            if (Vector3.Distance(new Vector3(x, y, 0f), endPosition) <= 0.1f)
            {
                break;
            }
        }
    }
}
