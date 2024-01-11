using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackgroundUI : MonoBehaviour
{
    [SerializeField] InputActionAsset actions;
    [SerializeField] GameObject confirm;

    InputAction cancelAction;
    InputAction menuAction;
    Vector2 newGameSize;
    Vector2 exitSize;
    int buttonClickedIndex;
    int selectedId;
    List<Button> buttons;
    

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>(true).ToList<Button>();
        selectedId = 0;
        buttonClickedIndex = -1;

        newGameSize = new(590, 315);
        exitSize = new(590, 230);

        cancelAction = actions.FindActionMap("UI").FindAction("Cancel");
        menuAction = actions.FindActionMap("UI").FindAction("Menu");

        cancelAction.performed += CancelHandler;
        menuAction.performed += CancelHandler;
    }

    void CancelHandler(InputAction.CallbackContext context)
    {
        if (confirm.activeSelf)
        {
            if (!buttons[selectedId].enabled) return;
            Confirm(false);
        }
        
        if (SettingUI.instance.settingPanel.activeSelf)
        {
            SettingUI.instance.Back();
        }
    }

    private void OnDestroy()
    {
        cancelAction.performed -= CancelHandler;
        menuAction.performed -= CancelHandler;
    }

    private void Start()
    {
        if (InventoryUI.instance != null && InventoryUI.instance.gameObject.activeSelf) InventoryUI.instance.gameObject.SetActive(false);

        buttons[1].interactable = PlayerPrefs.HasKey("maxInventory");
        EventSystem.current.SetSelectedGameObject(buttons[selectedId].gameObject);
    }

    private void Update()
    {

        if (SettingUI.instance.settingPanel.activeSelf) return;

        if (buttonClickedIndex != -1 && !confirm.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(buttons[selectedId].gameObject);
        }

        GameObject selectedButton = EventSystem.current.currentSelectedGameObject;
        if (selectedButton == null) return;
        if (buttons[selectedId].gameObject != selectedButton)
        {
            int nextId = buttons.IndexOf(selectedButton.GetComponent<Button>());

            if ((selectedId < 5 && nextId < 5) || (selectedId > 4 && nextId > 4))
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.MenuChange);

            }

            selectedId = nextId;
        }



        for (int i = 0; buttons.Count > i; i++)
        {
            if (i > 4) continue;
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
        if (buttonClickedIndex == 0) return;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.MenuSelect);
        buttonClickedIndex = 0;
        if (!PlayerPrefs.HasKey("maxInventory"))
        {
            StartCoroutine(Press());

            AudioManager.instance.PlaySfx(AudioManager.Sfx.MenuSelect);
            SaveManager.ResetSave();
            GameManager.instance.InfoInit();
            GameManager.instance.sceneName = "Stage_0";
            GameManager.instance.FadeOut();
        }
        else
        {
            confirm.GetComponent<RectTransform>().sizeDelta = newGameSize;
            confirm.GetComponentInChildren<Text>().text = "정말로 새 게임을 시작하시겠습니까?\n<color=red>(저장된 데이터가 모두 삭제됩니다.)</color>";
            confirm.SetActive(true);
            // 아니오 선택
            EventSystem.current.SetSelectedGameObject(confirm.GetComponentsInChildren<Button>()[1].gameObject);
        }
    }

    public void Confirm(bool positive)
    {
        if (positive)
        {
            if (buttonClickedIndex == 0)
            {
                buttons[selectedId].enabled = false;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.MenuSelect);

                SaveManager.ResetSave();
                GameManager.instance.InfoInit();

                GameManager.instance.sceneName = "Stage_0";
                GameManager.instance.FadeOut();

            }
            else if (buttonClickedIndex == 4)
            {
                buttons[selectedId].enabled = false;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.MenuSelect);
                Debug.Log("Exit");
                Application.Quit();
            }
        }
        else
        {
            confirm.SetActive(false);
            AudioManager.instance.PlaySfx(AudioManager.Sfx.MenuSelect);
            EventSystem.current.SetSelectedGameObject(buttons[buttonClickedIndex].gameObject);
            buttonClickedIndex = -1;
        }
    }

    public void ContinueGame()
    {
        if (buttonClickedIndex == 1) return;
        buttonClickedIndex = 1;
        StartCoroutine(Press());

        AudioManager.instance.PlaySfx(AudioManager.Sfx.MenuSelect);

        SaveManager.Load();

        GameManager.instance.sceneName = "Camp";
        GameManager.instance.FadeOut();
    }

    public void Setting()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.MenuSelect);
        SettingUI.instance.settingPanel.SetActive(true);
    }

    IEnumerator Press()
    {
        yield return null;
        buttons[selectedId].GetComponent<Animator>().SetBool("PressedByScript", false);
    }

    public void Exit()
    {
        if (buttonClickedIndex == 4) return;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.MenuSelect);
        buttonClickedIndex = 4;
        confirm.GetComponent<RectTransform>().sizeDelta = exitSize;
        confirm.GetComponentInChildren<Text>().text = "정말로 게임을 종료하시겠습니까?";
        confirm.SetActive(true);
        // 아니오 선택
        EventSystem.current.SetSelectedGameObject(confirm.GetComponentsInChildren<Button>()[1].gameObject);
    }
}
