using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    int selectedId;
    Color originAlpha;
    Color blankAlpha;
    Color halfAlpha;
    Color grayColor;
    Color greenColor;
    Color redColor;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>(true).ToList<Button>();
        itemImages = new Image[buttons.Count];
        canvases = new Canvas[buttons.Count];
        originAlpha = new Color(1f, 1f, 1f, 1f);
        blankAlpha = new Color(1f, 1f, 1f, 0f);
        halfAlpha = new Color(1f, 1f, 1f, .5f);
        grayColor = new Color(.7f, .7f, .7f, 1f);
        greenColor = new Color(.6f, 1f, .6f, 1f);
        redColor = new Color(1f, .4f, .4f, 1f);

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
        selectedId = 0;
    }

    void OnEnable()
    {
        if (isPressed)
        {
            isPressed = false;
            selectedId = pressedId;
        }
        

        for (int i = 0; i < GameManager.Instance.maxInventory; i++)
        {
            buttons[i].gameObject.SetActive(true);
        }

        ChangeAlpha(originAlpha);
        foreach (var canvas in canvases)
        {
            canvas.sortingOrder = 1;
        }
        EventSystem.current.SetSelectedGameObject(buttons[selectedId].gameObject);
        currentSelect = buttons[selectedId].gameObject;
        canvases[selectedId].sortingOrder = 2;
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

        selectedId = buttons.IndexOf(currentSelect.GetComponent<Button>());

        Init();
        if (isPressed) return;

        if (selectedId < 24)
        {
            if (GameManager.Instance.inventoryItemsId[selectedId] == -1)
            {
                itemName.text = "";
                itemDesc.text = "";
            }
            else
            {
                itemName.text = ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemName;
                itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemDesc;
            }
        }
        else
        {
            switch (selectedId)
            {
                case 24:
                    if (GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId] == -1)
                    {
                        itemName.text = "";
                        itemDesc.text = "";
                    }
                    else
                    {
                        itemName.text = ItemManager.Instance.itemDataArr[GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId]].itemName;
                        itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId]].itemDesc;
                    }
                    break;
                case 25:
                    if (GameManager.Instance.necklaceItem[GameManager.Instance.playerId] == -1)
                    {
                        itemName.text = "";
                        itemDesc.text = "";
                    }
                    else
                    {
                        itemName.text = ItemManager.Instance.itemDataArr[GameManager.Instance.necklaceItem[GameManager.Instance.playerId]].itemName;
                        itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.Instance.necklaceItem[GameManager.Instance.playerId]].itemDesc;
                    }
                    break;
                case 26:
                    if (GameManager.Instance.shoesItem[GameManager.Instance.playerId] == -1)
                    {
                        itemName.text = "";
                        itemDesc.text = "";
                    }
                    else
                    {
                        itemName.text = ItemManager.Instance.itemDataArr[GameManager.Instance.shoesItem[GameManager.Instance.playerId]].itemName;
                        itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.Instance.shoesItem[GameManager.Instance.playerId]].itemDesc;
                    }
                    break;
                case 27:
                    if (GameManager.Instance.rangeWeaponItem == -1)
                    {
                        itemName.text = "";
                        itemDesc.text = "";
                    }
                    else
                    {
                        itemName.text = ItemManager.Instance.itemDataArr[GameManager.Instance.rangeWeaponItem].itemName;
                        itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.Instance.rangeWeaponItem].itemDesc;
                    }
                    break;
                case 28:
                    if (GameManager.Instance.magicItem == -1)
                    {
                        itemName.text = "";
                        itemDesc.text = "";
                    }
                    else
                    {
                        itemName.text = ItemManager.Instance.itemDataArr[GameManager.Instance.magicItem].itemName;
                        itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.Instance.magicItem].itemDesc;
                    }
                    break;
            }

        }
    }

    void Init()
    {
        for (int i = 0; i < GameManager.Instance.maxInventory; i++)
        {
            if (GameManager.Instance.inventoryItemsId[i] == -1)
            {
                emptySlot(i);
            }
            else
            {
                itemImages[i].sprite = ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[i]].itemIcon;
                slotAlpha(i);
            }
        }

        for (int i = 24; i < 29; i++)
        {
            int tempItemId = -1;
            switch (i)
            {
                case 24:
                    tempItemId = GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId];
                    break;
                case 25:
                    tempItemId = GameManager.Instance.necklaceItem[GameManager.Instance.playerId];
                    break;
                case 26:
                    tempItemId = GameManager.Instance.shoesItem[GameManager.Instance.playerId];
                    break;
                case 27:
                    tempItemId = GameManager.Instance.rangeWeaponItem;
                    break;
                case 28:
                    tempItemId = GameManager.Instance.magicItem;
                    break;
            }
            if (tempItemId == -1)
            {
                emptySlot(i);
            }
            else
            {
                itemImages[i].sprite = ItemManager.Instance.itemDataArr[tempItemId].itemIcon;
                slotAlpha(i);
            }
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            if (isPressed)
            {

                if (i == pressedId)
                {
                    buttons[i].GetComponent<Image>().color = grayColor;
                }
                else if (i == selectedId)
                {
                    if (pressedId < 24)
                    {
                        int tempPressedItemType = (int)ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[pressedId]].itemType;
                        switch (i)
                        {
                            case 24:
                                if (tempPressedItemType != 0)
                                {
                                    buttons[i].GetComponent<Image>().color = redColor;
                                }
                                else
                                {
                                    buttons[i].GetComponent<Image>().color = greenColor;
                                }
                                break;
                            case 25:
                                if (tempPressedItemType != 3)
                                {
                                    buttons[i].GetComponent<Image>().color = redColor;
                                }
                                else
                                {
                                    buttons[i].GetComponent<Image>().color = greenColor;
                                }
                                break;
                            case 26:
                                if (tempPressedItemType != 2)
                                {
                                    buttons[i].GetComponent<Image>().color = redColor;
                                }
                                else
                                {
                                    buttons[i].GetComponent<Image>().color = greenColor;
                                }
                                break;
                            case 27:
                                if (tempPressedItemType != 1)
                                {
                                    buttons[i].GetComponent<Image>().color = redColor;
                                }
                                else
                                {
                                    buttons[i].GetComponent<Image>().color = greenColor;
                                }
                                break;
                            case 28:
                                if (tempPressedItemType != 4)
                                {
                                    buttons[i].GetComponent<Image>().color = redColor;
                                }
                                else
                                {
                                    buttons[i].GetComponent<Image>().color = greenColor;
                                }
                                break;
                            default:
                                buttons[i].GetComponent<Image>().color = greenColor;
                                break;
                        }
                    }
                    switch (pressedId)
                    {
                        case 24:
                            switch (selectedId)
                            {
                                case 25:
                                case 26:
                                case 27:
                                case 28:
                                    buttons[i].GetComponent<Image>().color = redColor;
                                    break;
                                default:
                                    if (GameManager.Instance.inventoryItemsId[selectedId] == -1)
                                    {
                                        buttons[i].GetComponent<Image>().color = greenColor;
                                        break;
                                    }
                                    if ((int)ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemType == 0)
                                    {
                                        buttons[i].GetComponent<Image>().color = greenColor;
                                    }
                                    else
                                    {
                                        buttons[i].GetComponent<Image>().color = redColor;
                                    }
                                    break;
                            }
                            break;
                        case 25:
                            switch (selectedId)
                            {
                                case 24:
                                case 26:
                                case 27:
                                case 28:
                                    buttons[i].GetComponent<Image>().color = redColor;
                                    break;
                                default:
                                    if (GameManager.Instance.inventoryItemsId[selectedId] == -1)
                                    {
                                        buttons[i].GetComponent<Image>().color = greenColor;
                                        break;
                                    }
                                    if ((int)ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemType == 3)
                                    {
                                        buttons[i].GetComponent<Image>().color = greenColor;
                                    }
                                    else
                                    {
                                        buttons[i].GetComponent<Image>().color = redColor;
                                    }
                                    break;
                            }
                            break;
                        case 26:
                            switch (selectedId)
                            {
                                case 24:
                                case 25:
                                case 27:
                                case 28:
                                    buttons[i].GetComponent<Image>().color = redColor;
                                    break;
                                default:
                                    if (GameManager.Instance.inventoryItemsId[selectedId] == -1)
                                    {
                                        buttons[i].GetComponent<Image>().color = greenColor;
                                        break;
                                    }
                                    if ((int)ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemType == 2)
                                    {
                                        buttons[i].GetComponent<Image>().color = greenColor;
                                    }
                                    else
                                    {
                                        buttons[i].GetComponent<Image>().color = redColor;
                                    }
                                    break;
                            }
                            break;
                        case 27:
                            switch (selectedId)
                            {
                                case 24:
                                case 25:
                                case 26:
                                case 28:
                                    buttons[i].GetComponent<Image>().color = redColor;
                                    break;
                                default:
                                    if (GameManager.Instance.inventoryItemsId[selectedId] == -1)
                                    {
                                        buttons[i].GetComponent<Image>().color = greenColor;
                                        break;
                                    }
                                    if ((int)ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemType == 1)
                                    {
                                        buttons[i].GetComponent<Image>().color = greenColor;
                                    }
                                    else
                                    {
                                        buttons[i].GetComponent<Image>().color = redColor;
                                    }
                                    break;
                            }
                            break;
                        case 28:
                            switch (selectedId)
                            {
                                case 24:
                                case 25:
                                case 26:
                                case 27:
                                    buttons[i].GetComponent<Image>().color = redColor;
                                    break;
                                default:
                                    if (GameManager.Instance.inventoryItemsId[selectedId] == -1)
                                    {
                                        buttons[i].GetComponent<Image>().color = greenColor;
                                        break;
                                    }
                                    if ((int)ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemType == 4)
                                    {
                                        buttons[i].GetComponent<Image>().color = greenColor;
                                    }
                                    else
                                    {
                                        buttons[i].GetComponent<Image>().color = redColor;
                                    }
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    buttons[i].GetComponent<Image>().color = originAlpha;
                }
            }
            else
            {
                buttons[i].GetComponent<Image>().color = originAlpha;
            }
        }
    }

    void emptySlot(int id)
    {
        itemImages[id].sprite = null;
        itemImages[id].color = blankAlpha;
    }

    void slotAlpha(int id)
    {
        if (isPressed)
        {
            if (id == pressedId)
            {
                itemImages[id].color = originAlpha;
            }
            else
            {
                itemImages[id].color = halfAlpha;
            }
        }
        else
        {
            itemImages[id].color = originAlpha;
        }
    }

    void OnPress(int buttonIndex)
    {
        if (isPressed)
        {
            // �κ��丮 �� ��ǰ�� ���� ���¿���
            if (pressedId < 24)
            {
                // �κ��丮 �� ��ǰ�� ���� ���
                if (buttonIndex < 24)
                {
                    int tempItemId = GameManager.Instance.inventoryItemsId[pressedId];
                    GameManager.Instance.inventoryItemsId[pressedId] = GameManager.Instance.inventoryItemsId[buttonIndex];
                    GameManager.Instance.inventoryItemsId[buttonIndex] = tempItemId;
                    isPressed = false;
                    return;
                }
                // ���â ��ǰ�� ���� ���
                switch (buttonIndex)
                {
                    case 24:
                        if ((int)ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[pressedId]].itemType == 0)
                        {
                            var tempItemId = GameManager.Instance.inventoryItemsId[pressedId];
                            GameManager.Instance.inventoryItemsId[pressedId] = GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId];
                            GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId] = tempItemId;
                            isPressed = false;
                            GameManager.Instance.StatusUpdate();
                        }
                        else
                        {
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                        }
                        return;
                    case 25:
                        if ((int)ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[pressedId]].itemType == 3)
                        {
                            var tempItemId = GameManager.Instance.inventoryItemsId[pressedId];
                            GameManager.Instance.inventoryItemsId[pressedId] = GameManager.Instance.necklaceItem[GameManager.Instance.playerId];
                            GameManager.Instance.necklaceItem[GameManager.Instance.playerId] = tempItemId;
                            isPressed = false;
                            GameManager.Instance.StatusUpdate();
                        }
                        else
                        {
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                        }
                        return;
                    case 26:
                        if ((int)ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[pressedId]].itemType == 2)
                        {
                            var tempItemId = GameManager.Instance.inventoryItemsId[pressedId];
                            GameManager.Instance.inventoryItemsId[pressedId] = GameManager.Instance.shoesItem[GameManager.Instance.playerId];
                            GameManager.Instance.shoesItem[GameManager.Instance.playerId] = tempItemId;
                            isPressed = false;
                            GameManager.Instance.StatusUpdate();
                        }
                        else
                        {
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                        }
                        return;
                    case 27:
                        if ((int)ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[pressedId]].itemType == 1)
                        {
                            var tempItemId = GameManager.Instance.inventoryItemsId[pressedId];
                            GameManager.Instance.inventoryItemsId[pressedId] = GameManager.Instance.rangeWeaponItem;
                            GameManager.Instance.rangeWeaponItem = tempItemId;
                            isPressed = false;
                            GameManager.Instance.StatusUpdate();
                        }
                        else
                        {
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                        }
                        return;
                    case 28:
                        if ((int)ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[pressedId]].itemType == 4)
                        {
                            var tempItemId = GameManager.Instance.inventoryItemsId[pressedId];
                            GameManager.Instance.inventoryItemsId[pressedId] = GameManager.Instance.magicItem;
                            GameManager.Instance.magicItem = tempItemId;
                            isPressed = false;
                            GameManager.Instance.StatusUpdate();
                        }
                        else
                        {
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                        }
                        return;
                }
            }
            // ���â ��ǰ�� ���� ���¿���
            else
            {
                // ���â ��ǰ�� ���� ���
                if (buttonIndex > 23)
                {
                    if (buttonIndex == pressedId)
                    {
                        //nothing happened
                        isPressed = false;
                    }
                    else
                    {
                        //fail
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                    }
                    return;
                }
                // �κ��丮 �� ��ǰ�� ���� ���
                else
                {
                    switch (pressedId)
                    {
                        // �ֹ��Ⱑ ���� �ִ� ���
                        case 24:
                            // �κ��丮 �� �󽽷��� ���� ���
                            if (GameManager.Instance.inventoryItemsId[buttonIndex] == -1)
                            {
                                GameManager.Instance.inventoryItemsId[buttonIndex] = GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId];
                                GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId] = -1;
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                            }
                            // �κ��丮 �� ��ǰ�� Melee Ÿ���� ��� (���� �÷��̾� ������ ���� ��� ���� ���� �����ؾ���)
                            else if ((int)ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[buttonIndex]].itemType == 0)
                            {
                                int tempPressedId = GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId];
                                GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId] = GameManager.Instance.inventoryItemsId[buttonIndex];
                                GameManager.Instance.inventoryItemsId[buttonIndex] = tempPressedId;
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                            }
                            // �κ��丮 �� ��ǰ�� ��ġ�� �ٲ� �� ���� ���
                            else
                            {
                                //fail
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                            }
                            return;

                        // ����̰� ���� �ִ� ���
                        case 25:
                            // �κ��丮 �� �󽽷��� ���� ���
                            if (GameManager.Instance.inventoryItemsId[buttonIndex] == -1)
                            {
                                GameManager.Instance.inventoryItemsId[buttonIndex] = GameManager.Instance.necklaceItem[GameManager.Instance.playerId];
                                GameManager.Instance.necklaceItem[GameManager.Instance.playerId] = -1;
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                            }
                            // �κ��丮 �� ��ǰ�� Necklace Ÿ���� ��� (���� �÷��̾� ������ ���� ��� ���� ���� �����ؾ���)
                            else if ((int)ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[buttonIndex]].itemType == 3)
                            {
                                int tempPressedId = GameManager.Instance.necklaceItem[GameManager.Instance.playerId];
                                GameManager.Instance.necklaceItem[GameManager.Instance.playerId] = GameManager.Instance.inventoryItemsId[buttonIndex];
                                GameManager.Instance.inventoryItemsId[buttonIndex] = tempPressedId;
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                            }
                            // �κ��丮 �� ��ǰ�� ��ġ�� �ٲ� �� ���� ���
                            else
                            {
                                //fail
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                            }
                            return;

                        // �Ź��� ���� �ִ� ���
                        case 26:
                            // �κ��丮 �� �󽽷��� ���� ���
                            if (GameManager.Instance.inventoryItemsId[buttonIndex] == -1)
                            {
                                GameManager.Instance.inventoryItemsId[buttonIndex] = GameManager.Instance.shoesItem[GameManager.Instance.playerId];
                                GameManager.Instance.shoesItem[GameManager.Instance.playerId] = -1;
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                            }
                            // �κ��丮 �� ��ǰ�� Shoes Ÿ���� ��� (���� �÷��̾� ������ ���� ��� ���� ���� �����ؾ���)
                            else if ((int)ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[buttonIndex]].itemType == 2)
                            {
                                int tempPressedId = GameManager.Instance.shoesItem[GameManager.Instance.playerId];
                                GameManager.Instance.shoesItem[GameManager.Instance.playerId] = GameManager.Instance.inventoryItemsId[buttonIndex];
                                GameManager.Instance.inventoryItemsId[buttonIndex] = tempPressedId;
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                            }
                            // �κ��丮 �� ��ǰ�� ��ġ�� �ٲ� �� ���� ���
                            else
                            {
                                //fail
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                            }
                            return;

                        // ��ô���Ⱑ ���� �ִ� ���
                        case 27:
                            // �κ��丮 �� �󽽷��� ���� ���
                            if (GameManager.Instance.inventoryItemsId[buttonIndex] == -1)
                            {
                                GameManager.Instance.inventoryItemsId[buttonIndex] = GameManager.Instance.rangeWeaponItem;
                                GameManager.Instance.rangeWeaponItem = -1;
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                            }
                            // �κ��丮 �� ��ǰ�� Range Ÿ���� ���
                            else if ((int)ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[buttonIndex]].itemType == 1)
                            {
                                int tempPressedId = GameManager.Instance.rangeWeaponItem;
                                GameManager.Instance.rangeWeaponItem = GameManager.Instance.inventoryItemsId[buttonIndex];
                                GameManager.Instance.inventoryItemsId[buttonIndex] = tempPressedId;
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                            }
                            // �κ��丮 �� ��ǰ�� ��ġ�� �ٲ� �� ���� ���
                            else
                            {
                                //fail
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                            }
                            return;

                        // ����å�� ���� �ִ� ���
                        case 28:
                            // �κ��丮 �� �󽽷��� ���� ���
                            if (GameManager.Instance.inventoryItemsId[buttonIndex] == -1)
                            {
                                GameManager.Instance.inventoryItemsId[buttonIndex] = GameManager.Instance.magicItem;
                                GameManager.Instance.magicItem = -1;
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                            }
                            // �κ��丮 �� ��ǰ�� Magic Ÿ���� ���
                            else if ((int)ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[buttonIndex]].itemType == 4)
                            {
                                int tempPressedId = GameManager.Instance.magicItem;
                                GameManager.Instance.magicItem = GameManager.Instance.inventoryItemsId[buttonIndex];
                                GameManager.Instance.inventoryItemsId[buttonIndex] = tempPressedId;
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                            }
                            // �κ��丮 �� ��ǰ�� ��ġ�� �ٲ� �� ���� ���
                            else
                            {
                                //fail
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                            }
                            return;
                    }
                }
            }
        }

        if (buttonIndex > 23)
        {
            switch (buttonIndex)
            {
                case 24:
                    if (GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId] == -1) return;
                    break;
                case 25:
                    if (GameManager.Instance.necklaceItem[GameManager.Instance.playerId] == -1) return;
                    break;
                case 26:
                    if (GameManager.Instance.shoesItem[GameManager.Instance.playerId] == -1) return;
                    break;
                case 27:
                    if (GameManager.Instance.rangeWeaponItem == -1) return;
                    break;
                case 28:
                    if (GameManager.Instance.magicItem == -1) return;
                    break;
            }
        }
        else if (GameManager.Instance.inventoryItemsId[buttonIndex] == -1)
        {
            return;
        }

        pressedId = buttonIndex;
        isPressed = true;
        ChangeAlpha(halfAlpha);
        itemImages[buttonIndex].color = originAlpha;
        buttons[buttonIndex].GetComponent<Image>().color = grayColor;
    }

    // �κ��丮 â���� �ƹ� ��ư�� ���õ��� ���� ��쿡 MenuŰ (Ű����: Esc, �����е�: Start)�� �������� �� �ֵ���.
    public void OnMenu()
    {
        if (GameManager.Instance.workingInventory)
        {
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
            EventSystem.current.SetSelectedGameObject(buttons[pressedId].gameObject);
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
