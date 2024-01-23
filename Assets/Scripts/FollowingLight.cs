using UnityEngine;

public class FollowingLight : MonoBehaviour
{
    Vector3 calibration;

    private void Awake()
    {
        calibration = new(0, 0.5f, 0f);
    }

    private void FixedUpdate()
    {
        if (Player.instance == null) return;
        transform.position = Player.instance.transform.position + calibration;

    }
}
