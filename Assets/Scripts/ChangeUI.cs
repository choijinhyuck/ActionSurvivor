using System.Collections;
using System.Collections.Generic;
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
            warning.text = "현재 플레이 중인 캐릭터입니다.";
            if (isWarning) StopCoroutine(warningCoroutine);
            warningCoroutine = StartCoroutine(WarningCoroutine());
            return;
        }
        else if (playerId > GameManager.instance.newCharacterUnlock)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
            warning.text = "아직 합류하지 않은 캐릭터입니다.";
            if (isWarning) StopCoroutine(warningCoroutine);
            warningCoroutine = StartCoroutine(WarningCoroutine());
            return;
        }

        switch (playerId)
        {
            case 0:
                confirmPanel.GetComponentInChildren<Text>(true).text = "<color=blue>[워리어]</color>로\r\n교체하시겠습니까?";
                break;
            case 1:
                confirmPanel.GetComponentInChildren<Text>(true).text = "<color=blue>[야만전사]</color>로\r\n교체하시겠습니까?";
                break;
            case 2:
                confirmPanel.GetComponentInChildren<Text>(true).text = "<color=blue>[폭탄맨]</color>으로\r\n교체하시겠습니까?";
                break;
            default:
                Debug.Log($"알 수 없는 캐릭터 Id 입니다. player Id: {playerId}");
                confirmPanel.GetComponentInChildren<Text>(true).text = "<color=blue>[알 수 없음]</color>으로\r\n교체하시겠습니까?";
                break;
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
