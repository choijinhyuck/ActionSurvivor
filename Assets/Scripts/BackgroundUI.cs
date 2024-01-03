using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackgroundUI : MonoBehaviour
{
    bool isNewGame;
    int selectedId;
    List<Button> buttons;
    [SerializeField] Image fadeOut;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>().ToList<Button>();
        selectedId = 0;
        isNewGame = false;
    }

    private void Start()
    {
        buttons[1].interactable = PlayerPrefs.HasKey("maxInventory") ? true : false;
        EventSystem.current.SetSelectedGameObject(buttons[selectedId].gameObject);
    }

    private void Update()
    {
        if (isNewGame)
        {
            EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
        }

        GameObject selectedButton = EventSystem.current.currentSelectedGameObject;
        if (selectedButton is null) return;

        if (buttons[selectedId].gameObject != selectedButton)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.MenuChange);
            selectedId = buttons.IndexOf(selectedButton.GetComponent<Button>());
        }

        

        for (int i = 0; buttons.Count > i; i++)
        {
            string replaceText = "";
            if (selectedButton == buttons[i].gameObject)
            {
                switch (i)
                {
                    case 0:
                        replaceText = "> 새 게임 <";
                        break;

                    case 1:
                        replaceText = "> 이어하기 <";
                        break;

                    case 2:
                        replaceText = "> 설정 <";
                        break;

                    case 3:
                        replaceText = "> 크레딧 <";
                        break;

                    case 4:
                        replaceText = "> 종료 <";
                        break;
                }
            }
            else
            {
                switch (i)
                {
                    case 0:
                        replaceText = "새 게임";
                        break;

                    case 1:
                        replaceText = "이어하기";
                        break;

                    case 2:
                        replaceText = "설정";
                        break;

                    case 3:
                        replaceText = "크레딧";
                        break;

                    case 4:
                        replaceText = "종료";
                        break;
                }
            }

            buttons[i].GetComponent<Text>().text = replaceText;
        }
    }

    public void NewGame()
    {
        if (isNewGame) return;
        isNewGame = true;
        buttons[0].GetComponent<Animator>().SetBool("PressedByScript", false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Start);
        GameManager.Instance.sceneName = "Camp";
        StartCoroutine(ToLoading());
    }

    IEnumerator ToLoading()
    {
        Color color = new Color(0f, 0f, 0f, 0f);
        float timer = 0f;
        while (timer < 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            color.a = timer;
            fadeOut.color = color;
        }
        SceneManager.LoadScene("Loading");
    }

    public void Exit()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Start);
        Debug.Log("Exit");
        Application.Quit();
    }
}
