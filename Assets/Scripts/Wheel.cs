using UnityEngine;

public class Wheel : MonoBehaviour
{
    public Transform pointObject;
    public Transform wheelObject;

    float axeleration;
    float angle;
    float angleDeg;

    RectTransform rt;

    bool dragged = false;
    float tm = 0;
    bool reverse = false;

    void Awake()
    {
        rt = pointObject.GetComponent<RectTransform>();
    }

    //		0
    //	90		-90
    //		0
    //
    public void drag()
    {
        dragged = true;

        Vector3 output;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, Input.mousePosition, GetComponent<Camera>(), out output);

        axeleration = Vector3.Magnitude(wheelObject.position - Input.mousePosition);
        if (axeleration > 75)
            axeleration = 75;

        angle = Mathf.Atan2(Input.mousePosition.y - wheelObject.position.y, Input.mousePosition.x - wheelObject.position.x) - 1.57f;// /*Mathf.PI / 2*/ ) * 57.29578f; //Mathf.Rad2Deg;

        angleDeg = angle * Mathf.Rad2Deg;

        if (angleDeg < -90)
        {
            reverse = true;
            angleDeg = -90 - (angleDeg + 90);
            axeleration = -axeleration;
        }
        else
            reverse = false;
        tm = 0.01f;
    }

    static float minDelta = 15;
    void moveWheel()
    {
        wheelObject.rotation = Quaternion.RotateTowards(wheelObject.rotation, Quaternion.AngleAxis(angleDeg, Vector3.forward), 6f);
        if (reverse)
            rt.anchoredPosition = new Vector2(Mathf.Sin(angle) * axeleration, -Mathf.Cos(angle) * axeleration);
        else
            rt.anchoredPosition = new Vector2(-Mathf.Sin(angle) * axeleration, Mathf.Cos(angle) * axeleration);

        Debug.Log(rt.anchoredPosition);

        if (rt.anchoredPosition.y < -minDelta)
            Player.instance.goDown();
        else if (rt.anchoredPosition.y > minDelta)
            Player.instance.goUp();

        if (rt.anchoredPosition.x < -minDelta)
            Player.instance.goLeft();
        else if (rt.anchoredPosition.x > minDelta)
            Player.instance.goRight();

    }

    private float GEARSPEEDMIN = 0.2f;
    private bool m_bBrakeRelease = false;
    private bool m_bAccelRelease = false;

    void Update()
    {
        if (dragged && !Input.GetMouseButton(0))
        {
            if (tm < 0)
            {
                if (Vector2.SqrMagnitude(rt.anchoredPosition) < 10)
                {
                    rt.anchoredPosition = Vector2.zero;
                    dragged = false;
                    axeleration = 0;
                    angleDeg = 0f;
                }
                else
                {
                    rt.anchoredPosition = Vector2.MoveTowards(rt.anchoredPosition, Vector2.zero, Time.deltaTime * 200f);
                    axeleration = 0;
                    wheelObject.rotation = Quaternion.RotateTowards(wheelObject.rotation, Quaternion.AngleAxis(0, Vector3.forward), 10f);
                    angleDeg = 0f;
                }
            }
            else
                tm -= Time.deltaTime;
        }
        if (Input.GetAxis("Horizontal") != 0)
        {
            angleDeg = -Input.GetAxis("Horizontal") * 90f;
            angle = angleDeg * Mathf.Deg2Rad;
            tm = 0.1f;
            dragged = true;
            reverse = false;
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            axeleration = Input.GetAxis("Vertical") * 75f;
            tm = 0.1f;
            dragged = true;
            reverse = false;
        }
        if (dragged && tm > 0)
            moveWheel();

    }
}
