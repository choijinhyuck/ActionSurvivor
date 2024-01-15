using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

public class MenuUI : MonoBehaviour
{
    public static MenuUI instance;
    [SerializeField] GameObject confirm;
    [SerializeField] GameObject helpPanel;
    [SerializeField] Sprite[] keySprites;
    [SerializeField] Image[] keyImages;
    [SerializeField] GameObject defaultHelpPanel;
    [SerializeField] GameObject changeHelpPanel;
    [SerializeField] GameObject rightArrow;
    [SerializeField] GameObject leftArrow;

    GameObject menuPanel;

    bool isUpgrading;
    bool isShopping;
    bool isStoring;
    bool isStageSelecting;
    bool isTutoriaring;

    List<Button> menuButtons;
    int selectedId;
    GameObject selectedObjectOnConfirm;
    Vector2 lastPressedMove;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        menuPanel = transform.GetChild(0).gameObject;
        menuButtons = GetComponentsInChildren<Button>(true).ToList();

        isUpgrading = false;
        isShopping = false;
        isStoring = false;
        isStageSelecting = false;
        isTutoriaring = false;

        selectedId = -1;
        selectedObjectOnConfirm = null;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (rightArrow.activeSelf) rightArrow.SetActive(false);
        if (leftArrow.activeSelf) leftArrow.SetActive(false);
        if (changeHelpPanel.activeSelf) changeHelpPanel.SetActive(false);
        if (!defaultHelpPanel.activeSelf) changeHelpPanel.SetActive(true);
        if (helpPanel.activeSelf) helpPanel.SetActive(false);
        if (confirm.activeSelf) confirm.SetActive(false);
        if (menuPanel.activeSelf) menuPanel.SetActive(false);

        isUpgrading = false;
        isShopping = false;
        isStoring = false;
        isStageSelecting = false;
        isTutoriaring = false;

        selectedId = -1;
        selectedObjectOnConfirm = null;

        lastPressedMove = Vector2.zero;
    }

    private void Update()
    {
        if (helpPanel.activeSelf)
        {
            switch (ControllerManager.instance.CurrentScheme)
            {
                case ControllerManager.scheme.Keyboard:
                    keyImages[0].sprite = keySprites[0];
                    keyImages[1].sprite = keySprites[1];
                    break;

                case ControllerManager.scheme.Gamepad:
                    keyImages[0].sprite = keySprites[2];
                    keyImages[1].sprite = keySprites[3];
                    break;
            }

            if (((InputSystemUIInputModule)EventSystem.current.currentInputModule).cancel.action.WasPerformedThisFrame())
            {
                HelpClose();
                return;
            }

            if (GameManager.instance.newCharacterUnlock > 0)
            {
                if (defaultHelpPanel.activeSelf)
                {
                    rightArrow.SetActive(true);
                    if (leftArrow.activeSelf) leftArrow.SetActive(false);
                }
                else if (changeHelpPanel.activeSelf)
                {
                    leftArrow.SetActive(true);
                    if (rightArrow.activeSelf) rightArrow.SetActive(false);
                }

                InputSystemUIInputModule input = (InputSystemUIInputModule)EventSystem.current.currentInputModule;

                if (input.move.action.ReadValue<Vector2>() != lastPressedMove)
                {
                    if (input.move.action.ReadValue<Vector2>().x > 0)
                    {
                        if (defaultHelpPanel.activeSelf)
                        {
                            defaultHelpPanel.SetActive(false);
                            changeHelpPanel.SetActive(true);
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonChange);
                        }
                    }
                    else if (input.move.action.ReadValue<Vector2>().x < 0)
                    {
                        if (changeHelpPanel.activeSelf)
                        {
                            defaultHelpPanel.SetActive(true);
                            changeHelpPanel.SetActive(false);
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonChange);
                        }
                    }
                    lastPressedMove = input.move.action.ReadValue<Vector2>();
                }
            }
        }
        else
        {
            lastPressedMove = Vector2.zero;
        }
        

        if (menuPanel.activeSelf)
        {
            if (SceneManager.GetActiveScene().name == "Camp" && GameManager.instance.newCharacterUnlock > 0)
            {
                menuButtons[2].enabled = true;
                menuButtons[2].GetComponent<Image>().color = Color.white;
                menuButtons[2].GetComponentInChildren<Text>().color = Color.black;
            }
            else
            {
                menuButtons[2].enabled = false;
                menuButtons[2].GetComponent<Image>().color = new(.85f, .85f, .85f, 1);
                menuButtons[2].GetComponentInChildren<Text>().color = new(0, 0, 0, .6f);
            }

            if (confirm.activeSelf)
            {
                if (EventSystem.current.currentInputModule != null)
                {
                    if (((InputSystemUIInputModule)EventSystem.current.currentInputModule).cancel.action.WasPerformedThisFrame())
                    {
                        if (!confirm.GetComponentInChildren<Button>().enabled) return;
                        ConfirmClose();
                    }
                }
                if (selectedObjectOnConfirm != EventSystem.current.currentSelectedGameObject)
                {
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonChange);
                    selectedObjectOnConfirm = EventSystem.current.currentSelectedGameObject;
                }
            }
            else
            {
                if (EventSystem.current.currentSelectedGameObject == null) return;
                if (EventSystem.current.currentSelectedGameObject != menuButtons[selectedId].gameObject)
                {
                    if (selectedId != -1) AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonChange);
                    selectedId = menuButtons.IndexOf(EventSystem.current.currentSelectedGameObject.GetComponent<Button>());
                }

            }
        }
    }

    // 인벤토리 창에서 아무 버튼도 선택되지 않은 경우에 Menu키 (키보드: Esc, 게임패드: Start)로 빠져나올 수 있도록.
    public void OnMenu()
    {
        if (isUpgrading || isShopping || isStoring || isStageSelecting || isTutoriaring) return;
        if (new List<string> { "Title", "Loading" }.Contains(SceneManager.GetActiveScene().name)) return;
        if (LevelUp.instance.isLevelUp) return;
        if (GameManager.instance.health < .1f) return;
        if (BaseUI.Instance.victory.gameObject.activeSelf) return;

        if (InventoryUI.instance == null || !InventoryUI.instance.gameObject.activeSelf)
        {
            Menu();
            return;
        }

        InventoryUI.instance.OnMenu();
    }

    public void Menu()
    {
        if (confirm.activeSelf)
        {
            if (!confirm.GetComponentInChildren<Button>().enabled) return;
            ConfirmClose();
        }
        else if (SettingUI.instance.settingPanel.activeSelf)
        {
            if (SettingUI.instance.DropdownOpened())
            {
                return;
            }
            else
            {
                SettingUI.instance.Back();
            }
        }
        else if (helpPanel.activeSelf)
        {
            HelpClose();
        }
        else if (FindAnyObjectByType<ChangeUI>() && FindAnyObjectByType<ChangeUI>().IsChangePanelActive())
        {
            ChangeUI changeUI = FindAnyObjectByType<ChangeUI>();
            if (changeUI.IsConfirmPanelActive())
            {
                changeUI.Confirm(false);
            }
            else
            {
                changeUI.CloseChangePanel();
            }
        }
        else if (!menuPanel.activeSelf)
        {
            GameManager.instance.workingInventory = true;
            AudioManager.instance.EffectBgm(true);
            GameManager.instance.Stop();
            menuPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(menuPanel.GetComponentsInChildren<Button>()[0].gameObject);
            selectedId = 0;
        }
        else
        {
            Close();
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
        }
    }

    public void Continue()
    {
        Close();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
    }

    public void Inventory()
    {
        Close();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
        GameManager.instance.OnInventory();
    }

    public void CharacterChange()
    {
        Close();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
        FindAnyObjectByType<ChangeUI>().OpenChangePanel();
    }

    public void Setting()
    {
        Close();
        GameManager.instance.workingInventory = true;
        GameManager.instance.Stop();

        AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
        SettingUI.instance.settingPanel.SetActive(true);
    }

    public void Help()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
        EventSystem.current.SetSelectedGameObject(null);
        helpPanel.SetActive(true);
    }

    void HelpClose()
    {
        if (rightArrow.activeSelf) rightArrow.SetActive(false);
        if (leftArrow.activeSelf) leftArrow.SetActive(false);
        if (changeHelpPanel.activeSelf) changeHelpPanel.SetActive(false);
        if (!defaultHelpPanel.activeSelf) defaultHelpPanel.SetActive(true);
        helpPanel.SetActive(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
        EventSystem.current.SetSelectedGameObject(menuButtons[selectedId].gameObject);
    }

    public void MainTitle()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
        confirm.SetActive(true);
        confirm.GetComponentInChildren<Text>().text = "메인 메뉴로 돌아가시겠습니까?\r\n\r\n<color=red><size=30>주의!\r\n저장은 야영지에서만 이루어집니다. (자동)</size></color>";
        confirm.GetComponentInChildren<Button>().enabled = true;
        EventSystem.current.SetSelectedGameObject(confirm.GetComponentsInChildren<Button>()[1].gameObject);
        selectedObjectOnConfirm = EventSystem.current.currentSelectedGameObject;
    }

    public void GameExit()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
        confirm.SetActive(true);
        confirm.GetComponentInChildren<Text>().text = "게임을 종료하시겠습니까?\r\n\r\n<color=red><size=30>주의!\r\n저장은 야영지에서만 이루어집니다. (자동)</size></color>";
        confirm.GetComponentInChildren<Button>().enabled = true;
        EventSystem.current.SetSelectedGameObject(confirm.GetComponentsInChildren<Button>()[1].gameObject);
        selectedObjectOnConfirm = EventSystem.current.currentSelectedGameObject;
    }

    void Close()
    {
        GameManager.instance.workingInventory = false;
        AudioManager.instance.EffectBgm(false);
        menuPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        selectedId = -1;
        GameManager.instance.Resume();
    }

    public void OnConfirm(bool yes)
    {
        if (yes)
        {
            confirm.GetComponentInChildren<Button>().enabled = false;
            EventSystem.current.SetSelectedGameObject(null);
            selectedObjectOnConfirm = null;
            if (selectedId == 5)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.MenuSelect);
                GameManager.instance.sceneName = "Title";
                GameManager.instance.FadeOut();
            }
            else if (selectedId == 6)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.MenuSelect);
                Application.Quit();
            }
        }
        else
        {
            ConfirmClose();
        }
    }

    void ConfirmClose()
    {
        if (confirm.activeSelf)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
            confirm.SetActive(false);
            selectedObjectOnConfirm = null;
            EventSystem.current.SetSelectedGameObject(menuButtons[selectedId].gameObject);
        }

    }

    private void LateUpdate()
    {
        if (FindAnyObjectByType<UpgradeUI>() == null)
        {
            isUpgrading = false;
        }
        else if (!FindAnyObjectByType<UpgradeUI>().gameObject.activeSelf)
        {
            isUpgrading = false;
        }
        else
        {
            isUpgrading = true;
        }

        if (FindAnyObjectByType<ShopUI>() == null)
        {
            isShopping = false;
        }
        else if (!FindAnyObjectByType<ShopUI>().gameObject.activeSelf)
        {
            isShopping = false;
        }
        else
        {
            isShopping = true;
        }

        if (FindAnyObjectByType<StorageUI>() == null)
        {
            isStoring = false;
        }
        else if (!FindAnyObjectByType<StorageUI>().gameObject.activeSelf)
        {
            isStoring = false;
        }
        else
        {
            isStoring = true;
        }

        if (FindAnyObjectByType<StageSelect>() == null)
        {
            isStageSelecting = false;
        }
        else if (!FindAnyObjectByType<StageSelect>().stageSelectPanel.activeSelf)
        {
            isStageSelecting = false;
        }
        else
        {
            isStageSelecting = true;
        }

        if (FindAnyObjectByType<TutorialUI>() == null)
        {
            isTutoriaring = false;
        }
        else
        {
            isTutoriaring = true;
        }
    }
}
