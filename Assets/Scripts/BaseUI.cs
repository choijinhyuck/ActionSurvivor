using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class BaseUI : MonoBehaviour
{
    [SerializeField] Image death;

    Text deathTitle;
    Text deathMessage;

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

        deathTitle = death.GetComponentsInChildren<Text>()[0];
        deathMessage = death.GetComponentsInChildren<Text>()[1];


    }

    public void Death()
    {
        ChangeAlpha(0f, death, deathTitle, deathMessage);
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
    }
}

