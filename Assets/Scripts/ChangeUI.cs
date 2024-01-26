using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class ChangeUI : MonoBehaviour
{
    [SerializeField] GameObject confirmPanel;
    [SerializeField] GameObject changePanel;
    [SerializeField] Text warning;
    [SerializeField] Sprite[] controlIcons;
    [SerializeField] GameObject help;

    Coroutine warningCoroutine;
    GameObject selectedObject;
    int lastPressedId;
    bool isWarning;

    private void Awake()
    {
        selectedObject = null;
        isWarning = false;
        warningCoroutine = null;
    }

    private void Start()
    {
        if (!help.activeSelf) help.SetActive(true);
        if (warning.gameObject.activeSelf) warning.gameObject.SetActive(false);
        if (confirmPanel.activeSelf) confirmPanel.SetActive(false);
        if (changePanel.activeSelf) changePanel.SetActive(false);
    }

    private void Update()
    {
        if (help.activeInHierarchy)
        {
            switch (ControllerManager.instance.CurrentScheme)
            {
                case ControllerManager.scheme.Keyboard:
                    help.GetComponentsInChildren<Image>()[0].sprite = controlIcons[0];
                    help.GetComponentsInChildren<Image>()[1].sprite = controlIcons[1];
                    break;
                case ControllerManager.scheme.Gamepad:
                    help.GetComponentsInChildren<Image>()[0].sprite = controlIcons[2];
                    help.GetComponentsInChildren<Image>()[1].sprite = controlIcons[3];
                    break;
            }
        }

        if (confirmPanel.activeSelf)
        {
            if (((InputSystemUIInputModule)EventSystem.current.currentInputModule).cancel.action.WasPerformedThisFrame())
            {
                Confirm(false);
                return;
            }
        }

        if (changePanel.activeSelf)
        {
            if (((InputSystemUIInputModule)EventSystem.current.currentInputModule).cancel.action.WasPerformedThisFrame())
            {
                CloseChangePanel();
                return;
            }

            if (selectedObject == null)
            {
                selectedObject = EventSystem.current.currentSelectedGameObject;
            }
            else
            {
                if (selectedObject != EventSystem.current.currentSelectedGameObject)
                {
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonChange);
                    selectedObject = EventSystem.current.currentSelectedGameObject;
                }
            }
        }
    }

    public void OnClick(int playerId)
    {
        if (GameManager.instance.playerId == playerId)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
            if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
            {
                warning.text = "현재 플레이 중인 캐릭터입니다.";
            }
            else
            {
                warning.text = "Currently playing the Character.";
            }
            
            if (isWarning) StopCoroutine(warningCoroutine);
            warningCoroutine = StartCoroutine(WarningCoroutine());
            return;
        }
        else if (playerId > GameManager.instance.newCharacterUnlock)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
            if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
            {
                warning.text = "아직 합류하지 않은 캐릭터입니다.";
            }
            else
            {
                warning.text = "The Character has not joined yet.";
            }
                
            if (isWarning) StopCoroutine(warningCoroutine);
            warningCoroutine = StartCoroutine(WarningCoroutine());
            return;
        }

        string characterName;
        switch (playerId)
        {
            case 0:
                characterName = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "워리어" : "Warrior";
                break;
            case 1:
                characterName = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "야만전사" : "Barbarian";
                break;
            case 2:
                characterName = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "폭탄맨" : "BombGuy";
                break;
            default:
                Debug.Log($"알 수 없는 캐릭터 Id 입니다. player Id: {playerId}");
                characterName = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "알 수 없음" : "Unknown";
                break;
        }
        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
        {
            confirmPanel.GetComponentInChildren<Text>(true).text = $"<color=blue>[{characterName}]</color>(으)로\r\n교체하시겠습니까?";
        }
        else
        {
            confirmPanel.GetComponentInChildren<Text>(true).text = $"Change your character to\r\n<color=blue>[{characterName}]</color>?";
        }
        
        confirmPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(confirmPanel.GetComponentsInChildren<Button>(true)[1].gameObject);
        selectedObject = EventSystem.current.currentSelectedGameObject;
        lastPressedId = playerId;
    }

    IEnumerator WarningCoroutine()
    {
        isWarning = true;
        warning.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);
        warning.gameObject.SetActive(false);
        isWarning = false;
        warningCoroutine = null;
    }

    public void Confirm(bool positive)
    {
        if (positive)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
            GameManager.instance.playerId = lastPressedId;
            confirmPanel.SetActive(false);
            changePanel.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            selectedObject = null;
            GameManager.instance.Resume();
            AudioManager.instance.EffectBgm(false);
            GameManager.instance.workingInventory = false;
            StartCoroutine(ChangeCoroutine());
        }
        else
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
            confirmPanel.SetActive(false);
            selectedObject = changePanel.GetComponentsInChildren<Button>(true)[lastPressedId].gameObject;
            EventSystem.current.SetSelectedGameObject(selectedObject);
        }
    }

    IEnumerator ChangeCoroutine()
    {
        Player.instance.gameObject.SetActive(false);
        yield return null;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.CharacterChange);
        Player.instance.gameObject.SetActive(true);
    }

    public void CloseChangePanel()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
        if (isWarning) StopCoroutine(warningCoroutine);
        if (warning.gameObject.activeSelf) warning.gameObject.SetActive(false);
        if (confirmPanel.activeSelf) confirmPanel.SetActive(false);
        changePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        selectedObject = null;
        GameManager.instance.Resume();
        AudioManager.instance.EffectBgm(false);
        GameManager.instance.workingInventory = false;
    }

    public void OpenChangePanel()
    {
        if (GameManager.instance.newCharacterUnlock < 1) return;
        GameManager.instance.workingInventory = true;
        AudioManager.instance.EffectBgm(true);

        var texts = GetComponentsInChildren<Text>(true);
        string[] words = new string[7];
        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
        {
            words[0] = "워리어";
            words[1] = "야만전사";
            words[2] = "선택";
            words[3] = "닫기";
            words[4] = "캐릭터 교체";
            words[5] = "예";
            words[6] = "아니요";
        }
        else
        {
            words[0] = "Warrior";
            words[1] = "Barbarian";
            words[2] = "Select";
            words[3] = "Close";
            words[4] = "Switch Character";
            words[5] = "Yes";
            words[6] = "No";
        }
        foreach (var text in texts)
        {
            if (text.name == "Player0 Name") text.text = words[0];
            if (text.name == "Player1 Name") text.text = words[1];
            if (text.name == "Select Text") text.text = words[2];
            if (text.name == "Close Text") text.text = words[3];
            if (text.name == "Change Title") text.text = words[4];
            if (text.name == "Yes Label") text.text = words[5];
            if (text.name == "No Label") text.text = words[6];
        }

        changePanel.SetActive(true);
        selectedObject = changePanel.GetComponentsInChildren<Button>(true)[GameManager.instance.playerId].gameObject;
        EventSystem.current.SetSelectedGameObject(selectedObject);
        GameManager.instance.Stop();
    }

    public bool IsChangePanelActive()
    {
        return changePanel.activeSelf;
    }

    public bool IsConfirmPanelActive()
    {
        return confirmPanel.activeSelf;
    }
}
