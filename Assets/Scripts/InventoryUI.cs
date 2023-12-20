using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Canvas baseUI;
    public Text itemName;
    public Text itemDesc;

    List<Button> buttons;
    Canvas[] canvases;
    Image[] itemImages;
    GameObject currentSelect;
    bool isPressed;
    int pressedId;
    Color originAlpha;
    Color blankAlpha;
    Color halfAlpha;

    bool isFirstStart;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>(true).ToList<Button>();
        itemImages = new Image[buttons.Count];
        canvases = new Canvas[buttons.Count];
        originAlpha = new Color(1f, 1f, 1f, 1f);
        blankAlpha = new Color(1f, 1f, 1f, 0f);
        halfAlpha = new Color(1f, 1f, 1f, .5f);

        // buttons 0-23: Inventory, 24-26: Equipment, 27:Range, 28:Magic
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i > 23)
            {
                buttons[i].gameObject.SetActive(true);
                continue;
            }
            buttons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            canvases[i] = buttons[i].GetComponentInParent<Canvas>();
            itemImages[i] = buttons[i].GetComponentsInChildren<Image>()[1];
            int temp = i;
            buttons[i].onClick.AddListener(() => OnPress(temp));
        }
    }

    void OnEnable()
    {
        isPressed = false;

        for (int i = 0; i < GameManager.Instance.maxInventory; i++)
        {
            buttons[i].gameObject.SetActive(true);
        }
        
        ChangeAlpha(originAlpha);
        foreach (var canvas in canvases)
        {
            canvas.sortingOrder = 1;
        }
        EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
        currentSelect = buttons[0].gameObject;
        canvases[0].sortingOrder = 2;
        baseUI.sortingOrder = 0;

        Init();
    }

    private void LateUpdate()
    {
        currentSelect.GetComponentInParent<Canvas>().sortingOrder = 1;
        currentSelect = EventSystem.current.currentSelectedGameObject;

        if (currentSelect is null) return;
        currentSelect.GetComponentInParent<Canvas>().sortingOrder = 2;
        baseUI.sortingOrder = 0;

        if (isPressed) return;
        Init();

        int selectedId = buttons.IndexOf(currentSelect.GetComponent<Button>());
        if (GameManager.Instance.inventoryItemsId[selectedId] == -1)
        {
            itemName.text = "";
            itemDesc.text = "";
        }
        else {
            itemName.text = ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemName;
            itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemDesc;
        }

        
    }

    void Init()
    {
        for (int i = 0; i < GameManager.Instance.maxInventory; i++)
        {
            if (GameManager.Instance.inventoryItemsId[i] == -1)
            {
                itemImages[i].sprite = null;
                itemImages[i].color = blankAlpha;
            }
            else
            {
                itemImages[i].sprite = ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[i]].itemIcon;
                itemImages[i].color = originAlpha;
            }
        }
        
        foreach (var button in buttons)
        {
            button.GetComponent<Image>().color = originAlpha;
        }
    }
    void OnPress(int buttonIndex)
    {
        if (isPressed)
        {
            var tempItemId = GameManager.Instance.inventoryItemsId[pressedId];
            GameManager.Instance.inventoryItemsId[pressedId] = GameManager.Instance.inventoryItemsId[buttonIndex];
            GameManager.Instance.inventoryItemsId[buttonIndex] = tempItemId;
            isPressed = false;
            return;
        }
        if (GameManager.Instance.inventoryItemsId[buttonIndex] == -1 )
        {
            return;
        }
        pressedId = buttonIndex;
        isPressed = true;
        ChangeAlpha(halfAlpha);
        itemImages[buttonIndex].color = originAlpha;
        buttons[buttonIndex].GetComponent<Image>().color = new Color(.7f, .7f, .7f, 1f);
    }

    // 인벤토리 창에서 아무 버튼도 선택되지 않은 경우에 Menu키 (키보드: Esc, 게임패드: Start)로 빠져나올 수 있도록.
    public void OnMenu()
    { 
        if (GameManager.Instance.workingInventory)
        {
            Debug.Log(isPressed);
            if (!isPressed)
            {
                GameManager.Instance.OnInventory();
            }
            else
            {
                isPressed = false;
            }
        }
    }

    public void OnCancel()
    {
        if (GameManager.Instance.workingInventory && isPressed)
        {
            isPressed = false;
        }
    }

    void ChangeAlpha(Color targetColor)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i < 24 && GameManager.Instance.inventoryItemsId[i] == -1)
            {
                itemImages[i].color = blankAlpha;
                continue;
            }
            itemImages[i].color = targetColor;
        }
    }
}
