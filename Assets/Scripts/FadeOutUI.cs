using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeOutUI : MonoBehaviour
{
    Image fadeOut;
    float time;

    private void Awake()
    {
        fadeOut = GetComponent<Image>();
        time = 1.5f;
    }
    private void Start()
    {
        GameManager.instance.Stop();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        Color color = new(0f, 0f, 0f, 0f);
        float timer = 0f;
        while (timer < time)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            color.a = timer / time;
            fadeOut.color = color;
        }

        SceneManager.LoadScene("Loading");
    }
}
