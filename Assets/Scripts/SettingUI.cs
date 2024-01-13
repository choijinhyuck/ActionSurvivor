using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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
    GameObject currentSelectedObject;

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
        currentSelectedObject = null;
        isOnConfirm = false;

        //Screen.SetResolution(800, 600, FullScreenMode.FullScreenWindow);
        

        if (PlayerPrefs.HasKey("bgmVolume"))
        {
            bgmSlider.value = PlayerPrefs.GetFloat("bgmVolume");
        }
        else
        {
            bgmSlider.value = 0.5f;
            PlayerPrefs.SetFloat("bgmVolume", 0.5f);
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        }
        else
        {
            sfxSlider.value = 0.5f;
            PlayerPrefs.SetFloat("sfxVolume", 0.5f);
            PlayerPrefs.Save();
        }

        // Value Change에 따른 Callback 함수 Null Reference 오류로 인해, 늦은 Callback Function 추가
        bgmSlider.onValueChanged.AddListener(delegate { OnBgmVolumeChanged(); });
        sfxSlider.onValueChanged.AddListener(delegate { OnSfxVolumeChanged(); });
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
                currentSelectedObject = EventSystem.current.currentSelectedGameObject;
            }
            else
            {
                if (currentSelectedObject != EventSystem.current.currentSelectedGameObject)
                {
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonChange);
                    currentSelectedObject = EventSystem.current.currentSelectedGameObject;
                }
            }

            bgmVolume.text = Mathf.RoundToInt(bgmSlider.value * 100).ToString();
            sfxVolume.text = Mathf.RoundToInt(sfxSlider.value * 100).ToString();

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
        if (lastSelectedObject != null && lastSelectedObject.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedObject);
        }
        settingPanel.SetActive(false);
        if (SceneManager.GetActiveScene().name != "Title")
        {
            GameManager.instance.workingInventory = false;
            GameManager.instance.Resume();
        }
    }

    public bool DropdownOpened()
    {
        if (resolutionDropdown.transform.childCount == 3)
        {
            return false;
        }
        else if (resolutionDropdown.transform.childCount == 4)
        {
            return true;
        }
        else
        {
            Debug.Log("Dropdown's childCount Error");
            return true;
        }
    }

    public void OnBgmVolumeChanged()
    {
        PlayerPrefs.SetFloat("bgmVolume", bgmSlider.value);
        PlayerPrefs.Save();
        AudioManager.instance.SetBgmVolume();
    }
    
    public void OnSfxVolumeChanged()
    {
        PlayerPrefs.SetFloat("sfxVolume", sfxSlider.value);
        PlayerPrefs.Save();
        AudioManager.instance.SetSfxVolume();
    }
}
