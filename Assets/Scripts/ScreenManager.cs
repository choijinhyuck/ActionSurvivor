using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScreenManager : MonoBehaviour
{
    private void Awake()
    {
        if (SettingUI.instance != null)
        {
            SettingUI.instance.LoadResolution();
        }
        Destroy(gameObject);
    }
}
