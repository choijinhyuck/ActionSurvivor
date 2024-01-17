using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class WarningUI : MonoBehaviour
{
    [SerializeField] float minAlpha;
    [SerializeField] float maxAlpha;

    Image overlayImage;
    Color lightRed;
    Color blankRed;
    bool isWarning;


    private void Awake()
    {
        if (maxAlpha == 0f || maxAlpha < minAlpha)
        {
            Debug.Log("적절한 Alpha 값을 입력하세요.");
        }

        blankRed = new Color(0.8f, 0f, 0f, 0f);
        lightRed = new Color(0.8f, 0f, 0f, minAlpha);

        overlayImage = GetComponent<Image>();
        overlayImage.color = blankRed;

        isWarning = false;
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
            overlayImage.color = blankRed;
            AudioManager.instance.PauseBGM(false);
        }
        else if (GameManager.instance.health < 0.1f)
        {
            StopCoroutine("Warning");
            isWarning = false;
            overlayImage.color = blankRed;
        }
    }

    IEnumerator Warning()
    {
        float timer = 0f;
        isWarning = true;
        overlayImage.color = lightRed;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.HeartBeat);
        while (true)
        {
            yield return null;
            timer += Time.deltaTime;

            if (timer < .2f)
            {
                overlayImage.color += new Color(0, 0, 0, (maxAlpha - minAlpha) * Time.deltaTime / .2f);
            }
            else if (timer < .45f)
            {
                overlayImage.color -= new Color(0, 0, 0, (maxAlpha - minAlpha) * Time.deltaTime / .25f);
            }
            else if (timer > 1.3f)
            {
                timer = 0f;
                overlayImage.color = lightRed;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.HeartBeat);
            }
        }
    }

    public void WarningToTrue()
    {
        isWarning = true;
    }
}
