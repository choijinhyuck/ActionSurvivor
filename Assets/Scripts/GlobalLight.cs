using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLight : MonoBehaviour
{
    Light2D light2d;
    Color lightRed;
    bool isWarning;


    private void Awake()
    {
        light2d = GetComponent<Light2D>();
        light2d.color = Color.white;
        isWarning = false;
        lightRed = new Color(1f, 0.5f, 0.5f, 1f);
    }

    private void LateUpdate()
    {
        if (GameManager.instance.health < 1.1f & GameManager.instance.health > 0.1f & !isWarning)
        {
            StartCoroutine("Warning");
            AudioManager.instance.PauseBGM(true);
        }
        else if (GameManager.instance.health > 1.1f & isWarning)
        {
            StopCoroutine("Warning");
            isWarning = false;
            light2d.color = Color.white;
            AudioManager.instance.PauseBGM(false);
        }
        else if (GameManager.instance.health < 0.1f)
        {
            StopCoroutine("Warning");
            isWarning = false;
            light2d.color = Color.white;
        }
    }

    IEnumerator Warning()
    {
        float timer = 0f;
        isWarning = true;
        light2d.color = lightRed;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.HeartBeat);
        while (true)
        {
            yield return null;
            timer += Time.deltaTime;

            if (timer < .2f)
            {
                light2d.color += new Color(0f, -0.4f * Time.deltaTime / .2f, -0.4f * Time.deltaTime / .2f);
            }
            else if (timer < .45f)
            {
                light2d.color += new Color(0f, 0.4f * Time.deltaTime / .25f, 0.4f * Time.deltaTime / .25f);
            }
            else if (timer > 1.3f)
            {
                timer = 0f;
                light2d.color = lightRed;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.HeartBeat);
            }
        }
    }

    public void WarningToTrue()
    {
        isWarning = true;
    }
}
