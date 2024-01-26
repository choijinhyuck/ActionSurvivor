using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopNPC : MonoBehaviour
{
    public enum ActionType { Open, Inventory }

    public ShopUI shopUI;
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
        openAction = GameManager.instance.actions.FindActionMap("UI").FindAction("OpenChest");
        inventoryAction = GameManager.instance.actions.FindActionMap("UI").FindAction("Inventory");
        cancelAction = GameManager.instance.actions.FindActionMap("UI").FindAction("Cancel");
        menuAction = GameManager.instance.actions.FindActionMap("UI").FindAction("Menu");

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
        shopUI.OnCancel();
    }
    void MenuHandler(InputAction.CallbackContext context)
    {
        shopUI.OnMenu();
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
            Close();
            return;
        }

        isNear = true;

        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
        {
            buttonImage.transform.parent.GetComponentInChildren<Text>(true).text = "����";
        }
        else
        {
            buttonImage.transform.parent.GetComponentInChildren<Text>(true).text = "Shop";
        }

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
                if (!GameManager.instance.workingInventory)
                {
                    //AudioManager.instance.EffectBgm(true);
                    GameManager.instance.workingInventory = true;
                    shopUI.gameObject.SetActive(true);
                    //GameManager.instance.Stop();
                    GameManager.instance.isLive = false;
                }
                break;

            case ActionType.Inventory:
                Close();
                break;
        }
    }

    void Close()
    {
        if (!shopUI.gameObject.activeSelf) return;
        if (GameManager.instance.workingInventory)
        {
            //AudioManager.instance.EffectBgm(false);
            GameManager.instance.workingInventory = false;
            shopUI.gameObject.SetActive(false);
            shopUI.buySellConfirm.transform.parent.gameObject.SetActive(false);
            //GameManager.instance.Resume();
            GameManager.instance.isLive = true;
        }
    }
}
