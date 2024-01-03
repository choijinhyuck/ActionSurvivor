using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Search;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Canvas baseUI;
    public Text itemName;
    public Text itemDesc;
    public Text itemEffect;
    public Text destroyDesc;
    public Button confirmNo;
    public InventoryControlHelp help;
    
    List<Button> buttons;
    Canvas[] canvases;
    Image[] itemImages;
    GameObject currentSelect;
    bool isDestroying;
    bool isPressed;
    int pressedId;
    int selectedId;
    GameObject selectedObejctOnDestroy;
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
        isDestroying = false;

        if (destroyDesc.transform.parent.gameObject.activeSelf)
        {
            destroyDesc.transform.parent.gameObject.SetActive(false);
        }
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
        
        if (isDestroying)
        {
            if (selectedObejctOnDestroy != EventSystem.current.currentSelectedGameObject)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonChange);
                selectedObejctOnDestroy = EventSystem.current.currentSelectedGameObject;
            }
            //  destroy canvas sortig order =3 으로
            destroyDesc.transform.parent.GetComponent<Canvas>().sortingOrder = 3;
            baseUI.sortingOrder = 0;
            return;
        }

        if (buttons[selectedId].gameObject != EventSystem.current.currentSelectedGameObject)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonChange);
        }

        currentSelect.GetComponentInParent<Canvas>().sortingOrder = 1;
        currentSelect = EventSystem.current.currentSelectedGameObject;

        if (currentSelect is null) return;
        currentSelect.GetComponentInParent<Canvas>().sortingOrder = 2;
        baseUI.sortingOrder = 0;
                
        selectedId = buttons.IndexOf(currentSelect.GetComponent<Button>());

        ShowHelp();
        
        Init();
        if (isPressed) return;

        if (selectedId < 24)
        {
            if (GameManager.Instance.inventoryItemsId[selectedId] == -1)
            {
                itemName.text = "";
                itemDesc.text = "";
                itemEffect.text = "";
            }
            else
            {
                itemName.text = ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemName;
                itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemDesc;
                itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemEffect;
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
                        itemEffect.text = "";
                    }
                    else
                    {
                        itemName.text = ItemManager.Instance.itemDataArr[GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId]].itemName;
                        itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId]].itemDesc;
                        itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId]].itemEffect;
                    }
                    break;
                case 25:
                    if (GameManager.Instance.necklaceItem[GameManager.Instance.playerId] == -1)
                    {
                        itemName.text = "";
                        itemDesc.text = "";
                        itemEffect.text = "";
                    }
                    else
                    {
                        itemName.text = ItemManager.Instance.itemDataArr[GameManager.Instance.necklaceItem[GameManager.Instance.playerId]].itemName;
                        itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.Instance.necklaceItem[GameManager.Instance.playerId]].itemDesc;
                        itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.Instance.necklaceItem[GameManager.Instance.playerId]].itemEffect;
                    }
                    break;
                case 26:
                    if (GameManager.Instance.shoesItem[GameManager.Instance.playerId] == -1)
                    {
                        itemName.text = "";
                        itemDesc.text = "";
                        itemEffect.text = "";
                    }
                    else
                    {
                        itemName.text = ItemManager.Instance.itemDataArr[GameManager.Instance.shoesItem[GameManager.Instance.playerId]].itemName;
                        itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.Instance.shoesItem[GameManager.Instance.playerId]].itemDesc;
                        itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.Instance.shoesItem[GameManager.Instance.playerId]].itemEffect;
                    }
                    break;
                case 27:
                    if (GameManager.Instance.rangeWeaponItem == -1)
                    {
                        itemName.text = "";
                        itemDesc.text = "";
                        itemEffect.text = "";
                    }
                    else
                    {
                        itemName.text = ItemManager.Instance.itemDataArr[GameManager.Instance.rangeWeaponItem].itemName;
                        itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.Instance.rangeWeaponItem].itemDesc;
                        itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.Instance.rangeWeaponItem].itemEffect;
                    }
                    break;
                case 28:
                    if (GameManager.Instance.magicItem == -1)
                    {
                        itemName.text = "";
                        itemDesc.text = "";
                        itemEffect.text = "";
                    }
                    else
                    {
                        itemName.text = ItemManager.Instance.itemDataArr[GameManager.Instance.magicItem].itemName;
                        itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.Instance.magicItem].itemDesc;
                        itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.Instance.magicItem].itemEffect;
                    }
                    break;
            }

        }
    }

    void ShowHelp()
    {
        // 컨트롤 도움말
        if (isPressed)
        {
            help.Show(InventoryControlHelp.ActionType.Pressed);
        }
        else if (selectedId > 23)
        {
            bool isEmpty = false;
            switch (selectedId)
            {
                case 24:
                    isEmpty = GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId] == -1;
                    break;

                case 25:
                    isEmpty = GameManager.Instance.necklaceItem[GameManager.Instance.playerId] == -1;
                    break;

                case 26:
                    isEmpty = GameManager.Instance.shoesItem[GameManager.Instance.playerId] == -1;
                    break;

                case 27:
                    isEmpty = GameManager.Instance.rangeWeaponItem == -1;
                    break;

                case 28:
                    isEmpty = GameManager.Instance.magicItem == -1;
                    break;
            }
            if (isEmpty)
            {
                help.Show(InventoryControlHelp.ActionType.Empty);
            }
            else
            {
                help.Show(InventoryControlHelp.ActionType.UnEquip);
            }
        }
        else
        {
            int itemId = GameManager.Instance.inventoryItemsId[selectedId];
            if (itemId == -1)
            {
                help.Show(InventoryControlHelp.ActionType.Empty);
            }
            else
            {
                switch (ItemManager.Instance.itemDataArr[itemId].itemType)
                {
                    case ItemData.ItemType.Potion:
                        help.Show(InventoryControlHelp.ActionType.Use);
                        break;

                    default:
                        help.Show(InventoryControlHelp.ActionType.Equip);
                        break;
                }
            }
        }
    }

    public void DestroyItem()
    {
        if (!gameObject.activeSelf) return;
        if (isPressed) return;
        if (!GameManager.Instance.workingInventory) return;
        if (selectedId > 23) return;
        if (GameManager.Instance.inventoryItemsId[selectedId] == -1) return;
        // 위 조건에 해당되지 않으면 파괴 버튼 도움말 팝업 띄우기

        EventSystem.current.SetSelectedGameObject(confirmNo.gameObject);
        selectedObejctOnDestroy = confirmNo.gameObject;
        isDestroying = true;
        destroyDesc.transform.parent.gameObject.SetActive(true);
        destroyDesc.text = string.Format("<color=green>{0}</color>\r\n을(를) 정말 <color=red>파괴</color>하시겠습니까?",
            ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemName);
        
    }
    
    public void OnConfirm(bool confirm)
    {
        if (confirm)
        {
            GameManager.Instance.inventoryItemsId[selectedId] = -1;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Destroy);
        }
        else
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
        }
        destroyDesc.transform.parent.gameObject.SetActive(false);
        isDestroying = false;
        EventSystem.current.SetSelectedGameObject(buttons[selectedId].gameObject);
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
                        ItemData.ItemType tempPressedItemType = ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[pressedId]].itemType;
                        switch (i)
                        {
                            case 24:
                                if (tempPressedItemType != ItemData.ItemType.Melee)
                                {
                                    buttons[i].GetComponent<Image>().color = redColor;
                                }
                                else
                                {
                                    if (GameManager.Instance.playerId == 0)
                                    {
                                        if (GameManager.Instance.inventoryItemsId[pressedId] < 3)
                                        {
                                            buttons[i].GetComponent<Image>().color = greenColor;
                                        }
                                        else
                                        {
                                            buttons[i].GetComponent<Image>().color = redColor;
                                        }
                                    }
                                    else if (GameManager.Instance.playerId == 1)
                                    {
                                        if (GameManager.Instance.inventoryItemsId[pressedId] > 2 && GameManager.Instance.inventoryItemsId[pressedId] < 6)
                                        {
                                            buttons[i].GetComponent<Image>().color = greenColor;
                                        }
                                        else
                                        {
                                            buttons[i].GetComponent<Image>().color = redColor;
                                        }
                                    }
                                }
                                break;
                            case 25:
                                if (tempPressedItemType != ItemData.ItemType.Necklace)
                                {
                                    buttons[i].GetComponent<Image>().color = redColor;
                                }
                                else
                                {
                                    buttons[i].GetComponent<Image>().color = greenColor;
                                }
                                break;
                            case 26:
                                if (tempPressedItemType != ItemData.ItemType.Shoes)
                                {
                                    buttons[i].GetComponent<Image>().color = redColor;
                                }
                                else
                                {
                                    buttons[i].GetComponent<Image>().color = greenColor;
                                }
                                break;
                            case 27:
                                if (tempPressedItemType != ItemData.ItemType.Range)
                                {
                                    buttons[i].GetComponent<Image>().color = redColor;
                                }
                                else
                                {
                                    buttons[i].GetComponent<Image>().color = greenColor;
                                }
                                break;
                            case 28:
                                if (tempPressedItemType != ItemData.ItemType.Magic)
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
                                    if (ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemType == ItemData.ItemType.Melee)
                                    {
                                        if (GameManager.Instance.playerId == 0)
                                        {
                                            if (GameManager.Instance.inventoryItemsId[selectedId] < 3)
                                            {
                                                buttons[i].GetComponent<Image>().color = greenColor;
                                            }
                                            else
                                            {
                                                buttons[i].GetComponent<Image>().color = redColor;
                                            }
                                        }
                                        else if (GameManager.Instance.playerId == 1)
                                        {
                                            if (GameManager.Instance.inventoryItemsId[selectedId] > 2 && GameManager.Instance.inventoryItemsId[selectedId] < 6)
                                            {
                                                buttons[i].GetComponent<Image>().color = greenColor;
                                            }
                                            else
                                            {
                                                buttons[i].GetComponent<Image>().color = redColor;
                                            }
                                        }
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
                                    if (ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemType == ItemData.ItemType.Necklace)
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
                                    if (ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemType == ItemData.ItemType.Shoes)
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
                                    if (ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemType == ItemData.ItemType.Range)
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
                                    if (ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemType == ItemData.ItemType.Magic)
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
            // 인벤토리 내 물품을 누른 상태에서
            if (pressedId < 24)
            {
                // 인벤토리 내 물품을 누른 경우
                if (buttonIndex < 24)
                {
                    int tempItemId = GameManager.Instance.inventoryItemsId[pressedId];
                    GameManager.Instance.inventoryItemsId[pressedId] = GameManager.Instance.inventoryItemsId[buttonIndex];
                    GameManager.Instance.inventoryItemsId[buttonIndex] = tempItemId;
                    isPressed = false;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
                    return;
                }
                // 장비창 물품을 누른 경우
                switch (buttonIndex)
                {
                    case 24:
                        // 주무기 장비
                        if (ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[pressedId]].itemType == ItemData.ItemType.Melee)
                        {
                            if (GameManager.Instance.playerId == 0)
                            {
                                if (GameManager.Instance.inventoryItemsId[pressedId] < 3)
                                {
                                    (GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId], GameManager.Instance.inventoryItemsId[pressedId]) 
                                        = (GameManager.Instance.inventoryItemsId[pressedId], GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId]);
                                    isPressed = false;
                                    GameManager.Instance.StatusUpdate();
                                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                                }
                                else
                                {
                                    help.Show(InventoryControlHelp.ActionType.WrongClass);
                                }
                            }
                            else if (GameManager.Instance.playerId == 1)
                            {
                                if (GameManager.Instance.inventoryItemsId[pressedId] > 2 && GameManager.Instance.inventoryItemsId[pressedId] < 6)
                                {
                                    (GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId], GameManager.Instance.inventoryItemsId[pressedId])
                                        = (GameManager.Instance.inventoryItemsId[pressedId], GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId]);
                                    isPressed = false;
                                    GameManager.Instance.StatusUpdate();
                                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                                }
                                else
                                {
                                    help.Show(InventoryControlHelp.ActionType.WrongClass);
                                }
                            }
                        }
                        else
                        {
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                            help.Show(InventoryControlHelp.ActionType.WrongPosition);
                        }
                        return;
                    case 25:
                        // 목걸이 장비
                        if (ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[pressedId]].itemType == ItemData.ItemType.Necklace)
                        {
                            (GameManager.Instance.necklaceItem[GameManager.Instance.playerId], GameManager.Instance.inventoryItemsId[pressedId])
                                = (GameManager.Instance.inventoryItemsId[pressedId], GameManager.Instance.necklaceItem[GameManager.Instance.playerId]);
                            isPressed = false;
                            GameManager.Instance.StatusUpdate();
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                        }
                        else
                        {
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                            help.Show(InventoryControlHelp.ActionType.WrongPosition);
                        }
                        return;
                    case 26:
                        // 신발 장비
                        if (ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[pressedId]].itemType == ItemData.ItemType.Shoes)
                        {
                            (GameManager.Instance.shoesItem[GameManager.Instance.playerId], GameManager.Instance.inventoryItemsId[pressedId])
                                = (GameManager.Instance.inventoryItemsId[pressedId], GameManager.Instance.shoesItem[GameManager.Instance.playerId]);
                            isPressed = false;
                            GameManager.Instance.StatusUpdate();
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                        }
                        else
                        {
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                            help.Show(InventoryControlHelp.ActionType.WrongPosition);
                        }
                        return;
                    case 27:
                        //투척 무기 장비
                        if (ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[pressedId]].itemType == ItemData.ItemType.Range)
                        {
                            (GameManager.Instance.rangeWeaponItem, GameManager.Instance.inventoryItemsId[pressedId])
                                = (GameManager.Instance.inventoryItemsId[pressedId], GameManager.Instance.rangeWeaponItem);
                            isPressed = false;
                            GameManager.Instance.StatusUpdate();
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                        }
                        else
                        {
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                            help.Show(InventoryControlHelp.ActionType.WrongPosition);
                        }
                        return;
                    case 28:
                        // 마법책 장비
                        if (ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[pressedId]].itemType == ItemData.ItemType.Magic)
                        {
                            (GameManager.Instance.magicItem, GameManager.Instance.inventoryItemsId[pressedId])
                                = (GameManager.Instance.inventoryItemsId[pressedId], GameManager.Instance.magicItem);
                            isPressed = false;
                            GameManager.Instance.StatusUpdate();
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                        }
                        else
                        {
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                            help.Show(InventoryControlHelp.ActionType.WrongPosition);
                        }
                        return;
                }
            }
            // 장비창 물품을 누른 상태에서
            else
            {
                // 장비창 물품을 누른 경우
                if (buttonIndex > 23)
                {
                    if (buttonIndex == pressedId)
                    {
                        //nothing happened
                        isPressed = false;
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
                    }
                    else
                    {
                        //fail
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                        help.Show(InventoryControlHelp.ActionType.WrongPosition);
                    }
                    return;
                }
                // 인벤토리 내 물품을 누른 경우
                else
                {
                    switch (pressedId)
                    {
                        // 주무기가 눌려 있는 경우
                        case 24:
                            // 인벤토리 내 빈슬롯을 누른 경우
                            if (GameManager.Instance.inventoryItemsId[buttonIndex] == -1)
                            {
                                GameManager.Instance.inventoryItemsId[buttonIndex] = GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId];
                                GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId] = -1;
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Unequip);
                            }
                            // 인벤토리 내 물품이 Melee 타입인 경우
                            else if (ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[buttonIndex]].itemType == ItemData.ItemType.Melee)
                            {
                                if (GameManager.Instance.playerId == 0)
                                {
                                    if (GameManager.Instance.inventoryItemsId[buttonIndex] < 3)
                                    {
                                        (GameManager.Instance.inventoryItemsId[buttonIndex], GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId])
                                            = (GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId], GameManager.Instance.inventoryItemsId[buttonIndex]);
                                        isPressed = false;
                                        GameManager.Instance.StatusUpdate();
                                        AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                                    }
                                    else
                                    {
                                        help.Show(InventoryControlHelp.ActionType.WrongClass);
                                    }
                                }
                                else if (GameManager.Instance.playerId == 1)
                                {
                                    if (GameManager.Instance.inventoryItemsId[buttonIndex] > 2 && GameManager.Instance.inventoryItemsId[buttonIndex] < 6)
                                    {
                                        (GameManager.Instance.inventoryItemsId[buttonIndex], GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId])
                                            = (GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId], GameManager.Instance.inventoryItemsId[buttonIndex]);
                                        isPressed = false;
                                        GameManager.Instance.StatusUpdate();
                                        AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                                    }
                                    else
                                    {
                                        help.Show(InventoryControlHelp.ActionType.WrongClass);
                                    }
                                }
                            }
                            // 인벤토리 내 물품과 위치를 바꿀 수 없는 경우
                            else
                            {
                                //fail
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                                help.Show(InventoryControlHelp.ActionType.WrongItem);
                            }
                            return;

                        // 목걸이가 눌려 있는 경우
                        case 25:
                            // 인벤토리 내 빈슬롯을 누른 경우
                            if (GameManager.Instance.inventoryItemsId[buttonIndex] == -1)
                            {
                                GameManager.Instance.inventoryItemsId[buttonIndex] = GameManager.Instance.necklaceItem[GameManager.Instance.playerId];
                                GameManager.Instance.necklaceItem[GameManager.Instance.playerId] = -1;
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                            }
                            // 인벤토리 내 물품이 Necklace 타입인 경우
                            else if (ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[buttonIndex]].itemType == ItemData.ItemType.Necklace)
                            {
                                (GameManager.Instance.inventoryItemsId[buttonIndex], GameManager.Instance.necklaceItem[GameManager.Instance.playerId])
                                    = (GameManager.Instance.necklaceItem[GameManager.Instance.playerId], GameManager.Instance.inventoryItemsId[buttonIndex]);
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Unequip);
                            }
                            // 인벤토리 내 물품과 위치를 바꿀 수 없는 경우
                            else
                            {
                                //fail
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                                help.Show(InventoryControlHelp.ActionType.WrongItem);
                            }
                            return;

                        // 신발이 눌려 있는 경우
                        case 26:
                            // 인벤토리 내 빈슬롯을 누른 경우
                            if (GameManager.Instance.inventoryItemsId[buttonIndex] == -1)
                            {
                                GameManager.Instance.inventoryItemsId[buttonIndex] = GameManager.Instance.shoesItem[GameManager.Instance.playerId];
                                GameManager.Instance.shoesItem[GameManager.Instance.playerId] = -1;
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                            }
                            // 인벤토리 내 물품이 Shoes 타입인 경우
                            else if (ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[buttonIndex]].itemType == ItemData.ItemType.Shoes)
                            {
                                (GameManager.Instance.inventoryItemsId[buttonIndex], GameManager.Instance.shoesItem[GameManager.Instance.playerId])
                                    = (GameManager.Instance.shoesItem[GameManager.Instance.playerId], GameManager.Instance.inventoryItemsId[buttonIndex]);
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Unequip);
                            }
                            // 인벤토리 내 물품과 위치를 바꿀 수 없는 경우
                            else
                            {
                                //fail
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                                help.Show(InventoryControlHelp.ActionType.WrongItem);
                            }
                            return;

                        // 투척무기가 눌려 있는 경우
                        case 27:
                            // 인벤토리 내 빈슬롯을 누른 경우
                            if (GameManager.Instance.inventoryItemsId[buttonIndex] == -1)
                            {
                                GameManager.Instance.inventoryItemsId[buttonIndex] = GameManager.Instance.rangeWeaponItem;
                                GameManager.Instance.rangeWeaponItem = -1;
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                            }
                            // 인벤토리 내 물품이 Range 타입인 경우
                            else if (ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[buttonIndex]].itemType == ItemData.ItemType.Range)
                            {
                                (GameManager.Instance.inventoryItemsId[buttonIndex], GameManager.Instance.rangeWeaponItem)
                                    = (GameManager.Instance.rangeWeaponItem, GameManager.Instance.inventoryItemsId[buttonIndex]);
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Unequip);
                            }
                            // 인벤토리 내 물품과 위치를 바꿀 수 없는 경우
                            else
                            {
                                //fail
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                                help.Show(InventoryControlHelp.ActionType.WrongItem);
                            }
                            return;

                        // 마법책이 눌려 있는 경우
                        case 28:
                            // 인벤토리 내 빈슬롯을 누른 경우
                            if (GameManager.Instance.inventoryItemsId[buttonIndex] == -1)
                            {
                                GameManager.Instance.inventoryItemsId[buttonIndex] = GameManager.Instance.magicItem;
                                GameManager.Instance.magicItem = -1;
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                            }
                            // 인벤토리 내 물품이 Magic 타입인 경우
                            else if (ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[buttonIndex]].itemType == ItemData.ItemType.Magic)
                            {
                                (GameManager.Instance.inventoryItemsId[buttonIndex], GameManager.Instance.magicItem) 
                                    = (GameManager.Instance.magicItem, GameManager.Instance.inventoryItemsId[buttonIndex]);
                                isPressed = false;
                                GameManager.Instance.StatusUpdate();
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Unequip);
                            }
                            // 인벤토리 내 물품과 위치를 바꿀 수 없는 경우
                            else
                            {
                                //fail
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                                help.Show(InventoryControlHelp.ActionType.WrongItem);
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

        AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
        pressedId = buttonIndex;
        isPressed = true;
        ChangeAlpha(halfAlpha);
        itemImages[buttonIndex].color = originAlpha;
        buttons[buttonIndex].GetComponent<Image>().color = grayColor;
    }

    // 인벤토리 창에서 아무 버튼도 선택되지 않은 경우에 Menu키 (키보드: Esc, 게임패드: Start)로 빠져나올 수 있도록.
    public void OnMenu()
    {
        if (!gameObject.activeSelf) return;

        if (GameManager.Instance.workingInventory)
        {
            if (!isPressed)
            {
                if (isDestroying)
                {
                    isDestroying = false;
                    destroyDesc.transform.parent.gameObject.SetActive(false);
                    EventSystem.current.SetSelectedGameObject(buttons[selectedId].gameObject);
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
                }
                else
                {
                    GameManager.Instance.OnInventory();
                }
            }
            else
            {
                isPressed = false;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
            }
        }
    }

    public void OnCancel()
    {
        if (!gameObject.activeSelf) return;

        if (GameManager.Instance.workingInventory)
        {
            if (isPressed)
            {
                EventSystem.current.SetSelectedGameObject(buttons[pressedId].gameObject);
                isPressed = false;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);

            }
            else if (isDestroying)
            {
                isDestroying = false;
                destroyDesc.transform.parent.gameObject.SetActive(false);
                EventSystem.current.SetSelectedGameObject(buttons[selectedId].gameObject);
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
            }
            else
            {
                //GameManager.Instance.OnInventory();
            }
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

    public void EquipUnequip()
    {
        if (!gameObject.activeSelf) return;
        if (!GameManager.Instance.workingInventory) return;
        if (isPressed) return;
        if (selectedId < 24)
        {
            int selectedItemId = GameManager.Instance.inventoryItemsId[selectedId];
            if (selectedItemId == -1) return;
            ItemData.ItemType selectedItemType = ItemManager.Instance.itemDataArr[selectedItemId].itemType;
            switch (selectedItemType)
            {
                case ItemData.ItemType.Melee:
                    int tempItemId;
                    if (GameManager.Instance.playerId == 0)
                    {
                        if (selectedItemId < 3)
                        {
                            tempItemId = GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId];
                            GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId] = selectedItemId;
                            GameManager.Instance.inventoryItemsId[selectedId] = tempItemId;
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                        }
                        else
                        {
                            help.Show(InventoryControlHelp.ActionType.WrongClass);
                        }
                    }
                    else if (GameManager.Instance.playerId == 1)
                    {
                        if (selectedItemId > 2 && selectedItemId < 6)
                        {
                            tempItemId = GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId];
                            GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId] = selectedItemId;
                            GameManager.Instance.inventoryItemsId[selectedId] = tempItemId;
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                        }
                        else
                        {
                            help.Show(InventoryControlHelp.ActionType.WrongClass);
                        }
                    }
                    break;

                case ItemData.ItemType.Necklace:
                    tempItemId = GameManager.Instance.necklaceItem[GameManager.Instance.playerId];
                    GameManager.Instance.necklaceItem[GameManager.Instance.playerId] = selectedItemId;
                    GameManager.Instance.inventoryItemsId[selectedId] = tempItemId;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                    break;

                case ItemData.ItemType.Shoes:
                    tempItemId = GameManager.Instance.shoesItem[GameManager.Instance.playerId];
                    GameManager.Instance.shoesItem[GameManager.Instance.playerId] = selectedItemId;
                    GameManager.Instance.inventoryItemsId[selectedId] = tempItemId;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                    break;

                case ItemData.ItemType.Range:
                    tempItemId = GameManager.Instance.rangeWeaponItem;
                    GameManager.Instance.rangeWeaponItem = selectedItemId;
                    GameManager.Instance.inventoryItemsId[selectedId] = tempItemId;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                    break;

                case ItemData.ItemType.Magic:
                    tempItemId = GameManager.Instance.magicItem;
                    GameManager.Instance.magicItem = selectedItemId;
                    GameManager.Instance.inventoryItemsId[selectedId] = tempItemId;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                    break;

                case ItemData.ItemType.Potion:
                    if (Mathf.Abs(GameManager.Instance.maxHealth - GameManager.Instance.health) < 0.1f)
                    {
                        Debug.Log("이미 체력이 가득 차 있습니다.");
                        // 에러 메시지 띄우기
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                        help.Show(InventoryControlHelp.ActionType.FullHeart);
                    }
                    else
                    {
                        GameManager.Instance.health = Mathf.Clamp(GameManager.Instance.health +
                            ItemManager.Instance.itemDataArr[selectedItemId].baseAmount, 0, GameManager.Instance.maxHealth);
                        GameManager.Instance.inventoryItemsId[selectedId] = -1;
                        // 물약 마시는 소리 추가
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.Healthy);
                    }
                    break;
            }
            GameManager.Instance.StatusUpdate();
        }
        else
        {
            

            int selectedSlot = -1;
            for (int i = 0; i < GameManager.Instance.maxInventory; i++)
            {
                if (GameManager.Instance.inventoryItemsId[i] == -1)
                {
                    selectedSlot = i;
                    break;
                }
            }
            // 인벤토리에 자리가 없는 경우
            if (selectedSlot == -1)
            {
                switch (selectedId)
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

                AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                help.Show(InventoryControlHelp.ActionType.FullMsg);
                return;
            }
            
            switch(selectedId)
            {
                case 24:
                    GameManager.Instance.inventoryItemsId[selectedSlot] = GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId];
                    GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId] = -1;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Unequip);
                    break;

                case 25:
                    GameManager.Instance.inventoryItemsId[selectedSlot] = GameManager.Instance.necklaceItem[GameManager.Instance.playerId];
                    GameManager.Instance.necklaceItem[GameManager.Instance.playerId] = -1;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Unequip);
                    break;

                case 26:
                    GameManager.Instance.inventoryItemsId[selectedSlot] = GameManager.Instance.shoesItem[GameManager.Instance.playerId];
                    GameManager.Instance.shoesItem[GameManager.Instance.playerId] = -1;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Unequip);
                    break;

                case 27:
                    GameManager.Instance.inventoryItemsId[selectedSlot] = GameManager.Instance.rangeWeaponItem;
                    GameManager.Instance.rangeWeaponItem = -1;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Unequip);
                    break;

                case 28:
                    GameManager.Instance.inventoryItemsId[selectedSlot] = GameManager.Instance.magicItem;
                    GameManager.Instance.magicItem = -1;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Unequip);
                    break;
            }
            GameManager.Instance.StatusUpdate();
        }
    }
}
