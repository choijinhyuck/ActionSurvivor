using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    Vector3[] textOriginPos;
    ControllerManager.scheme currentScheme;

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
        
        foreach (var stat in stats)
        {
            if (stat.activeSelf) stat.SetActive(false);
            if (stat.transform.parent != unSelectGroup)
            {
                stat.transform.parent = unSelectGroup;
            }
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < textOriginPos.Length;i++)
        {
            levelUpText[i].transform.position = textOriginPos[i];
        }

        StartCoroutine(MoveText());

        foreach (var stat in stats)
        {
            if (stat.activeSelf) stat.SetActive(false);
            if (stat.transform.parent != unSelectGroup)
            {
                stat.transform.parent = unSelectGroup;
            }
        }

        Show();
    }

    private void LateUpdate()
    {
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
            timer += Time.deltaTime;
            
            // 글자 "레"
            if (timer < .5f)
            {
                levelUpText[0].transform.localPosition += new Vector3(0f, 5f * Time.deltaTime, 0f);
            }
            else if (timer < 1f)
            {
                levelUpText[0].transform.localPosition -= new Vector3(0f, 5f * Time.deltaTime, 0f);
            }

            // 글자 "벨"
            if (timer > .1f)
            {
                if (timer < .6f)
                {
                    levelUpText[1].transform.localPosition += new Vector3(0f, 5f * Time.deltaTime, 0f);
                }
                else if (timer < 1.1f)
                {
                    levelUpText[1].transform.localPosition -= new Vector3(0f, 5f * Time.deltaTime, 0f);
                }
            }

            // 글자 "업"
            if (timer > .2f)
            {
                if (timer < .7f)
                {
                    levelUpText[2].transform.localPosition += new Vector3(0f, 5f * Time.deltaTime, 0f);
                }
                else if (timer < 1.2f)
                {
                    levelUpText[2].transform.localPosition -= new Vector3(0f, 5f * Time.deltaTime, 0f);
                }
            }

            // 글자 "!"
            if (timer > .3f)
            {
                if (timer < .8f)
                {
                    levelUpText[3].transform.localPosition += new Vector3(0f, 5f * Time.deltaTime, 0f);
                }
                else if (timer < 1.3f)
                {
                    levelUpText[3].transform.localPosition -= new Vector3(0f, 5f * Time.deltaTime, 0f);
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


    public void Show()
    {
        int[] statArr = new int[] { 0, 0, 0, 1, 1, 1, 2, 2, 3, 3, 4 };
        // max level인 경우 고려
        
        

        int randIndex = Random.Range(0, statArr.Length);
        
        
    }
}
