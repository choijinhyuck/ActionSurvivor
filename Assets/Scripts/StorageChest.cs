using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StorageChest : MonoBehaviour
{
    public enum ActionType { Open, Inventory }

    public StorageUI storageUI;
    public LayerMask playerLayer;
    public Sprite[] equipSprites;
    

    [SerializeField]
    Image buttonImage;

    InputAction openAction;
    InputAction inventoryAction;
    InputAction cancelAction;
    InputAction menuAction;
    InputAction destroyAction;
    InputAction equipAction;
    Animator animator;
    bool isNear;
    bool isOpening;

    private void Start()
    {
        animator = GetComponent<Animator>();
        isOpening = false;

        isNear = false;
        openAction = GameManager.Instance.actions.FindActionMap("UI").FindAction("OpenChest");
        inventoryAction = GameManager.Instance.actions.FindActionMap("UI").FindAction("Inventory");
        cancelAction = GameManager.Instance.actions.FindActionMap("UI").FindAction("Cancel");
        menuAction = GameManager.Instance.actions.FindActionMap("UI").FindAction("Menu");
        destroyAction = GameManager.Instance.actions.FindActionMap("UI").FindAction("Destroy");
        equipAction = GameManager.Instance.actions.FindActionMap("UI").FindAction("Equip");

        openAction.performed += _ => Open(ActionType.Open);
        inventoryAction.performed += _ => Open(ActionType.Inventory);
        cancelAction.performed += _ => storageUI.OnCancel();
        menuAction.performed += _ => storageUI.OnMenu();
        destroyAction.performed += _ => storageUI.DestroyItem();
        equipAction.performed += _ => storageUI.OnStore();
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 1f, Vector2.up, .5f, playerLayer);

        if (hit.collider is null)
        {
            isNear = false;
            buttonImage.transform.parent.gameObject.SetActive(false);
            return;
        }

        isNear = true;

        switch (ControllerManager.instance.CurrentScheme)
        {
            case ControllerManager.scheme.Keyboard:
                buttonImage.sprite = equipSprites[0];
                buttonImage.transform.parent.gameObject.SetActive(true);
                break;

            case ControllerManager.scheme.Gamepad:
                buttonImage.sprite = equipSprites[1];
                buttonImage.transform.parent.gameObject.SetActive(true);
                break;
        }

    }

    public void Open(ActionType actionType)
    {
        if (!isNear) return;

        switch (actionType)
        {
            case ActionType.Open:
                if (!GameManager.Instance.workingInventory)
                {
                    if (isOpening) return;
                    isOpening = true;
                    animator.SetTrigger("Open");

                    //AudioManager.instance.PlaySfx(AudioManager.Sfx.ChestOpen);
                    //AudioManager.instance.EffectBgm(true);
                    //GameManager.Instance.workingInventory = true;
                    //storageUI.gameObject.SetActive(true);
                    //GameManager.Instance.Stop();
                }
                break;

            case ActionType.Inventory:
                if (!storageUI.gameObject.activeSelf) return;
                if (GameManager.Instance.workingInventory)
                {
                    AudioManager.instance.EffectBgm(false);
                    GameManager.Instance.workingInventory = false;
                    storageUI.gameObject.SetActive(false);
                    storageUI.destroyDesc.transform.parent.gameObject.SetActive(false);
                    GameManager.Instance.Resume();
                }
                break;
        }
    }

    public void OpenChest()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.ChestOpen);
        AudioManager.instance.EffectBgm(true);
        GameManager.Instance.workingInventory = true;
        storageUI.gameObject.SetActive(true);
        GameManager.Instance.Stop();

        isOpening = false;
    }
    
    public void CloseSound()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.ChestOpen);
    }
}
