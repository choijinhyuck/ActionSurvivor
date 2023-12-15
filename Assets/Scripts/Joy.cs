using UnityEngine;

public class Joy : MonoBehaviour
{
    Vector3 joyPoint;
    Vector3 stickPoint;

    public Vector3 nextDir;
    public Transform stick;

    private void Awake()
    {
        stick = transform.GetChild(0);
    }


    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (transform.localScale.x == 0)
            {
                joyPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                joyPoint = Camera.main.WorldToScreenPoint(joyPoint);
                transform.position = joyPoint;
                transform.localScale = Vector3.one;
            }
            stickPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            stickPoint = Camera.main.WorldToScreenPoint(stickPoint);

            Vector3 dir = stickPoint - joyPoint;

            if (dir.magnitude > 30)
            {
                stick.position = joyPoint + dir.normalized * 30;
                nextDir = dir.normalized * 30;
            }
            else
            {
                stick.position = stickPoint;
                nextDir = dir;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            transform.localScale = Vector3.zero;
            nextDir = Vector3.zero;
        }
    }
}
