using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
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

    Vector2[] textOriginPos;
    ControllerManager.scheme currentScheme;
    GameObject currentEvent;

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

        textOriginPos = new Vector2[levelUpText.Length];
        for (int i = 0; i < textOriginPos.Length; i++)
        {
            textOriginPos[i] = levelUpText[i].GetComponent<RectTransform>().anchoredPosition;
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
                levelUpText[0].GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, 5f * Time.unscaledDeltaTime);
            }
            else if (timer < 1f)
            {
                levelUpText[0].GetComponent<RectTransform>().anchoredPosition -= new Vector2(0f, 5f * Time.unscaledDeltaTime);
            }

            // 글자 "벨"
            if (timer > .1f)
            {
                if (timer < .6f)
                {
                    levelUpText[1].GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, 5f * Time.unscaledDeltaTime);
                }
                else if (timer < 1.1f)
                {
                    levelUpText[1].GetComponent<RectTransform>().anchoredPosition -= new Vector2(0f, 5f * Time.unscaledDeltaTime);
                }
            }

            // 글자 "업"
            if (timer > .2f)
            {
                if (timer < .7f)
                {
                    levelUpText[2].GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, 5f * Time.unscaledDeltaTime);
                }
                else if (timer < 1.2f)
                {
                    levelUpText[2].GetComponent<RectTransform>().anchoredPosition -= new Vector2(0f, 5f * Time.unscaledDeltaTime);
                }
            }

            // 글자 "!"
            if (timer > .3f)
            {
                if (timer < .8f)
                {
                    levelUpText[3].GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, 5f * Time.unscaledDeltaTime);
                }
                else if (timer < 1.3f)
                {
                    levelUpText[3].GetComponent<RectTransform>().anchoredPosition -= new Vector2(0f, 5f * Time.unscaledDeltaTime);
                }
                // 모든 글자가 왕복을 마치면 timer 초기화 및 위치 초기화
                else
                {
                    timer = 0f;
                    for (int i = 0; i < textOriginPos.Length; i++)
                    {
                        levelUpText[i].GetComponent<RectTransform>().anchoredPosition = textOriginPos[i];
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
        firstText.transform.localScale = Vector3.zero;
        if (!firstText.gameObject.activeSelf) firstText.gameObject.SetActive(true);
        while (timer < 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            firstText.transform.localScale = new Vector3(timer, timer, timer);
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
            levelUpText[i].GetComponent<RectTransform>().anchoredPosition = textOriginPos[i];
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

        int[] playerLevels = new int[] {GameManager.instance.playerDamageLevel, GameManager.instance.playerSpeedLevel,
                                        GameManager.instance.playerHealthLevel, GameManager.instance.playerSkillLevel,
                                        GameManager.instance.playerDashLevel};

        // Max Level이 아닌 경우만 고려
        List<int> levelArr = new List<int> { };
        if (GameManager.instance.playerDamageLevel < 3) levelArr.Add(0);
        if (GameManager.instance.playerSpeedLevel < 3) levelArr.Add(1);
        if (GameManager.instance.playerHealthLevel < 4) levelArr.Add(2);
        if (GameManager.instance.playerSkillLevel < 6) levelArr.Add(3);
        if (GameManager.instance.playerDashLevel < 4) levelArr.Add(4);


        List<int> select = new List<int>();
        while (select.Count < Mathf.Min(levelArr.Count, 3))
        {
            int randIndex = Random.Range(0, levelArr.Count);
            if (select.Contains(levelArr[randIndex]))
            {
                continue;
            }
            else
            {
                select.Add(levelArr[randIndex]);
            }
        }

        foreach (var index in select)
        {
            stats[index].transform.SetParent(selectGroup);
            stats[index].SetActive(true);
            Text[] statTexts = stats[index].GetComponentsInChildren<Text>();
            string[] texts = GetDesc(index);
            statTexts[0].text = texts[0];
            statTexts[1].text = texts[1];
            //    switch (index)
            //    {
            //        case 0:
            //            statTexts[0].text = string.Format("힘 <size=9><color=blue>Lv.{0}</color></size>", GameManager.Instance.playerDamageLevel + 1);
            //            statTexts[1].text = string.Format("공격력 <color=red><size=7>{0}</size></color> 증가", 1);
            //            break;

            //        case 1:
            //            statTexts[0].text = string.Format("민첩 <size=9><color=blue>Lv.{0}</color></size>", GameManager.Instance.playerSpeedLevel + 1);
            //            statTexts[1].text = string.Format("이동속도 <color=red><size=7>{0}</size></color> 증가", 1);
            //            break;

            //        case 2:
            //            statTexts[0].text = string.Format("건강 <size=9><color=blue>Lv.{0}</color></size>", GameManager.Instance.playerHealthLevel + 1);
            //            statTexts[1].text = string.Format("최대 하트 <color=red><size=7>{0}</size></color> 증가", 1);
            //            break; ;

            //        case 3:
            //            statTexts[0].text = string.Format("기술 <size=9><color=blue>Lv.{0}</color></size>", GameManager.Instance.playerSkillLevel + 1);
            //            statTexts[1].text = string.Format("기술 재사용 시간 감소");
            //            break;

            //        case 4:
            //            statTexts[0].text = string.Format("대시 <size=9><color=blue>Lv.{0}</color></size>", GameManager.Instance.playerDashLevel + 1);
            //            statTexts[1].text = string.Format("대시 재사용 시간 감소");
            //            break;
            //    }
            //}
        }
        EventSystem.current.SetSelectedGameObject(stats[select[0]]);
        currentEvent = EventSystem.current.currentSelectedGameObject;
    }


    public void Up(int levelIndex)
    {
        StatUp(levelIndex);

        GameManager.instance.StatusUpdate();

        AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
        if (GameManager.instance.health > 1.1f)
        {
            AudioManager.instance.PauseBGM(false);
        }
        GameManager.instance.Resume();
        isLevelUp = false;
        currentEvent = null;

        foreach (var element in GetComponentsInChildren<Transform>(true))
        {
            element.gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    string[] GetDesc(int index)
    {
        string[] texts = new string[2];
        switch (index)
        {
            case 0:
                if (GameManager.instance.playerDamageLevel == 2)
                {
                    texts[0] = string.Format("힘 <size=9><color=blue>Lv.{0}</color> <color=red>(Max)</color></size>", GameManager.instance.playerDamageLevel + 1);
                }
                else
                {
                    texts[0] = string.Format("힘 <size=9><color=blue>Lv.{0}</color></size>", GameManager.instance.playerDamageLevel + 1);
                }
                texts[1] = string.Format("공격력 <color=red><size=7>{0}</size></color> 증가", 1);
                break;

            case 1:
                if (GameManager.instance.playerSpeedLevel == 2)
                {
                    texts[0] = string.Format("민첩 <size=9><color=blue>Lv.{0}</color> <color=red>(Max)</color></size>", GameManager.instance.playerSpeedLevel + 1);
                }
                else
                {
                    texts[0] = string.Format("민첩 <size=9><color=blue>Lv.{0}</color></size>", GameManager.instance.playerSpeedLevel + 1);
                }
                texts[1] = string.Format("이동속도 <color=red><size=7>{0}</size></color> 증가", 1);
                break;

            case 2:
                if (GameManager.instance.playerHealthLevel == 3)
                {
                    texts[0] = string.Format("건강 <size=9><color=blue>Lv.{0}</color> <color=red>(Max)</color></size>", GameManager.instance.playerHealthLevel + 1);
                    texts[1] = "피격 시 무적시간 <size=7><color=red>100%</color></size> 증가";

                }
                else
                {
                    texts[0] = string.Format("건강 <size=9><color=blue>Lv.{0}</color></size>", GameManager.instance.playerHealthLevel + 1);
                    texts[1] = string.Format("최대 하트 <color=red><size=7>{0}</size></color> 증가", 1);

                }
                break;

            case 3:
                texts[0] = string.Format("기술 <size=9><color=blue>Lv.{0}</color></size>", GameManager.instance.playerSkillLevel + 1);
                switch (GameManager.instance.playerSkillLevel)
                {
                    case 5:
                        texts[0] = string.Format("기술 <size=9><color=blue>Lv.{0}</color> <color=red>(Max)</color></size>", GameManager.instance.playerSkillLevel + 1);
                        texts[1] = string.Format("<size=7><color=red>3단계</color></size> 충전 기술 개방");
                        break;
                    case 4:
                        texts[1] = string.Format("최대 기술 사용 횟수 <size=7><color=red>2</color></size> 증가");
                        break;
                    case 3:
                        texts[1] = string.Format("기 1회 충전 소요 시간 <size=7><color=red>20%</color></size> 단축");
                        break;
                    case 2:
                        texts[1] = string.Format("<size=7><color=red>2단계</color></size> 충전 기술 개방");
                        break;
                    case 1:
                        texts[1] = string.Format("최대 기술 사용 횟수 <size=7><color=red>1</color></size> 증가");
                        break;
                    default:
                        texts[1] = string.Format("기 1회 충전 소요 시간 <size=7><color=red>20%</color></size> 단축");
                        break;
                }
                break;

            case 4:
                switch (GameManager.instance.playerDashLevel)
                {
                    case 3:
                        texts[0] = string.Format("대시 <size=9><color=blue>Lv.{0}</color> <color=red>(Max)</color></size>", GameManager.instance.playerDashLevel + 1);
                        texts[1] = string.Format("대시 이동거리 <size=7><color=red>30%</color></size> 증가");
                        break;
                    case 2:
                        texts[0] = string.Format("대시 <size=9><color=blue>Lv.{0}</color></size>", GameManager.instance.playerDashLevel + 1);
                        texts[1] = string.Format("대시 재사용 시간 <size=7><color=red>20%</color></size> 감소");
                        break;
                    case 1:
                        texts[0] = string.Format("대시 <size=9><color=blue>Lv.{0}</color></size>", GameManager.instance.playerDashLevel + 1);
                        texts[1] = string.Format("대시 재사용 시간 <size=7><color=red>15%</color></size> 감소");
                        break;
                    default:
                        texts[0] = string.Format("대시 <size=9><color=blue>Lv.{0}</color></size>", GameManager.instance.playerDashLevel + 1);
                        texts[1] = string.Format("대시 재사용 시간 <size=7><color=red>10%</color></size> 감소");
                        break;
                }
                break;
        }
        return texts;
    }

    void StatUp(int levelIndex)
    {
        switch (levelIndex)
        {
            case 0:
                GameManager.instance.playerDamageLevel++;
                break;

            case 1:
                GameManager.instance.playerSpeedLevel++;
                break;

            case 2:
                if (GameManager.instance.playerHealthLevel == 3)
                {
                    GameManager.instance.playerImmuneTime *= 2;
                }
                else
                {
                    GameManager.instance.health++;
                }
                GameManager.instance.playerHealthLevel++;
                break;

            case 3:
                switch (GameManager.instance.playerSkillLevel)
                {
                    case 5:
                        GameManager.instance.maxChargibleCount = 3;
                        break;
                    case 4:
                        GameManager.instance.maxChargeCount += 2;
                        break;
                    case 3:
                        GameManager.instance.chargeTime *= .8f;
                        break;
                    case 2:
                        GameManager.instance.maxChargibleCount = 2;
                        break;
                    case 1:
                        GameManager.instance.maxChargeCount += 1;
                        break;
                    default:
                        GameManager.instance.chargeTime *= .8f;
                        break;
                }
                GameManager.instance.playerSkillLevel++;
                break;

            case 4:
                switch (GameManager.instance.playerDashLevel)
                {
                    case 3:
                        GameManager.instance.dodgeSpeed *= 1.3f;
                        break;
                    case 2:
                        GameManager.instance.dodgeTime *= .8f;
                        break;
                    case 1:
                        GameManager.instance.dodgeTime *= .85f;
                        break;
                    default:
                        GameManager.instance.dodgeTime *= .9f;
                        break;
                }
                GameManager.instance.playerDashLevel++;
                break;
        }
    }
}
