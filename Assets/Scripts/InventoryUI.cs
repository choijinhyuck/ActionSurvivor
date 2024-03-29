using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;

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
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

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
        InitLanguage();

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


        for (int i = 0; i < GameManager.instance.maxInventory; i++)
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
            if (GameManager.instance.inventoryItemsId[selectedId] == -1)
            {
                itemName.text = "";
                itemDesc.text = "";
                itemEffect.text = "";
            }
            else
            {
                if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
                {
                    itemName.text = ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[selectedId]].itemName;
                    itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[selectedId]].itemDesc;
                    itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[selectedId]].itemEffect;
                }
                else
                {
                    itemName.text = ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[selectedId]].itemNameEng;
                    itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[selectedId]].itemDescEng;
                    itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[selectedId]].itemEffectEng;
                }
                
            }
        }
        else
        {
            switch (selectedId)
            {
                case 24:
                    if (GameManager.instance.mainWeaponItem[GameManager.instance.playerId] == -1)
                    {
                        itemName.text = "";
                        itemDesc.text = "";
                        itemEffect.text = "";
                    }
                    else
                    {
                        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
                        {
                            itemName.text = ItemManager.Instance.itemDataArr[GameManager.instance.mainWeaponItem[GameManager.instance.playerId]].itemName;
                            itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.instance.mainWeaponItem[GameManager.instance.playerId]].itemDesc;
                            itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.instance.mainWeaponItem[GameManager.instance.playerId]].itemEffect;
                        }
                        else
                        {
                            itemName.text = ItemManager.Instance.itemDataArr[GameManager.instance.mainWeaponItem[GameManager.instance.playerId]].itemNameEng;
                            itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.instance.mainWeaponItem[GameManager.instance.playerId]].itemDescEng;
                            itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.instance.mainWeaponItem[GameManager.instance.playerId]].itemEffectEng;
                        }
                        
                    }
                    break;
                case 25:
                    if (GameManager.instance.necklaceItem[GameManager.instance.playerId] == -1)
                    {
                        itemName.text = "";
                        itemDesc.text = "";
                        itemEffect.text = "";
                    }
                    else
                    {
                        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
                        {
                            itemName.text = ItemManager.Instance.itemDataArr[GameManager.instance.necklaceItem[GameManager.instance.playerId]].itemName;
                            itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.instance.necklaceItem[GameManager.instance.playerId]].itemDesc;
                            itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.instance.necklaceItem[GameManager.instance.playerId]].itemEffect;
                        }
                        else
                        {
                            itemName.text = ItemManager.Instance.itemDataArr[GameManager.instance.necklaceItem[GameManager.instance.playerId]].itemNameEng;
                            itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.instance.necklaceItem[GameManager.instance.playerId]].itemDescEng;
                            itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.instance.necklaceItem[GameManager.instance.playerId]].itemEffectEng;
                        }
                    }
                    break;
                case 26:
                    if (GameManager.instance.shoesItem[GameManager.instance.playerId] == -1)
                    {
                        itemName.text = "";
                        itemDesc.text = "";
                        itemEffect.text = "";
                    }
                    else
                    {
                        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
                        {
                            itemName.text = ItemManager.Instance.itemDataArr[GameManager.instance.shoesItem[GameManager.instance.playerId]].itemName;
                            itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.instance.shoesItem[GameManager.instance.playerId]].itemDesc;
                            itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.instance.shoesItem[GameManager.instance.playerId]].itemEffect;
                        }
                        else
                        {
                            itemName.text = ItemManager.Instance.itemDataArr[GameManager.instance.shoesItem[GameManager.instance.playerId]].itemNameEng;
                            itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.instance.shoesItem[GameManager.instance.playerId]].itemDescEng;
                            itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.instance.shoesItem[GameManager.instance.playerId]].itemEffectEng;
                        }
                    }
                    break;
                case 27:
                    if (GameManager.instance.rangeWeaponItem == -1)
                    {
                        itemName.text = "";
                        itemDesc.text = "";
                        itemEffect.text = "";
                    }
                    else
                    {
                        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
                        {
                            itemName.text = ItemManager.Instance.itemDataArr[GameManager.instance.rangeWeaponItem].itemName;
                            itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.instance.rangeWeaponItem].itemDesc;
                            itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.instance.rangeWeaponItem].itemEffect;
                        }
                        else
                        {
                            itemName.text = ItemManager.Instance.itemDataArr[GameManager.instance.rangeWeaponItem].itemNameEng;
                            itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.instance.rangeWeaponItem].itemDescEng;
                            itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.instance.rangeWeaponItem].itemEffectEng;
                        }
                    }
                    break;
                case 28:
                    if (GameManager.instance.magicItem == -1)
                    {
                        itemName.text = "";
                        itemDesc.text = "";
                        itemEffect.text = "";
                    }
                    else
                    {
                        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
                        {
                            itemName.text = ItemManager.Instance.itemDataArr[GameManager.instance.magicItem].itemName;
                            itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.instance.magicItem].itemDesc;
                            itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.instance.magicItem].itemEffect;
                        }
                        else
                        {
                            itemName.text = ItemManager.Instance.itemDataArr[GameManager.instance.magicItem].itemNameEng;
                            itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.instance.magicItem].itemDescEng;
                            itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.instance.magicItem].itemEffectEng;
                        }
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
                    isEmpty = GameManager.instance.mainWeaponItem[GameManager.instance.playerId] == -1;
                    break;

                case 25:
                    isEmpty = GameManager.instance.necklaceItem[GameManager.instance.playerId] == -1;
                    break;

                case 26:
                    isEmpty = GameManager.instance.shoesItem[GameManager.instance.playerId] == -1;
                    break;

                case 27:
                    isEmpty = GameManager.instance.rangeWeaponItem == -1;
                    break;

                case 28:
                    isEmpty = GameManager.instance.magicItem == -1;
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
            int itemId = GameManager.instance.inventoryItemsId[selectedId];
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
        if (!GameManager.instance.workingInventory) return;
        if (selectedId > 23) return;
        if (GameManager.instance.inventoryItemsId[selectedId] == -1) return;
        // 위 조건에 해당되지 않으면 파괴 버튼 도움말 팝업 띄우기

        EventSystem.current.SetSelectedGameObject(confirmNo.gameObject);
        selectedObejctOnDestroy = confirmNo.gameObject;
        isDestroying = true;
        destroyDesc.transform.parent.gameObject.SetActive(true);
        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
        {
            destroyDesc.text = string.Format("<color=green>{0}</color>\r\n을(를) 정말 <color=red>파괴</color>하시겠습니까?",
            ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[selectedId]].itemName);
        }
        else
        {
            destroyDesc.text = string.Format("Are you sure you want to <color=red>Destroy</color>\r\n<color=green>{0}</color>?",
            ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[selectedId]].itemNameEng);
        }


    }

    public void OnConfirm(bool confirm)
    {
        if (confirm)
        {
            GameManager.instance.inventoryItemsId[selectedId] = -1;
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
        for (int i = 0; i < GameManager.instance.maxInventory; i++)
        {
            if (GameManager.instance.inventoryItemsId[i] == -1)
            {
                emptySlot(i);
            }
            else
            {
                itemImages[i].sprite = ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[i]].itemIcon;
                slotAlpha(i);
            }
        }

        for (int i = 24; i < 29; i++)
        {
            int tempItemId = -1;
            switch (i)
            {
                case 24:
                    tempItemId = GameManager.instance.mainWeaponItem[GameManager.instance.playerId];
                    break;
                case 25:
                    tempItemId = GameManager.instance.necklaceItem[GameManager.instance.playerId];
                    break;
                case 26:
                    tempItemId = GameManager.instance.shoesItem[GameManager.instance.playerId];
                    break;
                case 27:
                    tempItemId = GameManager.instance.rangeWeaponItem;
                    break;
                case 28:
                    tempItemId = GameManager.instance.magicItem;
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
                        ItemData.ItemType tempPressedItemType = ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[pressedId]].itemType;
                        switch (i)
                        {
                            case 24:
                                if (tempPressedItemType != ItemData.ItemType.Melee)
                                {
                                    buttons[i].GetComponent<Image>().color = redColor;
                                }
                                else
                                {
                                    if (GameManager.instance.playerId == 0)
                                    {
                                        if (GameManager.instance.inventoryItemsId[pressedId] < 3)
                                        {
                                            buttons[i].GetComponent<Image>().color = greenColor;
                                        }
                                        else
                                        {
                                            buttons[i].GetComponent<Image>().color = redColor;
                                        }
                                    }
                                    else if (GameManager.instance.playerId == 1)
                                    {
                                        if (GameManager.instance.inventoryItemsId[pressedId] > 2 && GameManager.instance.inventoryItemsId[pressedId] < 6)
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
                                    if (GameManager.instance.inventoryItemsId[selectedId] == -1)
                                    {
                                        buttons[i].GetComponent<Image>().color = greenColor;
                                        break;
                                    }
                                    if (ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[selectedId]].itemType == ItemData.ItemType.Melee)
                                    {
                                        if (GameManager.instance.playerId == 0)
                                        {
                                            if (GameManager.instance.inventoryItemsId[selectedId] < 3)
                                            {
                                                buttons[i].GetComponent<Image>().color = greenColor;
                                            }
                                            else
                                            {
                                                buttons[i].GetComponent<Image>().color = redColor;
                                            }
                                        }
                                        else if (GameManager.instance.playerId == 1)
                                        {
                                            if (GameManager.instance.inventoryItemsId[selectedId] > 2 && GameManager.instance.inventoryItemsId[selectedId] < 6)
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
                                    if (GameManager.instance.inventoryItemsId[selectedId] == -1)
                                    {
                                        buttons[i].GetComponent<Image>().color = greenColor;
                                        break;
                                    }
                                    if (ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[selectedId]].itemType == ItemData.ItemType.Necklace)
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
                                    if (GameManager.instance.inventoryItemsId[selectedId] == -1)
                                    {
                                        buttons[i].GetComponent<Image>().color = greenColor;
                                        break;
                                    }
                                    if (ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[selectedId]].itemType == ItemData.ItemType.Shoes)
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
                                    if (GameManager.instance.inventoryItemsId[selectedId] == -1)
                                    {
                                        buttons[i].GetComponent<Image>().color = greenColor;
                                        break;
                                    }
                                    if (ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[selectedId]].itemType == ItemData.ItemType.Range)
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
                                    if (GameManager.instance.inventoryItemsId[selectedId] == -1)
                                    {
                                        buttons[i].GetComponent<Image>().color = greenColor;
                                        break;
                                    }
                                    if (ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[selectedId]].itemType == ItemData.ItemType.Magic)
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
                    int tempItemId = GameManager.instance.inventoryItemsId[pressedId];
                    GameManager.instance.inventoryItemsId[pressedId] = GameManager.instance.inventoryItemsId[buttonIndex];
                    GameManager.instance.inventoryItemsId[buttonIndex] = tempItemId;
                    isPressed = false;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
                    return;
                }
                // 장비창 물품을 누른 경우
                switch (buttonIndex)
                {
                    case 24:
                        // 주무기 장비
                        if (ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[pressedId]].itemType == ItemData.ItemType.Melee)
                        {
                            if (GameManager.instance.playerId == 0)
                            {
                                if (GameManager.instance.inventoryItemsId[pressedId] < 3)
                                {
                                    (GameManager.instance.mainWeaponItem[GameManager.instance.playerId], GameManager.instance.inventoryItemsId[pressedId])
                                        = (GameManager.instance.inventoryItemsId[pressedId], GameManager.instance.mainWeaponItem[GameManager.instance.playerId]);
                                    isPressed = false;
                                    GameManager.instance.StatusUpdate();
                                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                                }
                                else
                                {
                                    help.Show(InventoryControlHelp.ActionType.WrongClass);
                                }
                            }
                            else if (GameManager.instance.playerId == 1)
                            {
                                if (GameManager.instance.inventoryItemsId[pressedId] > 2 && GameManager.instance.inventoryItemsId[pressedId] < 6)
                                {
                                    (GameManager.instance.mainWeaponItem[GameManager.instance.playerId], GameManager.instance.inventoryItemsId[pressedId])
                                        = (GameManager.instance.inventoryItemsId[pressedId], GameManager.instance.mainWeaponItem[GameManager.instance.playerId]);
                                    isPressed = false;
                                    GameManager.instance.StatusUpdate();
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
                        if (ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[pressedId]].itemType == ItemData.ItemType.Necklace)
                        {
                            (GameManager.instance.necklaceItem[GameManager.instance.playerId], GameManager.instance.inventoryItemsId[pressedId])
                                = (GameManager.instance.inventoryItemsId[pressedId], GameManager.instance.necklaceItem[GameManager.instance.playerId]);
                            isPressed = false;
                            GameManager.instance.StatusUpdate();
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
                        if (ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[pressedId]].itemType == ItemData.ItemType.Shoes)
                        {
                            (GameManager.instance.shoesItem[GameManager.instance.playerId], GameManager.instance.inventoryItemsId[pressedId])
                                = (GameManager.instance.inventoryItemsId[pressedId], GameManager.instance.shoesItem[GameManager.instance.playerId]);
                            isPressed = false;
                            GameManager.instance.StatusUpdate();
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
                        if (ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[pressedId]].itemType == ItemData.ItemType.Range)
                        {
                            (GameManager.instance.rangeWeaponItem, GameManager.instance.inventoryItemsId[pressedId])
                                = (GameManager.instance.inventoryItemsId[pressedId], GameManager.instance.rangeWeaponItem);
                            isPressed = false;
                            GameManager.instance.StatusUpdate();
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
                        if (ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[pressedId]].itemType == ItemData.ItemType.Magic)
                        {
                            (GameManager.instance.magicItem, GameManager.instance.inventoryItemsId[pressedId])
                                = (GameManager.instance.inventoryItemsId[pressedId], GameManager.instance.magicItem);
                            isPressed = false;
                            GameManager.instance.StatusUpdate();
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
                            if (GameManager.instance.inventoryItemsId[buttonIndex] == -1)
                            {
                                GameManager.instance.inventoryItemsId[buttonIndex] = GameManager.instance.mainWeaponItem[GameManager.instance.playerId];
                                GameManager.instance.mainWeaponItem[GameManager.instance.playerId] = -1;
                                isPressed = false;
                                GameManager.instance.StatusUpdate();
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Unequip);
                            }
                            // 인벤토리 내 물품이 Melee 타입인 경우
                            else if (ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[buttonIndex]].itemType == ItemData.ItemType.Melee)
                            {
                                if (GameManager.instance.playerId == 0)
                                {
                                    if (GameManager.instance.inventoryItemsId[buttonIndex] < 3)
                                    {
                                        (GameManager.instance.inventoryItemsId[buttonIndex], GameManager.instance.mainWeaponItem[GameManager.instance.playerId])
                                            = (GameManager.instance.mainWeaponItem[GameManager.instance.playerId], GameManager.instance.inventoryItemsId[buttonIndex]);
                                        isPressed = false;
                                        GameManager.instance.StatusUpdate();
                                        AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                                    }
                                    else
                                    {
                                        help.Show(InventoryControlHelp.ActionType.WrongClass);
                                    }
                                }
                                else if (GameManager.instance.playerId == 1)
                                {
                                    if (GameManager.instance.inventoryItemsId[buttonIndex] > 2 && GameManager.instance.inventoryItemsId[buttonIndex] < 6)
                                    {
                                        (GameManager.instance.inventoryItemsId[buttonIndex], GameManager.instance.mainWeaponItem[GameManager.instance.playerId])
                                            = (GameManager.instance.mainWeaponItem[GameManager.instance.playerId], GameManager.instance.inventoryItemsId[buttonIndex]);
                                        isPressed = false;
                                        GameManager.instance.StatusUpdate();
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
                            if (GameManager.instance.inventoryItemsId[buttonIndex] == -1)
                            {
                                GameManager.instance.inventoryItemsId[buttonIndex] = GameManager.instance.necklaceItem[GameManager.instance.playerId];
                                GameManager.instance.necklaceItem[GameManager.instance.playerId] = -1;
                                isPressed = false;
                                GameManager.instance.StatusUpdate();
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                            }
                            // 인벤토리 내 물품이 Necklace 타입인 경우
                            else if (ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[buttonIndex]].itemType == ItemData.ItemType.Necklace)
                            {
                                (GameManager.instance.inventoryItemsId[buttonIndex], GameManager.instance.necklaceItem[GameManager.instance.playerId])
                                    = (GameManager.instance.necklaceItem[GameManager.instance.playerId], GameManager.instance.inventoryItemsId[buttonIndex]);
                                isPressed = false;
                                GameManager.instance.StatusUpdate();
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
                            if (GameManager.instance.inventoryItemsId[buttonIndex] == -1)
                            {
                                GameManager.instance.inventoryItemsId[buttonIndex] = GameManager.instance.shoesItem[GameManager.instance.playerId];
                                GameManager.instance.shoesItem[GameManager.instance.playerId] = -1;
                                isPressed = false;
                                GameManager.instance.StatusUpdate();
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                            }
                            // 인벤토리 내 물품이 Shoes 타입인 경우
                            else if (ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[buttonIndex]].itemType == ItemData.ItemType.Shoes)
                            {
                                (GameManager.instance.inventoryItemsId[buttonIndex], GameManager.instance.shoesItem[GameManager.instance.playerId])
                                    = (GameManager.instance.shoesItem[GameManager.instance.playerId], GameManager.instance.inventoryItemsId[buttonIndex]);
                                isPressed = false;
                                GameManager.instance.StatusUpdate();
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
                            if (GameManager.instance.inventoryItemsId[buttonIndex] == -1)
                            {
                                GameManager.instance.inventoryItemsId[buttonIndex] = GameManager.instance.rangeWeaponItem;
                                GameManager.instance.rangeWeaponItem = -1;
                                isPressed = false;
                                GameManager.instance.StatusUpdate();
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                            }
                            // 인벤토리 내 물품이 Range 타입인 경우
                            else if (ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[buttonIndex]].itemType == ItemData.ItemType.Range)
                            {
                                (GameManager.instance.inventoryItemsId[buttonIndex], GameManager.instance.rangeWeaponItem)
                                    = (GameManager.instance.rangeWeaponItem, GameManager.instance.inventoryItemsId[buttonIndex]);
                                isPressed = false;
                                GameManager.instance.StatusUpdate();
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
                            if (GameManager.instance.inventoryItemsId[buttonIndex] == -1)
                            {
                                GameManager.instance.inventoryItemsId[buttonIndex] = GameManager.instance.magicItem;
                                GameManager.instance.magicItem = -1;
                                isPressed = false;
                                GameManager.instance.StatusUpdate();
                                AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                            }
                            // 인벤토리 내 물품이 Magic 타입인 경우
                            else if (ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[buttonIndex]].itemType == ItemData.ItemType.Magic)
                            {
                                (GameManager.instance.inventoryItemsId[buttonIndex], GameManager.instance.magicItem)
                                    = (GameManager.instance.magicItem, GameManager.instance.inventoryItemsId[buttonIndex]);
                                isPressed = false;
                                GameManager.instance.StatusUpdate();
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
                    if (GameManager.instance.mainWeaponItem[GameManager.instance.playerId] == -1) return;
                    break;
                case 25:
                    if (GameManager.instance.necklaceItem[GameManager.instance.playerId] == -1) return;
                    break;
                case 26:
                    if (GameManager.instance.shoesItem[GameManager.instance.playerId] == -1) return;
                    break;
                case 27:
                    if (GameManager.instance.rangeWeaponItem == -1) return;
                    break;
                case 28:
                    if (GameManager.instance.magicItem == -1) return;
                    break;
            }
        }
        else if (GameManager.instance.inventoryItemsId[buttonIndex] == -1)
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
        if (GameManager.instance.workingInventory)
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
                    GameManager.instance.OnInventory();
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

        if (GameManager.instance.workingInventory)
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
            if (i < 24 && GameManager.instance.inventoryItemsId[i] == -1)
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
        if (!GameManager.instance.workingInventory) return;
        if (isPressed) return;
        if (selectedId < 24)
        {
            int selectedItemId = GameManager.instance.inventoryItemsId[selectedId];
            if (selectedItemId == -1) return;
            ItemData.ItemType selectedItemType = ItemManager.Instance.itemDataArr[selectedItemId].itemType;
            switch (selectedItemType)
            {
                case ItemData.ItemType.Melee:
                    int tempItemId;
                    if (GameManager.instance.playerId == 0)
                    {
                        if (selectedItemId < 3)
                        {
                            tempItemId = GameManager.instance.mainWeaponItem[GameManager.instance.playerId];
                            GameManager.instance.mainWeaponItem[GameManager.instance.playerId] = selectedItemId;
                            GameManager.instance.inventoryItemsId[selectedId] = tempItemId;
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                        }
                        else
                        {
                            help.Show(InventoryControlHelp.ActionType.WrongClass);
                        }
                    }
                    else if (GameManager.instance.playerId == 1)
                    {
                        if (selectedItemId > 2 && selectedItemId < 6)
                        {
                            tempItemId = GameManager.instance.mainWeaponItem[GameManager.instance.playerId];
                            GameManager.instance.mainWeaponItem[GameManager.instance.playerId] = selectedItemId;
                            GameManager.instance.inventoryItemsId[selectedId] = tempItemId;
                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                        }
                        else
                        {
                            help.Show(InventoryControlHelp.ActionType.WrongClass);
                        }
                    }
                    break;

                case ItemData.ItemType.Necklace:
                    tempItemId = GameManager.instance.necklaceItem[GameManager.instance.playerId];
                    GameManager.instance.necklaceItem[GameManager.instance.playerId] = selectedItemId;
                    GameManager.instance.inventoryItemsId[selectedId] = tempItemId;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                    break;

                case ItemData.ItemType.Shoes:
                    tempItemId = GameManager.instance.shoesItem[GameManager.instance.playerId];
                    GameManager.instance.shoesItem[GameManager.instance.playerId] = selectedItemId;
                    GameManager.instance.inventoryItemsId[selectedId] = tempItemId;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                    break;

                case ItemData.ItemType.Range:
                    tempItemId = GameManager.instance.rangeWeaponItem;
                    GameManager.instance.rangeWeaponItem = selectedItemId;
                    GameManager.instance.inventoryItemsId[selectedId] = tempItemId;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                    break;

                case ItemData.ItemType.Magic:
                    tempItemId = GameManager.instance.magicItem;
                    GameManager.instance.magicItem = selectedItemId;
                    GameManager.instance.inventoryItemsId[selectedId] = tempItemId;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                    break;

                case ItemData.ItemType.Potion:
                    if (Mathf.Abs(GameManager.instance.maxHealth - GameManager.instance.health) < 0.1f)
                    {
                        Debug.Log("이미 체력이 가득 차 있습니다.");
                        // 에러 메시지 띄우기
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                        help.Show(InventoryControlHelp.ActionType.FullHeart);
                    }
                    else
                    {
                        GameManager.instance.health = Mathf.Clamp(GameManager.instance.health +
                            ItemManager.Instance.itemDataArr[selectedItemId].baseAmount, 0, GameManager.instance.maxHealth);
                        GameManager.instance.inventoryItemsId[selectedId] = -1;
                        // 물약 마시는 소리 추가
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.Healthy);
                    }
                    break;
            }
            GameManager.instance.StatusUpdate();
        }
        else
        {
            // 장비 슬롯이 비어 있는 경우
            switch (selectedId)
            {
                case 24:
                    if (GameManager.instance.mainWeaponItem[GameManager.instance.playerId] == -1) return;
                    break;

                case 25:
                    if (GameManager.instance.necklaceItem[GameManager.instance.playerId] == -1) return;
                    break;

                case 26:
                    if (GameManager.instance.shoesItem[GameManager.instance.playerId] == -1) return;
                    break;

                case 27:
                    if (GameManager.instance.rangeWeaponItem == -1) return;
                    break;

                case 28:
                    if (GameManager.instance.magicItem == -1) return;
                    break;
            }

            int selectedSlot = -1;
            for (int i = 0; i < GameManager.instance.maxInventory; i++)
            {
                if (GameManager.instance.inventoryItemsId[i] == -1)
                {
                    selectedSlot = i;
                    break;
                }
            }
            // 인벤토리에 자리가 없는 경우
            if (selectedSlot == -1)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                help.Show(InventoryControlHelp.ActionType.FullMsg);
                return;
            }

            switch (selectedId)
            {
                case 24:
                    GameManager.instance.inventoryItemsId[selectedSlot] = GameManager.instance.mainWeaponItem[GameManager.instance.playerId];
                    GameManager.instance.mainWeaponItem[GameManager.instance.playerId] = -1;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Unequip);
                    break;

                case 25:
                    GameManager.instance.inventoryItemsId[selectedSlot] = GameManager.instance.necklaceItem[GameManager.instance.playerId];
                    GameManager.instance.necklaceItem[GameManager.instance.playerId] = -1;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Unequip);
                    break;

                case 26:
                    GameManager.instance.inventoryItemsId[selectedSlot] = GameManager.instance.shoesItem[GameManager.instance.playerId];
                    GameManager.instance.shoesItem[GameManager.instance.playerId] = -1;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Unequip);
                    break;

                case 27:
                    GameManager.instance.inventoryItemsId[selectedSlot] = GameManager.instance.rangeWeaponItem;
                    GameManager.instance.rangeWeaponItem = -1;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Unequip);
                    break;

                case 28:
                    GameManager.instance.inventoryItemsId[selectedSlot] = GameManager.instance.magicItem;
                    GameManager.instance.magicItem = -1;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Unequip);
                    break;
            }
            GameManager.instance.StatusUpdate();
        }
    }

    void InitLanguage()
    {
        Dictionary<string, string[]> nameDic = new();
        nameDic["Change Slot"] = new string[] { "자리 바꿀 슬롯 선택", "Choose Slot to Swap" };
        nameDic["Unlock2"] = new string[] { "<color=blue>창고</color>에서 잠금 해제 : <color=yellow>1000</color>   ", " Unlock in <color=blue>Storage</color> : <color=yellow>1000</color>   " };
        nameDic["Unlock3"] = new string[] { "<color=blue>창고</color>에서 잠금 해제 : <color=yellow>3000</color>   ", " Unlock in <color=blue>Storage</color> : <color=yellow>3000</color>   " };
        nameDic["Destroy Yes Label"] = new string[] { "예", "Yes" };
        nameDic["Destroy No Label"] = new string[] { "아니요", "No" };
        nameDic["Destroy Text"] = new string[] { "파괴", "Destroy" };
        nameDic["Close Text"] = new string[] { "창 닫기", "Close" };
        nameDic["Cancel Text"] = new string[] { "취소", "Cancel" };
        nameDic["Equipment Text"] = new string[] { "장비", "Equipment" };
        nameDic["Weapon Text"] = new string[] { "주무기", "Weapon" };
        nameDic["Necklace Text"] = new string[] { "목걸이", "Necklace" };
        nameDic["Shoes Text"] = new string[] { "신발", "Shoes" };
        nameDic["Range Text"] = new string[] { "투척", "Range" };
        nameDic["Magic Text"] = new string[] { "마법", "Magic" };


        var texts = transform.parent.GetComponentsInChildren<Text>(true);
        int textId = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? 0 : 1;
        foreach (var text in texts)
        {
            if (nameDic.ContainsKey(text.name))
            {
                text.text = nameDic[text.name][textId];
            }
        }
    }
}
