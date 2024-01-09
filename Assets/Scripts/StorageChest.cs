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
    bool isClosing;

    private void Start()
    {
        animator = GetComponent<Animator>();
        isClosing = false;

        isNear = false;
        openAction = GameManager.instance.actions.FindActionMap("UI").FindAction("OpenChest");
        inventoryAction = GameManager.instance.actions.FindActionMap("UI").FindAction("Inventory");
        cancelAction = GameManager.instance.actions.FindActionMap("UI").FindAction("Cancel");
        menuAction = GameManager.instance.actions.FindActionMap("UI").FindAction("Menu");
        destroyAction = GameManager.instance.actions.FindActionMap("UI").FindAction("Destroy");
        equipAction = GameManager.instance.actions.FindActionMap("UI").FindAction("Equip");

        openAction.performed += OpenHandler;
        inventoryAction.performed += InventoryHandler;
        cancelAction.performed += CancelHandler;
        menuAction.performed += MenuHandler;
        destroyAction.performed += DestroyHandler;
        equipAction.performed += EquipHandler;
    }

    void OpenHandler(InputAction.CallbackContext context)
    {
        Open(ActionType.Open);
    }
    void InventoryHandler(InputAction.CallbackContext context)
    {
        Open(ActionType.Inventory);
    }
    void CancelHandler(InputAction.CallbackContext context)
    {
        storageUI.OnCancel();
    }
    void MenuHandler(InputAction.CallbackContext context)
    {
        storageUI.OnMenu();
    }
    void DestroyHandler(InputAction.CallbackContext context)
    {
        storageUI.DestroyItem();
    }
    void EquipHandler(InputAction.CallbackContext context)
    {
        storageUI.OnStore();
    }

    private void OnDestroy()
    {
        openAction.performed -= OpenHandler;
        inventoryAction.performed -= InventoryHandler;
        cancelAction.performed -= CancelHandler;
        menuAction.performed -= MenuHandler;
        destroyAction.performed -= DestroyHandler;
        equipAction.performed -= EquipHandler;
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 1f, Vector2.up, .5f, playerLayer);

        if (hit.collider is null)
        {
            isNear = false;
            buttonImage.transform.parent.gameObject.SetActive(false);
            Close();
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
        if (isClosing) return;

        switch (actionType)
        {
            case ActionType.Open:
                if (!GameManager.instance.workingInventory)
                {
                    animator.SetTrigger("Open");
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.ChestOpen);
                    //AudioManager.instance.EffectBgm(true);
                    GameManager.instance.workingInventory = true;
                    storageUI.gameObject.SetActive(true);
                    //GameManager.instance.Stop();
                    GameManager.instance.isLive = false;
                }
                break;

            case ActionType.Inventory:
                Close();
                break;
        }
    }
    
    public void CloseSound()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.ChestOpen);
    }

    public void FinishClose()
    {
        isClosing = false;
    }

    void Close()
    {
        if (!storageUI.gameObject.activeSelf) return;
        if (GameManager.instance.workingInventory)
        {
            isClosing = true;
            animator.SetTrigger("Close");
            //AudioManager.instance.EffectBgm(false);
            GameManager.instance.workingInventory = false;
            storageUI.gameObject.SetActive(false);
            storageUI.destroyDesc.transform.parent.gameObject.SetActive(false);
            //GameManager.instance.Resume();
            GameManager.instance.isLive = true;
        }
    }
}
