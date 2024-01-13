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

    GameObject selectedObject;
    int lastPressedId;

    private void Awake()
    {
        selectedObject = null;
    }

    private void Start()
    {
        if (confirmPanel.activeSelf) confirmPanel.SetActive(false);
        if (changePanel.activeSelf) changePanel.SetActive(false);
    }

    private void Update()
    {
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
        Player.instance.gameObject.SetActive(true);
    }

    public void CloseChangePanel()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
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
