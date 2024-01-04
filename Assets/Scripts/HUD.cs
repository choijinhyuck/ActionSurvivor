using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType
    {
        Exp, Level, Kill, Time, Health, EnemyCount, StageName
    }
    public InfoType type;
    public Sprite[] heartImages;


    Text myText;
    Slider mySlider;
    List<Image> hearts;
    GameObject heartMold;
    WaitForFixedUpdate waitFix;

    float maxHealth;
    int intMaxHealth;
    bool isLosing;
    bool alreadyStarted;

    private void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
        waitFix = new WaitForFixedUpdate();
        alreadyStarted = false;
        hearts = new List<Image>();
    }

    private void OnEnable()
    {
        isLosing = false;
        if (!alreadyStarted) return;
        if (type == InfoType.Health)
        {
            heartMold = transform.GetChild(0).gameObject;
            InitHeart();
        }
    }

    private void Start()
    {
        alreadyStarted = true;
        if (type == InfoType.Health)
        {
            heartMold = transform.GetChild(0).gameObject;
            InitHeart();
        }
    }

    private void LateUpdate()
    {
        switch (type)
        {
            case InfoType.Exp:
                float curExp = GameManager.Instance.exp;
                float maxExp = GameManager.Instance.nextExp[Mathf.Min(GameManager.Instance.level, GameManager.Instance.nextExp.Length - 1)];
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Level:
                if (GameManager.Instance.level == 20)
                {
                    myText.text = string.Format("Lv.Max");
                }
                else
                {
                    myText.text = string.Format("Lv.{0:F0}", GameManager.Instance.level + 1);
                }
                break;
            case InfoType.Kill:
                switch (GameManager.Instance.kill)
                {
                    case 0:
                        myText.text = string.Format("{0:F0} kill", 0);
                        break;
                    case 1:
                        myText.text = string.Format("{0:F0} kill", 1);
                        break;
                    default:
                        myText.text = string.Format("{0:F0} kills", GameManager.Instance.kill);
                        break;
                }
                break;
            case InfoType.Time:
                float remainTime = Mathf.Max(0f, GameManager.Instance.maxGameTime - GameManager.Instance.gameTime);
                if (remainTime < 10)
                {
                    myText.color = Color.red;
                }
                else
                {
                    myText.color = Color.white;
                }
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
            case InfoType.Health:
                InitHeart();
                float curHealth = GameManager.Instance.health;
                int intCurHealth = Mathf.FloorToInt(curHealth);
                int heartCount = 0;

                for (int i = 0; i < intCurHealth; i++)
                {
                    hearts[i].sprite = heartImages[0];
                    heartCount++;
                }

                if (curHealth - intCurHealth > 0.1f)
                {
                    hearts[heartCount].sprite = heartImages[1];
                    heartCount++;
                }

                while (heartCount + 1 <= intMaxHealth)
                {
                    hearts[heartCount].sprite = heartImages[2];
                    heartCount++;
                }

                if (GameManager.Instance.player.isHit && !isLosing) StartCoroutine("LoseHeart");

                break;

            case InfoType.EnemyCount:
                if (GameManager.Instance.maxGameTime < GameManager.Instance.gameTime)
                {
                    int count = PoolManager.instance.EnemyCount();
                    GetComponent<Text>().text = $"³²Àº Àû: {count}";
                }
                else
                {
                    GetComponent<Text>().text = "";
                }
                break;

            case InfoType.StageName:
                if (GetComponent<Text>().text == GameManager.Instance.stage.stageDataArr[GameManager.Instance.stageId].stageName) return;
                GetComponent<Text>().text = GameManager.Instance.stage.stageDataArr[GameManager.Instance.stageId].stageName;
                break;
        }
    }

    IEnumerator LoseHeart()
    {
        isLosing = true;
        Vector2 deltaVec = new Vector2(1f, 0f);
        Vector2 originVec = GetComponent<RectTransform>().anchoredPosition;

        for (int i = 0; i < 3; i++)
        {
            GetComponent<RectTransform>().anchoredPosition = originVec + deltaVec;
            yield return waitFix;
            GetComponent<RectTransform>().anchoredPosition = originVec;
            yield return waitFix;
            GetComponent<RectTransform>().anchoredPosition = originVec - deltaVec;
            yield return waitFix;
            GetComponent<RectTransform>().anchoredPosition = originVec;
            yield return waitFix;
        }
        isLosing = false;
    }

    void InitHeart()
    {
        if (hearts.Count == 0) hearts.Add(transform.GetChild(0).GetComponent<Image>());
        maxHealth = GameManager.Instance.maxHealth;
        intMaxHealth = Mathf.FloorToInt(maxHealth);

        if (transform.childCount == intMaxHealth)
        {
            return;
        }
        else if (transform.childCount < intMaxHealth)
        {
            heartMold.GetComponent<Image>().sprite = heartImages[0];
            for (int i = 0; i < intMaxHealth - transform.childCount; i++)
            {
                GameObject newHeart = Instantiate(heartMold, transform);
                newHeart.GetComponent<Image>().sprite = heartImages[0];
                hearts.Add(newHeart.GetComponent<Image>());
            }
        }
        else
        {
            for (int i = 0; i < transform.childCount - intMaxHealth; i++)
            {
                hearts.RemoveAt(hearts.Count - 1);
                Destroy(transform.GetChild(i + intMaxHealth).gameObject);
            }
        }

    }
}
