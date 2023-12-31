using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageUI : MonoBehaviour
{
    public Canvas baseUI;
    public Text itemName;
    public Text itemDesc;
    public Text itemEffect;
    public Text destroyDesc;
    public Button confirmNo;
    public InventoryControlHelp help;

    [SerializeField]
    StorageChest storageChest;

    List<Button> buttons;
    List<Canvas> canvases;
    List<Image> itemImages;
    GameObject currentSelect;
    bool isDestroying;
    bool isPressed;
    bool isUnlocking;
    int pressedId;
    int selectedId;
    GameObject selectedObjectOnDestroy;
    Color originAlpha;
    Color blankAlpha;
    Color halfAlpha;
    Color grayColor;
    Color greenColor;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>(true).ToList<Button>();
        itemImages = new List<Image>();
        canvases = new List<Canvas>();
        originAlpha = new Color(1f, 1f, 1f, 1f);
        blankAlpha = new Color(1f, 1f, 1f, 0f);
        halfAlpha = new Color(1f, 1f, 1f, .5f);
        grayColor = new Color(.7f, .7f, .7f, 1f);
        greenColor = new Color(.6f, 1f, .6f, 1f);

        // buttons 0-23: Inventory, 24-47: Storage, 48-49: Lock
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
            canvases.Add(buttons[i].GetComponentInParent<Canvas>());
            itemImages.Add(buttons[i].GetComponentsInChildren<Image>()[1]);
            int temp = i;
            buttons[i].onClick.AddListener(() => OnPress(temp));
        }
        selectedId = 0;
    }

    void OnEnable()
    {
        isDestroying = false;
        isUnlocking = false;

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
        switch (GameManager.Instance.maxInventory)
        {
            case 8:
                buttons[48].gameObject.SetActive(true);
                buttons[48].interactable = true;
                buttons[49].gameObject.SetActive(true);
                buttons[49].interactable = false;
                break;

            case 16:
                buttons[48].gameObject.SetActive(false);
                buttons[49].gameObject.SetActive(true);
                buttons[49].interactable = true;
                int[] targetInts = new int[] {0, 1};
                foreach (int targetInt in targetInts)
                {
                    Navigation navigation = buttons[targetInt].navigation;
                    navigation.mode = Navigation.Mode.Automatic;
                    buttons[targetInt].navigation = navigation;
                }
                break;

            case 24:
                buttons[48].gameObject.SetActive(false);
                buttons[49].gameObject.SetActive(false);
                targetInts = new int[] { 0, 1, 8, 9};
                foreach (int targetInt in targetInts)
                {
                    Navigation navigation = buttons[targetInt].navigation;
                    navigation.mode = Navigation.Mode.Automatic;
                    buttons[targetInt].navigation = navigation;
                }
                break;
        }

        ChangeAlpha(originAlpha);
        foreach (var canvas in canvases)
        {
            canvas.sortingOrder = 2;
        }
        EventSystem.current.SetSelectedGameObject(buttons[selectedId].gameObject);
        currentSelect = buttons[selectedId].gameObject;
        canvases[selectedId].sortingOrder = 3;
        baseUI.sortingOrder = 1;

        Init();
    }

    private void LateUpdate()
    {

        if (isDestroying)
        {
            if (selectedObjectOnDestroy != EventSystem.current.currentSelectedGameObject)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonChange);
                selectedObjectOnDestroy = EventSystem.current.currentSelectedGameObject;
            }
            //  destroy canvas sortig order =4 으로
            destroyDesc.transform.parent.GetComponent<Canvas>().sortingOrder = 4;
            baseUI.sortingOrder = 1;
            return;
        }

        if (buttons[selectedId].gameObject != EventSystem.current.currentSelectedGameObject)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonChange);
        }


        currentSelect.GetComponentInParent<Canvas>().sortingOrder = 2;
        currentSelect = EventSystem.current.currentSelectedGameObject;

        if (currentSelect is null) return;
        currentSelect.GetComponentInParent<Canvas>().sortingOrder = 3;
        baseUI.sortingOrder = 1;

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
        else if (selectedId < 48)
        {
            if (GameManager.Instance.storedItemsId[selectedId % 24] == -1)
            {
                itemName.text = "";
                itemDesc.text = "";
                itemEffect.text = "";
            }
            else
            {
                itemName.text = ItemManager.Instance.itemDataArr[GameManager.Instance.storedItemsId[selectedId % 24]].itemName;
                itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.Instance.storedItemsId[selectedId % 24]].itemDesc;
                itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.Instance.storedItemsId[selectedId % 24]].itemEffect;
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
        else if (selectedId > 47)
        {
            help.Show(InventoryControlHelp.ActionType.Unlock);
        }
        else
        {
            int itemId;
            if (selectedId < 24)
            {
                itemId = GameManager.Instance.inventoryItemsId[selectedId];

                if (itemId != -1)
                {
                    help.Show(InventoryControlHelp.ActionType.ToStorage);
                }
            }
            else
            {
                itemId = GameManager.Instance.storedItemsId[selectedId % 24];

                if (itemId != -1)
                {
                    help.Show(InventoryControlHelp.ActionType.ToInventory);
                }
            }

            if (itemId == -1)
            {
                help.Show(InventoryControlHelp.ActionType.Empty);
            }
        }
    }


    public void OnStore()
    {
        if (!gameObject.activeSelf) return;
        if (isPressed) return;
        if (isDestroying) return;
        if (!GameManager.Instance.workingInventory) return;
        if (selectedId > 47) return;
        if (selectedId < 24)
        {
            if (GameManager.Instance.inventoryItemsId[selectedId] == -1) return;
        }
        else
        {
            if (GameManager.Instance.storedItemsId[selectedId % 24] == -1) return;
        }

        if (selectedId < 24)
        {
            bool isEmpty = false;
            for (int i = 0; i < GameManager.Instance.storedItemsId.Length; i++)
            {
                if (GameManager.Instance.storedItemsId[i] == -1)
                {
                    GameManager.Instance.storedItemsId[i] = GameManager.Instance.inventoryItemsId[selectedId];
                    GameManager.Instance.inventoryItemsId[selectedId] = -1;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                    isEmpty = true;
                    break;
                }
            }
            Debug.Log(isEmpty);
            if (!isEmpty)
            {
                Debug.Log("full");
                help.Show(InventoryControlHelp.ActionType.ToFullStorageMsg);
            }

        }
        else
        {
            bool isEmpty = false;
            for (int i = 0; i < GameManager.Instance.maxInventory; i++)
            {
                if (GameManager.Instance.inventoryItemsId[i] == -1)
                {
                    GameManager.Instance.inventoryItemsId[i] = GameManager.Instance.storedItemsId[selectedId % 24];
                    GameManager.Instance.storedItemsId[selectedId % 24] = -1;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                    isEmpty = true;
                    break;
                }
            }
            Debug.Log(isEmpty);
            if (!isEmpty)
            {
                Debug.Log("full");
                help.Show(InventoryControlHelp.ActionType.ToFullInventory);
            }

        }
    }

    public void DestroyItem()
    {
        if (!gameObject.activeSelf) return;
        if (isPressed) return;
        if (isUnlocking) return;
        if (!GameManager.Instance.workingInventory) return;
        if (selectedId > 47) return;
        if (selectedId < 24)
        {
            if (GameManager.Instance.inventoryItemsId[selectedId] == -1) return;
        }
        else
        {
            if(GameManager.Instance.storedItemsId[selectedId % 24] == -1) return;
        }

        // 위 조건에 해당되지 않으면 파괴 버튼 도움말 팝업 띄우기

        EventSystem.current.SetSelectedGameObject(confirmNo.gameObject);
        selectedObjectOnDestroy = confirmNo.gameObject;
        isDestroying = true;
        destroyDesc.transform.parent.gameObject.SetActive(true);
        if (selectedId < 24)
        {
            destroyDesc.text = string.Format("<color=green>{0}</color>\r\n을(를) 정말 <color=red>파괴</color>하시겠습니까?",
            ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemName);
        }
        else
        {
            destroyDesc.text = string.Format("<color=green>{0}</color>\r\n을(를) 정말 <color=red>파괴</color>하시겠습니까?",
            ItemManager.Instance.itemDataArr[GameManager.Instance.storedItemsId[selectedId % 24]].itemName);
        }
    }

    public void OnConfirm(bool confirm)
    {
        if (confirm)
        {
            if (isUnlocking)
            {
                switch (selectedId)
                {
                    case 48:
                        GameManager.Instance.maxInventory = 16;
                        buttons[48].gameObject.SetActive(false);
                        buttons[49].interactable = true;
                        int[] targetInts = new int[] { 0, 1};
                        foreach (int targetInt in targetInts)
                        {
                            Navigation navigation = buttons[targetInt].navigation;
                            navigation.mode = Navigation.Mode.Automatic;
                            buttons[targetInt].navigation = navigation;
                        }
                        GameManager.Instance.gold -= 1000;
                        break;

                    case 49:
                        GameManager.Instance.maxInventory = 24;
                        buttons[49].gameObject.SetActive(false);
                        targetInts = new int[] { 0, 1, 8, 9 };
                        foreach (int targetInt in targetInts)
                        {
                            Navigation navigation = buttons[targetInt].navigation;
                            navigation.mode = Navigation.Mode.Automatic;
                            buttons[targetInt].navigation = navigation;
                        }
                        GameManager.Instance.gold -= 3000;
                        break;
                }
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Equip);
                for (int i = 0; i < GameManager.Instance.maxInventory; i++)
                {
                    buttons[i].gameObject.SetActive(true);
                }
                selectedId = 0;
            }
            else
            {
                if (selectedId < 24)
                {
                    GameManager.Instance.inventoryItemsId[selectedId] = -1;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Destroy);
                }
                else
                {
                    GameManager.Instance.storedItemsId[selectedId % 24] = -1;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Destroy);
                }
            }
            
        }
        else
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
        }
        destroyDesc.transform.parent.gameObject.SetActive(false);
        isDestroying = false;
        isUnlocking = false;
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

        for (int i = 24; i < 48; i++)
        {
            if (GameManager.Instance.storedItemsId[i % 24] == -1)
            {
                emptySlot(i);
            }
            else
            {
                itemImages[i].sprite = ItemManager.Instance.itemDataArr[GameManager.Instance.storedItemsId[i % 24]].itemIcon;
                slotAlpha(i);
            }
        }


        for (int i = 0; i < buttons.Count - 2; i++)
        {
            if (isPressed)
            {
                if (i == pressedId)
                {
                    buttons[i].GetComponent<Image>().color = grayColor;
                }
                else if (i == selectedId)
                {
                    buttons[i].GetComponent<Image>().color = greenColor;
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
                else if (buttonIndex < 48)
                {
                    int tempItemId = GameManager.Instance.inventoryItemsId[pressedId];
                    GameManager.Instance.inventoryItemsId[pressedId] = GameManager.Instance.storedItemsId[buttonIndex % 24];
                    GameManager.Instance.storedItemsId[buttonIndex % 24] = tempItemId;
                    isPressed = false;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
                    return;
                }
            }
            else if (pressedId < 48)
            {
                if (buttonIndex < 24)
                {
                    int tempItemId = GameManager.Instance.storedItemsId[pressedId % 24];
                    GameManager.Instance.storedItemsId[pressedId % 24] = GameManager.Instance.inventoryItemsId[buttonIndex];
                    GameManager.Instance.inventoryItemsId[buttonIndex] = tempItemId;
                    isPressed = false;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
                    return;
                }
                else if (buttonIndex < 48)
                {
                    int tempItemId = GameManager.Instance.storedItemsId[pressedId % 24];
                    GameManager.Instance.storedItemsId[pressedId % 24] = GameManager.Instance.storedItemsId[buttonIndex % 24];
                    GameManager.Instance.storedItemsId[buttonIndex % 24] = tempItemId;
                    isPressed = false;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
                    return;
                }
            }
        }

        if (buttonIndex < 24)
        {
            if (GameManager.Instance.inventoryItemsId[buttonIndex] == -1)
            {
                return;
            }
        }
        else if (buttonIndex < 48)
        {
            if (GameManager.Instance.storedItemsId[buttonIndex % 24] == -1)
            {
                return;
            }
        }
        else
        {
            OnUnlock();
            return;
        }
        

        AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
        pressedId = buttonIndex;
        isPressed = true;
        ChangeAlpha(halfAlpha);
        itemImages[buttonIndex].color = originAlpha;
        buttons[buttonIndex].GetComponent<Image>().color = grayColor;
    }

    
    void OnUnlock()
    {
        if (selectedId == 48)
        {
            if (GameManager.Instance.gold < 1000)
            {
                help.Show(InventoryControlHelp.ActionType.NotEnoughMoney);
                return;
            }
        }
        else if (selectedId == 49)
        {
            if (GameManager.Instance.gold < 3000)
            {
                help.Show(InventoryControlHelp.ActionType.NotEnoughMoney);
                return;
            }
        }
        EventSystem.current.SetSelectedGameObject(confirmNo.gameObject);
        selectedObjectOnDestroy = confirmNo.gameObject;
        isDestroying = true;
        isUnlocking = true;
        destroyDesc.transform.parent.gameObject.SetActive(true);

        destroyDesc.text = "정말 인벤토리 <color=red>8칸</color>을 확장 하시겠습니까?";
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
                    isUnlocking = false;
                    destroyDesc.transform.parent.gameObject.SetActive(false);
                    EventSystem.current.SetSelectedGameObject(buttons[selectedId].gameObject);
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
                }
                else
                {
                    storageChest.Open(StorageChest.ActionType.Inventory);
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
                isUnlocking = false;
                destroyDesc.transform.parent.gameObject.SetActive(false);
                EventSystem.current.SetSelectedGameObject(buttons[selectedId].gameObject);
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
            }
            else
            {
                //storageChest.Open(StorageChest.ActionType.Inventory);
            }
        }
    }

    void ChangeAlpha(Color targetColor)
    {
        for (int i = 0; i < buttons.Count - 2; i++)
        {
            if (i < 24)
            {
                if (GameManager.Instance.inventoryItemsId[i] == -1)
                {
                    itemImages[i].color = blankAlpha;
                    continue;
                }
            }
            else if (i < 48)
            {
                if (GameManager.Instance.storedItemsId[i % 24] == -1)
                {
                    itemImages[i].color = blankAlpha;
                    continue;
                }
            }
            itemImages[i].color = targetColor;
        }
    }
}
