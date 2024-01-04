using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeInUI : MonoBehaviour
{
    Image fadeIn;
    float time;

    private void Awake()
    {
        fadeIn = GetComponent<Image>();
        time = 1.5f;
    }
    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        Color color = new Color(0f, 0f, 0f, 1f);
        float timer = 0f;
        while (timer < time)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            color.a = (time - timer) / time;
            fadeIn.color = color;
        }
        Destroy(gameObject);
    }
}
