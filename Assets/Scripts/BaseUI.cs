using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
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
        ChangeAlpha(0f, death, deathTitle, deathMessage);
        resurrection.gameObject.SetActive(false);
        death.gameObject.SetActive(true);
        StartCoroutine(ChangeAlphaCoroutine());
    }

    void ChangeAlpha (float alpha, params MaskableGraphic[] components)
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

        while (timer <  endSecond)
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
        victoryTitle.gameObject.SetActive(false);
        victoryMessage.gameObject.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(null);
        AudioManager.instance.PauseBGM(false);

        GameManager.instance.Resume();

        isComeBack = false;
        remainTime.text = "10초 후 귀환합니다.";
        remainTime.gameObject.SetActive(true);
        float remainTimer = 10f;
        while (remainTimer > 0f)
        {
            remainTime.text = $"{Mathf.FloorToInt(remainTimer)}초 후 귀환합니다.";
            yield return null;
            remainTimer -= Time.deltaTime;
        }
        GameManager.instance.sceneName = "Camp";
        GameManager.instance.FadeOut();
    }
}

