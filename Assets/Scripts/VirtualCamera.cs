using Cinemachine;
using System.Collections;
using UnityEngine;

public class VirtualCamera : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;

    private void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        StartCoroutine(InitFollow());
    }

    IEnumerator InitFollow()
    {
        while (true)
        {
            if (Player.instance != null)
            {
                vCam.Follow = Player.instance.transform;
                yield break;
            }
            else
            {
                yield return null;
            }
        }
    }

    public void FollowTarget(Transform target)
    {
        if (target == null)
        {
            Debug.Log("찾을 수 없는 객체입니다. Null reference");
            return;
        }
        vCam.Follow = target;
    }
}
