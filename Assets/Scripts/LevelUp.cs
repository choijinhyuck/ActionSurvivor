using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA;

public class LevelUp : MonoBehaviour
{
    public Text[] levelUpText;

    Vector3[] textOriginPos;

    private void Awake()
    {
        textOriginPos = new Vector3[levelUpText.Length];
        for (int i = 0; i < textOriginPos.Length; i++)
        {
            textOriginPos[i] = levelUpText[i].transform.position;
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < textOriginPos.Length;i++)
        {
            levelUpText[i].transform.position = textOriginPos[i];
        }

        StartCoroutine(MoveText());
    }

    IEnumerator MoveText()
    {
        float timer = 0f;
        while (true)
        {
            
            yield return null;
            timer += Time.deltaTime;
            levelUpText[0].transform.localPosition += new Vector3(0f, 5f * Time.deltaTime, 0f);
            if (timer > 1f)
            {
                
            }
        }
    }
}
