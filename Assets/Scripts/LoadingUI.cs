using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] RectTransform movingGuy;
    [SerializeField] Slider loadingBar;
    [SerializeField] Text isLoading;
    Text loadingPercentage;

    float leftEnd;
    float rightEnd;
    float posY;
    string[] loadingText;

    private void Awake()
    {
        leftEnd = 0f;
        rightEnd = 1740f;
        posY = 90f;
        loadingText = new string[4]
        {
            "로딩 중",
            "로딩 중.",
            "로딩 중..",
            "로딩 중..."
        };
        loadingPercentage = movingGuy.GetComponentInChildren<Text>();

    }

    private void Start()
    {
        if (InventoryUI.instance is not null && InventoryUI.instance.gameObject.activeSelf) InventoryUI.instance.gameObject.SetActive(false);

        movingGuy.anchoredPosition = new Vector2(leftEnd, posY);
        loadingBar.value = 0f;
        StartCoroutine(Loading());
        isLoading.text = loadingText[0];
        loadingPercentage.text = string.Format("{0}%", 0);

        GameManager.Instance.player.gameObject.SetActive(false);
    }

    IEnumerator Loading()
    {
        AsyncOperation scene = SceneManager.LoadSceneAsync(GameManager.Instance.sceneName);
        scene.allowSceneActivation = false;
        float timer = 0f;
        float textTimer = 0f;
        int textIndex = 0;
        float textInterval = .1f;

        while (timer < 3f)
        {
            if (textTimer > textInterval)
            {
                textTimer = 0f;
                textIndex++;
            }
            isLoading.text = loadingText[textIndex % 4];
            movingGuy.anchoredPosition = new Vector2(Mathf.Min(scene.progress * 10 / .9f, timer / 3) * (rightEnd - leftEnd) + leftEnd, posY);
            loadingBar.value = Mathf.Min(scene.progress * 10 / .9f, timer / 3);
            loadingPercentage.text = string.Format("{0}%", Mathf.FloorToInt(loadingBar.value * 100));
            yield return null;
            timer += Time.unscaledDeltaTime;
            textTimer += Time.unscaledDeltaTime;
        }
        scene.allowSceneActivation = true;
        while (!scene.isDone)
        {
            if (textTimer > textInterval)
            {
                textTimer = 0f;
                textIndex++;
            }
            isLoading.text = loadingText[textIndex % 4];
            movingGuy.anchoredPosition = new Vector2(scene.progress * 10 / .9f * (rightEnd - leftEnd) + leftEnd, posY);
            loadingBar.value = scene.progress * 10 / .9f;
            loadingPercentage.text = string.Format("{0}%", Mathf.FloorToInt(loadingBar.value * 100));
            yield return null;
            textTimer += Time.unscaledDeltaTime;
        }
    }
}
