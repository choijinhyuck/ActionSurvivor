using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class MainCamera : MonoBehaviour
{
    PixelPerfectCamera pp;
    int currPPU;

    private void Awake()
    {
        pp = GetComponent<PixelPerfectCamera>();
        Init();
        StartCoroutine(RepeatInitCoroutine());
    }

    IEnumerator RepeatInitCoroutine()
    {
        yield return null;
        Init();
    }

    private void LateUpdate()
    {
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            if (pp.refResolutionX != Screen.width)
            {
                Init();
            }
        }
        else
        {
            if (pp.refResolutionX != Screen.currentResolution.width)
            {
                Init();
            }
        }
    }

    void Init()
    {
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            pp.refResolutionX = Screen.width;
            pp.refResolutionY = Screen.height;
            currPPU = Mathf.FloorToInt(Screen.width / 1920f * 150);
        }
        else
        {
            pp.refResolutionX = Screen.currentResolution.width;
            pp.refResolutionY = Screen.currentResolution.height;
            currPPU = Mathf.FloorToInt(Screen.currentResolution.width / 1920f * 150);
        }
        currPPU = (currPPU % 2) == 0 ? currPPU : currPPU + 1;
        pp.assetsPPU = currPPU;

        if (GameManager.instance != null && GameManager.instance.originPPU != currPPU)
        {
            GameManager.instance.originPPU = currPPU;
        }
    }
}
