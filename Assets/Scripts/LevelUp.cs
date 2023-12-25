using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelUp : MonoBehaviour
{
    public static LevelUp instance;

    public Text[] levelUpText;
    public Sprite[] buttonImages;
    public Image selectKeyImage;
    public Transform unSelectGroup;
    public Transform selectGroup;
    // 0: Power, 1: Speed, 2: Skill, 3: Dash, 4: Health
    public GameObject[] stats;
    public Text firstText;
    public bool isLevelUp;

    Vector3[] textOriginPos;
    ControllerManager.scheme currentScheme;
    GameObject currentEvent;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;

        textOriginPos = new Vector3[levelUpText.Length];
        for (int i = 0; i < textOriginPos.Length; i++)
        {
            textOriginPos[i] = levelUpText[i].transform.position;
        }

        currentScheme = ControllerManager.scheme.Undefined;
        isLevelUp = false;
        currentEvent = null;

        //if (!unSelectGroup.gameObject.activeSelf) unSelectGroup.gameObject.SetActive(true);
        //if (!selectGroup.gameObject.activeSelf) selectGroup.gameObject.SetActive(true);

        //foreach (var stat in stats)
        //{
        //    if (stat.activeSelf) stat.SetActive(false);
        //    if (stat.transform.parent != unSelectGroup)
        //    {
        //        stat.transform.parent = unSelectGroup;
        //    }
        //}
    }

    private void Start()
    {
        foreach (var stat in stats)
        {
            if (stat.activeSelf) stat.SetActive(false);
            if (stat.transform.parent != unSelectGroup)
            {
                stat.transform.SetParent(unSelectGroup);
            }
        }

        foreach (var element in GetComponentsInChildren<Transform>(true))
        {
            element.gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (currentEvent != null)
        {
            if (currentEvent != EventSystem.current.currentSelectedGameObject)
            {
                currentEvent = EventSystem.current.currentSelectedGameObject;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonChange);
            }
        }

        if (currentScheme == ControllerManager.instance.CurrentScheme) return;
        currentScheme = ControllerManager.instance.CurrentScheme;
        switch (currentScheme)
        {
            case ControllerManager.scheme.Keyboard:
                selectKeyImage.sprite = buttonImages[0];
                break;

            case ControllerManager.scheme.Gamepad:
                selectKeyImage.sprite = buttonImages[1];
                break;

            default:
                Debug.Log("Undefined Scheme from LevelUp");
                break;
        }

    }

    IEnumerator MoveText()
    {
        float timer = 0f;
        while (true)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;

            // 글자 "레"
            if (timer < .5f)
            {
                levelUpText[0].transform.localPosition += new Vector3(0f, 5f * Time.unscaledDeltaTime, 0f);
            }
            else if (timer < 1f)
            {
                levelUpText[0].transform.localPosition -= new Vector3(0f, 5f * Time.unscaledDeltaTime, 0f);
            }

            // 글자 "벨"
            if (timer > .1f)
            {
                if (timer < .6f)
                {
                    levelUpText[1].transform.localPosition += new Vector3(0f, 5f * Time.unscaledDeltaTime, 0f);
                }
                else if (timer < 1.1f)
                {
                    levelUpText[1].transform.localPosition -= new Vector3(0f, 5f * Time.unscaledDeltaTime, 0f);
                }
            }

            // 글자 "업"
            if (timer > .2f)
            {
                if (timer < .7f)
                {
                    levelUpText[2].transform.localPosition += new Vector3(0f, 5f * Time.unscaledDeltaTime, 0f);
                }
                else if (timer < 1.2f)
                {
                    levelUpText[2].transform.localPosition -= new Vector3(0f, 5f * Time.unscaledDeltaTime, 0f);
                }
            }

            // 글자 "!"
            if (timer > .3f)
            {
                if (timer < .8f)
                {
                    levelUpText[3].transform.localPosition += new Vector3(0f, 5f * Time.unscaledDeltaTime, 0f);
                }
                else if (timer < 1.3f)
                {
                    levelUpText[3].transform.localPosition -= new Vector3(0f, 5f * Time.unscaledDeltaTime, 0f);
                }
                // 모든 글자가 왕복을 마치면 timer 초기화 및 위치 초기화
                else
                {
                    timer = 0f;
                    for (int i = 0; i < textOriginPos.Length; i++)
                    {
                        levelUpText[i].transform.position = textOriginPos[i];
                    }
                }
            }
        }
    }

    public void Do()
    {
        isLevelUp = true;
        currentEvent = null;
        AudioManager.instance.PauseBGM(true);
        gameObject.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        float timer = 0f;
        firstText.transform.localEulerAngles = Vector3.zero;
        firstText.fontSize = 1;
        if (!firstText.gameObject.activeSelf) firstText.gameObject.SetActive(true);
        while (timer < 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            firstText.fontSize = Mathf.Max(1, Mathf.FloorToInt(timer * 20));
            if (timer < .3f)
            {
                firstText.transform.localEulerAngles = new Vector3(0, 0, timer * 360 * 2 / .3f);
            }
            else
            {
                firstText.transform.localEulerAngles = Vector3.zero;
            }
        }
        
        Show();
    }

    public void Show()
    {
        for (int i = 0; i < textOriginPos.Length; i++)
        {
            levelUpText[i].transform.position = textOriginPos[i];
        }

        foreach (var element in GetComponentsInChildren<Transform>(true))
        {
            element.gameObject.SetActive(true);
        }
        firstText.gameObject.SetActive(false);

        StartCoroutine(MoveText());

        foreach (var stat in stats)
        {
            if (stat.activeSelf) stat.SetActive(false);
            if (stat.transform.parent != unSelectGroup)
            {
                stat.transform.SetParent(unSelectGroup);
            }
        }

        int[] playerLevels = new int[] {GameManager.Instance.playerDamageLevel, GameManager.Instance.playerSpeedLevel,
                                        GameManager.Instance.playerHealthLevel, GameManager.Instance.playerSkillLevel,
                                        GameManager.Instance.playerDashLevel};

        // Max Level이 아닌 경우만 고려
        List<int> levelArr = new List<int> { };
        if (GameManager.Instance.playerDamageLevel < 3) levelArr.Add(0);
        if (GameManager.Instance.playerSpeedLevel < 3) levelArr.Add(1);
        if (GameManager.Instance.playerHealthLevel < 4) levelArr.Add(2);
        if (GameManager.Instance.playerSkillLevel < 6) levelArr.Add(3);
        if (GameManager.Instance.playerDashLevel < 4) levelArr.Add(4);

        List<int> select = new List<int>();
        while (select.Count < Mathf.Min(levelArr.Count, 3))
        {
            int randIndex = Random.Range(0, levelArr.Count);
            if (select.Contains(randIndex))
            {
                continue;
            }
            else
            {
                select.Add(randIndex);
            }
        }

        foreach (var index in select)
        {
            stats[index].transform.SetParent(selectGroup);
            stats[index].SetActive(true);
        }

        EventSystem.current.SetSelectedGameObject(stats[select[0]]);
        currentEvent = EventSystem.current.currentSelectedGameObject;
    }


    public void Up(int levelIndex)
    {
        switch(levelIndex)
        {
            case 0:
                GameManager.Instance.playerDamageLevel++;
                break;

            case 1:
                GameManager.Instance.playerSpeedLevel++;
                break;

            case 2:
                GameManager.Instance.playerHealthLevel++;
                break;

            case 3:
                GameManager.Instance.playerSkillLevel++;
                break;

            case 4:
                GameManager.Instance.playerDashLevel++;
                break;
        }

        AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
        AudioManager.instance.PauseBGM(false);
        GameManager.Instance.Resume();
        isLevelUp = false;
        currentEvent = null;

        foreach (var element in GetComponentsInChildren<Transform>(true))
        {
            element.gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }
}
