using UnityEngine;

public class Launcher : MonoBehaviour
{
    public float angle;
    public float launchPower;
    private float curveHeight = 5f;

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
        (Vector2 p1, Vector2 p2, Vector2 p3) = SetKnownPoints();

        //init the line renderer
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, p1);
        lineRenderer.SetPosition(1, p2);

        //check if the current mouse position is negative or positive
        if (p2.x > p1.x)
        {
            for (float x = p1.x; x < p2.x; x += 0.1f)
            {
                NewLinePosition(lineRenderer, x, p1, p2, p3);
            }
        }
        else if (p2.x < p1.x)
        {
            for (float x = p1.x; x > p2.x; x -= 0.1f)
            {
                NewLinePosition(lineRenderer, x, p1, p2, p3);
            }
        }
    }

    //sets the three points required for the lagrande formula (origin, midpoint and mouse position)
    private (Vector2, Vector2, Vector2) SetKnownPoints()
    {
        Vector2 p1 = transform.position;
        Vector2 p2 = MousePositionToWorldPoint();
        Vector2 p3 = ((p1 + p2) / 2f) + new Vector2(0f, curveHeight);

        return (p1, p2, p3);
    }

    //grabs the mouse position in world units
    private Vector2 MousePositionToWorldPoint()
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
    }

    //add a new point to the line renderer, and reposition the final position to point p2 (mouse position)
    private void NewLinePosition(LineRenderer lineRenderer, float x, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        lineRenderer.positionCount++;
        Vector2 newPoint = new Vector2(x, LagrangeFormula(x, p1, p2, p3));
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, p2);
        lineRenderer.SetPosition(lineRenderer.positionCount - 2, newPoint);
    }

    //calculates the y value of a given x value when there are 3 known points
    private float LagrangeFormula(float x, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float langrange1 = ((x - p2.x) * (x - p3.x)) / ((p1.x - p2.x) * (p1.x - p3.x));
        float langrange2 = ((x - p1.x) * (x - p3.x)) / ((p2.x - p1.x) * (p2.x - p3.x));
        float langrange3 = ((x - p1.x) * (x - p2.x)) / ((p3.x - p1.x) * (p3.x - p2.x));

        return (langrange1 * p1.y) + (langrange2 * p2.y) + (langrange3 * p3.y);
    }
}
