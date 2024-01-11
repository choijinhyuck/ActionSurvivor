using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VirtualCamera : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;

    private void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
    }

    private void LateUpdate()
    {
        if (vCam.Follow == Player.instance.transform) return;
        if (Player.instance != null)
        {
            vCam.Follow = Player.instance.transform;
        }
    }
}
