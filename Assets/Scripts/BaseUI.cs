using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseUI : MonoBehaviour
{
    [SerializeField] Image death;
    [SerializeField] Sprite[] sprites;
    [SerializeField] Image selectHelp;
    [SerializeField] Text remainTime;
    public Image victory;

    Text deathTitle;
    Text deathMessage;
    Button resurrection;

    Text victoryTitle;
    Text victoryMessage;
    Text comeBack;
    Text later;
    Text rightNow;

    GameObject currentSelected;

    bool isOnClick;
    bool isComeBack;

    public static BaseUI Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);

        deathTitle = death.GetComponentsInChildren<Text>(true)[0];
        deathMessage = death.GetComponentsInChildren<Text>(true)[1];
        resurrection = death.GetComponentInChildren<Button>(true);
        isOnClick = false;

        victoryTitle = victory.GetComponentsInChildren<Text>(true)[0];
        victoryMessage = victory.GetComponentsInChildren<Text>(true)[1];
        comeBack = victory.GetComponentsInChildren<Text>(true)[2];
        later = victory.GetComponentsInChildren<Text>(true)[3];
        rightNow = victory.GetComponentsInChildren<Text>(true)[4];
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //rightNow.GetComponent<Animator>().SetBool("PressedByScript", false);
        //later.GetComponent<Animator>().SetBool("PressedByScript", false);
        resurrection.gameObject.SetActive(false);

        isOnClick = false;
        ChangeAlpha(0f, death, deathTitle, deathMessage);
        death.gameObject.SetActive(false);

        ChangeAlpha(0f, victory, victoryTitle, victoryMessage);
        victory.gameObject.SetActive(false);
        comeBack.gameObject.SetActive(false);
        later.gameObject.SetActive(false);
        rightNow.gameObject.SetActive(false);
        selectHelp.gameObject.SetActive(false);
        isComeBack = false;

        remainTime.gameObject.SetActive(false);

        currentSelected = null;
    }

    void OnSceneUnloaded(Scene scene)
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        if (resurrection.gameObject.activeSelf || selectHelp.gameObject.activeSelf)
        {
            switch (ControllerManager.instance.CurrentScheme)
            {
                case ControllerManager.scheme.Keyboard:
                    resurrection.GetComponent<Image>().sprite = sprites[0];
                    selectHelp.GetComponent<Image>().sprite = sprites[0];
                    break;

                case ControllerManager.scheme.Gamepad:
                    resurrection.GetComponent<Image>().sprite = sprites[1];
                    selectHelp.GetComponent<Image>().sprite = sprites[1];
                    break;
            }
        }

        if (isComeBack)
        {
            if (currentSelected == rightNow.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(rightNow.gameObject);
            }
            else if (currentSelected == later.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(later.gameObject);
            }

            return;
        }

        if (EventSystem.current.currentSelectedGameObject is not null)
        {
            if (resurrection.gameObject.activeSelf || rightNow.gameObject.activeSelf)
            {
                if (currentSelected is null)
                {
                    currentSelected = EventSystem.current.currentSelectedGameObject;
                }
                else if (currentSelected != EventSystem.current.currentSelectedGameObject)
                {
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.MenuChange);
                    currentSelected = EventSystem.current.currentSelectedGameObject;
                }
            }
        }
    }

    public void Death()
    {
        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
        {
            deathMessage.text = "소지금을 제외한 모든 아이템을 잃었습니다.";
        }
        else
        {
            deathMessage.text = "You lost all items except for the gold.";
        }

        ChangeAlpha(0f, death, deathTitle, deathMessage);
        resurrection.gameObject.SetActive(false);
        death.gameObject.SetActive(true);
        StartCoroutine(ChangeAlphaCoroutine());
    }

    void ChangeAlpha(float alpha, params MaskableGraphic[] components)
    {
        foreach (var component in components)
        {
            Color originColor = component.color;
            originColor.a = alpha;
            component.color = originColor;
        }
    }
    IEnumerator ChangeAlphaCoroutine()
    {
        float timer = 0f;
        float endSecond = 3f;
        float targetAlpha = .9f;

        while (timer < endSecond)
        {
            ChangeAlpha(timer / endSecond * targetAlpha, death, deathTitle, deathMessage);
            yield return null;
            timer += Time.unscaledDeltaTime;
        }
        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
        {
            resurrection.GetComponentInChildren<Text>(true).text = "야영지에서 부활하기";
        }
        else
        {
            resurrection.GetComponentInChildren<Text>(true).text = "Respawning at the Camp";
        }
        resurrection.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(resurrection.gameObject);
    }

    public void Resurrect()
    {
        if (isOnClick) return;
        isOnClick = true;

        GameManager.instance.sceneName = "Camp";
        AudioManager.instance.PlaySfx(AudioManager.Sfx.MenuSelect);
        GameManager.instance.FadeOut();
    }

    public void Victory()
    {
        InitLanguage();
        ChangeAlpha(0f, victory, victoryTitle, victoryMessage);
        comeBack.gameObject.SetActive(false);
        later.gameObject.SetActive(false);
        rightNow.gameObject.SetActive(false);

        victory.gameObject.SetActive(true);
        StartCoroutine(VictoryMessageCoroutine());
    }
    IEnumerator VictoryMessageCoroutine()
    {
        float timer = 0f;
        float endSecond = 3f;
        float targetAlpha = .7f;

        while (timer < endSecond)
        {
            ChangeAlpha(timer / endSecond * targetAlpha, victory, victoryTitle, victoryMessage);
            yield return null;
            timer += Time.unscaledDeltaTime;
        }
        comeBack.gameObject.SetActive(true);
        later.gameObject.SetActive(true);
        rightNow.gameObject.SetActive(true);
        selectHelp.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(later.gameObject);
    }

    public void ComeBack(bool now)
    {
        if (isComeBack) return;
        if (now)
        {
            isComeBack = true;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.MenuSelect);
            StartCoroutine(Press(rightNow.gameObject));
            GameManager.instance.sceneName = "Camp";
            GameManager.instance.FadeOut();
        }
        else
        {
            isComeBack = true;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.MenuSelect);
            StartCoroutine(Press(later.gameObject));
            StartCoroutine(LaterComeBack());
        }
    }

    IEnumerator Press(GameObject button)
    {
        yield return null;
        button.GetComponent<Animator>().SetBool("PressedByScript", true);
    }

    IEnumerator LaterComeBack()
    {
        float timer = 0f;
        float endSecond = 1f;
        float targetAlpha = .7f;

        while (timer < endSecond)
        {
            if (timer > .5f)
            {
                if (comeBack.gameObject.activeSelf) comeBack.gameObject.SetActive(false);
                if (later.gameObject.activeSelf) later.gameObject.SetActive(false);
                if (rightNow.gameObject.activeSelf) rightNow.gameObject.SetActive(false);
                if (selectHelp.gameObject.activeSelf) selectHelp.gameObject.SetActive(false);
            }

            ChangeAlpha(targetAlpha - (timer / endSecond * targetAlpha), victory, victoryTitle, victoryMessage);
            yield return null;
            timer += Time.unscaledDeltaTime;
        }
        victory.gameObject.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        AudioManager.instance.PauseBGM(false);

        GameManager.instance.Resume();

        isComeBack = false;
        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
        {
            remainTime.text = "10초 후 귀환합니다.";
        }
        else
        {
            remainTime.text = "Return in 10 seconds.";
        }
        remainTime.gameObject.SetActive(true);
        float remainTimer = 10f;
        while (remainTimer > 0f)
        {
            if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
            {
                remainTime.text = $"{Mathf.FloorToInt(remainTimer)}초 후 귀환합니다.";
            }
            else
            {
                remainTime.text = $"Return in {Mathf.FloorToInt(remainTimer)} seconds.";
            }
            yield return null;
            remainTimer -= Time.deltaTime;
        }
        GameManager.instance.sceneName = "Camp";
        GameManager.instance.FadeOut();
    }

    void InitLanguage()
    {
        Dictionary<string, string[]> nameDic = new();
        nameDic["Victory Message"] = new string[] { "대단합니다! 모든 적을 섬멸했습니다.", "Excellent! You have successfully eliminated all enemies." };
        nameDic["Come Back Text"] = new string[] { "야영지로 돌아갑니다.", "Returning to the Camp." };
        nameDic["Later Text"] = new string[] { "10초 후 돌아가기", "Return in 10 seconds" };
        nameDic["Right Now Text"] = new string[] { "즉시 돌아가기", "Return right now" };
        nameDic["Help Text"] = new string[] { "선택", "Select" };

        var texts = transform.GetComponentsInChildren<Text>(true);
        int textId = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? 0 : 1;
        foreach (var text in texts)
        {
            if (nameDic.ContainsKey(text.name))
            {
                text.text = nameDic[text.name][textId];
            }
        }
    }
}

