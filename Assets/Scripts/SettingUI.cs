using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    public enum LanguageType { Korean, English }

    public static SettingUI instance;

    public Dropdown resolutionDropdown;
    public RectTransform dropdownTemplate;
    public Toggle[] screenTypes;
    public GameObject settingPanel;
    public Text bgmVolume;
    public Text sfxVolume;
    public LanguageType currLanguage;

    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Toggle[] languageTypes;

    bool isOnConfirm;
    int resolutionId;
    FullScreenMode screenMode;
    GameObject lastSelectedObject;
    GameObject currentSelectedObject;
    List<int> widths;
    List<int> heights;

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

        // Language 설정
        if (PlayerPrefs.HasKey("language"))
        {
            switch (PlayerPrefs.GetString("language"))
            {
                case "korean":
                    currLanguage = LanguageType.Korean;
                    break;
                case "english":
                    currLanguage = LanguageType.English;
                    break;
            }
        }
        else if (Application.systemLanguage == SystemLanguage.Korean)
        {
            currLanguage = LanguageType.Korean;
            languageTypes[0].isOn = true;
            languageTypes[1].isOn = false;
            PlayerPrefs.SetString("language", "korean");
            PlayerPrefs.Save();
        }
        else
        {
            currLanguage = LanguageType.English;
            languageTypes[0].isOn = false;
            languageTypes[1].isOn = true;
            PlayerPrefs.SetString("language", "english");
            PlayerPrefs.Save();
        }
        if (currLanguage == LanguageType.Korean)
        {
            languageTypes[0].isOn = true;
            languageTypes[1].isOn = false;
        }
        else
        {
            languageTypes[0].isOn = false;
            languageTypes[1].isOn = true;
        }


        if (settingPanel.activeSelf) settingPanel.SetActive(false);
        lastSelectedObject = null;
        currentSelectedObject = null;
        isOnConfirm = false;


        InitResolution();
        LoadResolution();

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

        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChanged(); });
        foreach (var screenType in screenTypes)
        {
            screenType.onValueChanged.AddListener(delegate { OnScreenTypeChanged(); });
        }

        foreach (var languageType in languageTypes)
        {
            languageType.onValueChanged.AddListener(delegate { OnLanguageTypeChanged(); });
        }

        InitLanguage();
    }

    void InitLanguage()
    {
        Dictionary<string, string[]> nameDic = new();
        nameDic["Title"] = new string[] { "설정", "Settings" };
        nameDic["Resolution Title"] = new string[] { "해상도", "Resolution" };
        nameDic["FullScreen Label"] = new string[] { "전체화면", "FullScreen" };
        nameDic["Borderless Label"] = new string[] { "테두리 없음", "Borderless" };
        nameDic["Windowed Label"] = new string[] { "창모드", "Windowed" };
        nameDic["Volume Desc"] = new string[] { "음량", "Volume" };
        nameDic["BGM Title"] = new string[] { "배경음", "Music" };
        nameDic["SFX Title"] = new string[] { "효과음", "Effect" };
        nameDic["Back Label"] = new string[] { "뒤로 가기", "Back" };

        var texts = GetComponentsInChildren<Text>(true);
        int textId = currLanguage == LanguageType.Korean ? 0 : 1;
        foreach (var text in texts)
        {
            if (nameDic.ContainsKey(text.name))
            {
                text.text = nameDic[text.name][textId];
            }
        }
    }

    void InitResolution()
    {
        screenMode = Screen.fullScreenMode;
        foreach (var screenType in screenTypes)
        {
            screenType.isOn = false;
        }
        switch (screenMode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                screenTypes[0].isOn = true;
                break;
            case FullScreenMode.FullScreenWindow:
                screenTypes[1].isOn = true;
                break;
            case FullScreenMode.Windowed:
                screenTypes[2].isOn = true;
                break;
            default:
                Debug.Log("사용하지 않는 스크린 모드입니다.");
                screenTypes[2].isOn = true;
                break;
        }

        resolutionDropdown.options = new List<Dropdown.OptionData>();
        var resolutions = Screen.resolutions;
        widths = new();
        heights = new();

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width * 9 == resolutions[i].height * 16 ||
                ResolutionList.IsAppropriateResolution(resolutions[i].width, resolutions[i].height))
            {
                widths.Add(resolutions[i].width);
                heights.Add(resolutions[i].height);
            }
        }
        widths = widths.Distinct().ToList();
        heights = heights.Distinct().ToList();
        widths = widths.OrderByDescending(x => x).ToList();
        heights = heights.OrderByDescending(x => x).ToList();

        for (int i = 0; i < widths.Count; i++)
        {
            Dropdown.OptionData optionData = new()
            {
                text = $"{widths[i]}x{heights[i]}"
            };
            resolutionDropdown.options.Add(optionData);
        }

        int currWidth;
        if (screenMode == FullScreenMode.Windowed)
        {
            Screen.SetResolution(Screen.width, ResolutionList.GetAppropriateHeight(Screen.width), FullScreenMode.Windowed);
            currWidth = Screen.width;
        }   
        else if (screenMode == FullScreenMode.FullScreenWindow)
        {
            Screen.SetResolution(Screen.currentResolution.width, ResolutionList.GetAppropriateHeight(Screen.currentResolution.width), FullScreenMode.FullScreenWindow);
            currWidth = Screen.currentResolution.width;
        }
        else
        {
            Screen.SetResolution(Screen.currentResolution.width, ResolutionList.GetAppropriateHeight(Screen.currentResolution.width), FullScreenMode.ExclusiveFullScreen);
            currWidth = Screen.currentResolution.width;
        }

        if (GetResolutionId(currWidth) == -1)
        {
            resolutionId = 0;
            resolutionDropdown.value = 0;
            FitScreen();
        }
        else
        {
            resolutionId = GetResolutionId(currWidth);
            resolutionDropdown.value = GetResolutionId(currWidth);
        }

        if (resolutionDropdown.options.Count > 10)
        {
            dropdownTemplate.sizeDelta = new Vector2(dropdownTemplate.sizeDelta.x, 30);
        }
        else
        {
            dropdownTemplate.sizeDelta = new Vector2(dropdownTemplate.sizeDelta.x, 50);
        }

        FitScreen();
    }

    public void LoadResolution()
    {
        if (PlayerPrefs.HasKey("screenWidth"))
        {
            resolutionId = GetResolutionId(PlayerPrefs.GetInt("screenWidth"));
            resolutionDropdown.value = resolutionId;
            screenMode = (FullScreenMode)PlayerPrefs.GetInt("screenMode");
            foreach (var screenType in screenTypes)
            {
                screenType.isOn = false;
            }
            screenTypes[GetScreenTypeId(screenMode)].isOn = true;

            Screen.SetResolution(PlayerPrefs.GetInt("screenWidth"), PlayerPrefs.GetInt("screenHeight"), screenMode);

            FitScreen();
        }
        else
        {
            if (screenMode == FullScreenMode.Windowed)
            {
                PlayerPrefs.SetInt("screenWidth", Screen.width);
                PlayerPrefs.SetInt("screenHeight", Screen.height);
            }
            else
            {
                PlayerPrefs.SetInt("screenWidth", Screen.currentResolution.width);
                PlayerPrefs.SetInt("screenHeight", Screen.currentResolution.height);
            }
            PlayerPrefs.SetInt("screenMode", (int)screenMode);
            PlayerPrefs.Save();
        }
    }

    void FitScreen()
    {
        if (CheckScreen()) return;
        screenMode = FullScreenMode.ExclusiveFullScreen;
        resolutionId = 0;
        resolutionDropdown.value = 0;
        Screen.SetResolution(widths[resolutionId], heights[resolutionId], screenMode);
        PlayerPrefs.SetInt("screenWidth", widths[resolutionId]);
        PlayerPrefs.SetInt("screenHeight", heights[resolutionId]);
        PlayerPrefs.SetInt("screenMode", (int)screenMode);
    }

    bool CheckScreen()
    {
        if (screenMode == FullScreenMode.ExclusiveFullScreen) return true;

        if (screenMode == FullScreenMode.Windowed)
        {
            while (true)
            {
                if (widths[resolutionId] > Screen.currentResolution.width)
                {
                    if (resolutionId == widths.Count - 1) return false;
                    resolutionId++;
                    resolutionDropdown.value = resolutionId;
                    Screen.SetResolution(widths[resolutionId], heights[resolutionId], screenMode);
                    continue;
                }
                else if (heights[resolutionId] > Screen.currentResolution.height)
                {
                    if (resolutionId == widths.Count - 1) return false;
                    resolutionId++;
                    resolutionDropdown.value = resolutionId;
                    Screen.SetResolution(widths[resolutionId], heights[resolutionId], screenMode);
                    continue;
                }
                break;
            }
        }
        else if (screenMode == FullScreenMode.FullScreenWindow)
        {
            while (true)
            {
                if (widths[resolutionId] > Display.main.systemWidth)
                {
                    if (resolutionId == widths.Count - 1) return false;
                    resolutionId++;
                    resolutionDropdown.value = resolutionId;
                    Screen.SetResolution(widths[resolutionId], heights[resolutionId], screenMode);
                    continue;
                }
                else if (heights[resolutionId] > Display.main.systemWidth)
                {
                    if (resolutionId == widths.Count - 1) return false;
                    resolutionId++;
                    resolutionDropdown.value = resolutionId;
                    Screen.SetResolution(widths[resolutionId], heights[resolutionId], screenMode);
                    continue;
                }
                break;
            }
        }

        PlayerPrefs.SetInt("screenWidth", widths[resolutionId]);
        PlayerPrefs.SetInt("screenHeight", heights[resolutionId]);
        PlayerPrefs.SetInt("screenMode", (int)screenMode);
        PlayerPrefs.Save();
        return true;
    }

    int GetResolutionId(int width)
    {
        for (int i = 0; i < resolutionDropdown.options.Count; i++)
        {
            int dropDownWidth = int.Parse(resolutionDropdown.options[i].text.Split('x')[0]);
            if (dropDownWidth == width) return i;
        }
        Debug.Log("전달 받은 width에 해당하는 index를 리스트에서 찾을 수 없습니다.");
        return -1;
    }

    int GetScreenTypeId(FullScreenMode screenMode)
    {
        switch (screenMode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                return 0;
            case FullScreenMode.FullScreenWindow:
                return 1;
            case FullScreenMode.Windowed:
                return 2;
            default:
                return -1;
        }
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

            foreach (var languageType in languageTypes)
            {
                if (languageType.isOn)
                {
                    languageType.GetComponentInChildren<Text>().color = Color.white;
                }
                else
                {
                    languageType.GetComponentInChildren<Text>().color = new(1, 1, 1, 0.1f);
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

    void OnResolutionChanged()
    {
        if (resolutionDropdown.value == resolutionId) return;
        resolutionId = resolutionDropdown.value;
        var screenSize = resolutionDropdown.options[resolutionId].text.Split('x');
        int width = int.Parse(screenSize[0]);
        int height = int.Parse(screenSize[1]);
        //Screen.SetResolution(width, height, screenMode);
        PlayerPrefs.SetInt("screenWidth", width);
        PlayerPrefs.SetInt("screenHeight", height);
        PlayerPrefs.Save();
        LoadResolution();
    }

    void OnScreenTypeChanged()
    {
        for (int i = 0; i < screenTypes.Length; i++)
        {
            if (screenTypes[i].isOn)
            {
                switch (i)
                {
                    case 0:
                        if (screenMode == FullScreenMode.ExclusiveFullScreen) return;
                        screenMode = FullScreenMode.ExclusiveFullScreen;
                        break;
                    case 1:
                        if (screenMode == FullScreenMode.FullScreenWindow) return;
                        screenMode = FullScreenMode.FullScreenWindow;
                        break;
                    case 2:
                        if (screenMode == FullScreenMode.Windowed) return;
                        screenMode = FullScreenMode.Windowed;
                        break;
                }
                PlayerPrefs.SetInt("screenMode", (int)screenMode);
                PlayerPrefs.Save();
                LoadResolution();
                //if (Screen.fullScreenMode == FullScreenMode.Windowed)
                //{
                //    Screen.SetResolution(Screen.width, Screen.height, screenMode);
                //    return;
                //}
                //else
                //{
                //    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, screenMode);
                //    return;
                //}
            }
        }
    }

    void OnLanguageTypeChanged()
    {
        if (languageTypes[0].isOn)
        {
            currLanguage = LanguageType.Korean;
            InitLanguage();
            PlayerPrefs.SetString("language", "korean");
            PlayerPrefs.Save();
        }
        else
        {
            currLanguage = LanguageType.English;
            InitLanguage();
            PlayerPrefs.SetString("language", "english");
            PlayerPrefs.Save();
        }
    }
}
