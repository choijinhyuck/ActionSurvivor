using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    public static SettingUI instance;

    public Dropdown resolutionDropdown;
    public Toggle[] screenTypes;
    public GameObject settingPanel;
    public Text bgmVolume;
    public Text sfxVolume;

    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

    bool isOnConfirm;
    GameObject lastSelectedObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        if (settingPanel.activeSelf) settingPanel.SetActive(false);
        lastSelectedObject = null;
        isOnConfirm = false;
    }

    private void LateUpdate()
    {
        if (settingPanel.activeSelf)
        {
            if (!isOnConfirm)
            {
                isOnConfirm = true;
                lastSelectedObject = EventSystem.current.currentSelectedGameObject;
                EventSystem.current.SetSelectedGameObject(resolutionDropdown.gameObject);
            }
            foreach (var screenType in screenTypes)
            {
                if (screenType.isOn)
                {
                    screenType.GetComponentInChildren<Text>().color = Color.white;
                }
                else
                {
                    screenType.GetComponentInChildren<Text>().color = new(1, 1, 1, 0.1f);
                }
            }
        }
        else
        {
            if (isOnConfirm)
            {
                isOnConfirm = false;
            }
        }
    }

    public void Back(bool buttonPress = false)
    {
        if (buttonPress)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
        }
        else
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
        }
        if (lastSelectedObject.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedObject);
        }
        settingPanel.SetActive(false);
    }
}
