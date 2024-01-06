using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UpgradeNPC : MonoBehaviour
{
    public enum ActionType { Open, Inventory }

    public UpgradeUI upgradeUI;
    public LayerMask playerLayer;
    public Sprite[] equipSprites;

    [SerializeField]
    Image buttonImage;

    InputAction openAction;
    InputAction inventoryAction;
    InputAction cancelAction;
    InputAction menuAction;
    bool isNear;

    private void Start()
    {
        isNear = false;
        openAction = GameManager.Instance.actions.FindActionMap("UI").FindAction("OpenChest");
        inventoryAction = GameManager.Instance.actions.FindActionMap("UI").FindAction("Inventory");
        cancelAction = GameManager.Instance.actions.FindActionMap("UI").FindAction("Cancel");
        menuAction = GameManager.Instance.actions.FindActionMap("UI").FindAction("Menu");

        openAction.performed += OpenHandler;
        inventoryAction.performed += InventoryHandler;
        cancelAction.performed += CancelHandler;
        menuAction.performed += MenuHandler;
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
        upgradeUI.OnCancel();
    }
    void MenuHandler(InputAction.CallbackContext context)
    {
        upgradeUI.OnMenu();
    }


    private void OnDestroy()
    {
        openAction.performed -= OpenHandler;
        inventoryAction.performed -= InventoryHandler;
        cancelAction.performed -= CancelHandler;
        menuAction.performed -= MenuHandler;
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
                    AudioManager.instance.EffectBgm(true);
                    GameManager.Instance.workingInventory = true;
                    upgradeUI.gameObject.SetActive(true);
                    GameManager.Instance.Stop();
                }
                break;

            case ActionType.Inventory:
                if (!upgradeUI.gameObject.activeSelf) return;
                if (GameManager.Instance.workingInventory)
                {
                    upgradeUI.hammering.SetActive(false);
                    upgradeUI.upgradeResult.SetActive(false);
                    AudioManager.instance.EffectBgm(false);
                    GameManager.Instance.workingInventory = false;
                    upgradeUI.gameObject.SetActive(false);
                    upgradeUI.upgradeConfirm.transform.parent.gameObject.SetActive(false);
                    GameManager.Instance.Resume();
                }
                break;
        }
    }
}
