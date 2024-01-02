using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public Canvas shopCanvas;
    public Text itemName;
    public Text itemDesc;
    public Text itemEffect;
    public Text buySellConfirm;
    public Button confirmNo;
    public InventoryControlHelp help;

    [SerializeField]
    ShopNPC shopNPC;
    [SerializeField]
    Text npcDialogue;

    List<Button> buttons;
    List<Canvas> canvases;
    List<Image> itemImages;
    int[] shopItems;
    GameObject currentSelect;
    bool isBuySell;
    int selectedId;
    GameObject selectedObjectOnDestroy;
    Color originAlpha;
    Color blankAlpha;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>(true).ToList<Button>();
        itemImages = new List<Image>();
        canvases = new List<Canvas>();
        originAlpha = new Color(1f, 1f, 1f, 1f);
        blankAlpha = new Color(1f, 1f, 1f, 0f);

        shopItems = new int[24] {   0,  1,  2,
                                    3,  4,  5,
                                    -1, -1, -1,
                                    18, 19, 20,
                                    6,  7,  8,
                                    21, 22, 23,
                                    9,  10, 11,
                                    15, 16, 17, };

        // buttons 0-23: Inventory, 24-55: Shop,
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
        isBuySell = false;

        if (buySellConfirm.transform.parent.gameObject.activeSelf)
        {
            buySellConfirm.transform.parent.gameObject.SetActive(false);
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
        shopCanvas.sortingOrder = 0;

        Init();
    }

    private void LateUpdate()
    {
        if (isBuySell)
        {
            if (selectedObjectOnDestroy != EventSystem.current.currentSelectedGameObject)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonChange);
                selectedObjectOnDestroy = EventSystem.current.currentSelectedGameObject;
            }
            //  destroy canvas sortig order =3 으로
            buySellConfirm.transform.parent.GetComponent<Canvas>().sortingOrder = 3;
            shopCanvas.sortingOrder = 0;
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
        shopCanvas.sortingOrder = 0;

        selectedId = buttons.IndexOf(currentSelect.GetComponent<Button>());

        ShowHelp();

        Init();

        if (selectedId < 24)
        {
            if (GameManager.Instance.inventoryItemsId[selectedId] == -1)
            {
                itemName.text = "";
                itemDesc.text = "";
                itemEffect.text = "";

                npcDialogue.text = "먼 템 팔 거임?";
            }
            else
            {
                itemName.text = ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemName;
                itemDesc.text = ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemDesc;
                itemEffect.text = ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemEffect;

                npcDialogue.text = $"<color=blue>{Mathf.FloorToInt(ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].priceToBuy / 5)} 골드</color>에 삼~\r\n팔 거임?";
            }
        }
        else if (selectedId < buttons.Count)
        {
            if (shopItems[selectedId - 24] == -1)
            {
                itemName.text = "";
                itemDesc.text = "";
                itemEffect.text = "";

                npcDialogue.text = "먼 템 살 거임?";
            }
            else
            {
                itemName.text = ItemManager.Instance.itemDataArr[shopItems[selectedId - 24]].itemName;
                itemDesc.text = ItemManager.Instance.itemDataArr[shopItems[selectedId - 24]].itemDesc;
                itemEffect.text = ItemManager.Instance.itemDataArr[shopItems[selectedId - 24]].itemEffect;

                npcDialogue.text = $"<color=blue>{ItemManager.Instance.itemDataArr[shopItems[selectedId - 24]].priceToBuy} 골드</color>에 팜~\r\n살 거임?";
            }
        }
    }

    void ShowHelp()
    {
        // 컨트롤 도움말
        int itemId;
        
        //판매
        if (selectedId < 24)
        {
            itemId = GameManager.Instance.inventoryItemsId[selectedId];

            if (itemId != -1)
            {
                help.Show(InventoryControlHelp.ActionType.Sell);
            }
        }
        //구매
        else
        {
            itemId = shopItems[selectedId - 24];

            if (itemId != -1)
            {
                help.Show(InventoryControlHelp.ActionType.Buy);
            }
        }

        if (itemId == -1)
        {
            help.Show(InventoryControlHelp.ActionType.Empty);
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

        for (int i = 0; i < shopItems.Length; i++)
        {
            if (shopItems[i] == -1)
            {
                emptySlot(i + 24);
            }
            else
            {
                itemImages[i + 24].sprite = ItemManager.Instance.itemDataArr[shopItems[i]].itemIcon;
                slotAlpha(i + 24);
            }
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].GetComponent<Image>().color = originAlpha;
        }
    }

    void emptySlot(int id)
    {
        itemImages[id].sprite = null;
        itemImages[id].color = blankAlpha;
    }

    void slotAlpha(int id)
    {
        itemImages[id].color = originAlpha;
    }

    void OnPress(int buttonIndex)
    {
        // 인벤토리 아이템 누름 (판매)
        if (buttonIndex < 24)
        {
            if (GameManager.Instance.inventoryItemsId[buttonIndex] == -1)
            {
                return;
            }

        }
        // 상점 아이템 누름 (구매)
        else
        {
            if (shopItems[buttonIndex - 24] == -1)
            {
                return;
            }

        }

        BuySellItem();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
    }

    public void BuySellItem()
    {
        if (!gameObject.activeSelf) return;
        if (!GameManager.Instance.workingInventory) return;

        // 위 조건에 해당되지 않으면 파괴 버튼 도움말 팝업 띄우기

        EventSystem.current.SetSelectedGameObject(confirmNo.gameObject);
        selectedObjectOnDestroy = confirmNo.gameObject;
        isBuySell = true;
        buySellConfirm.transform.parent.gameObject.SetActive(true);
        if (selectedId < 24)
        {
            buySellConfirm.text = string.Format("<color=green>{0}</color> 을(를)\r\n<color=red>{1:N0} 골드</color>에 {2}하시겠습니까?",
            ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].itemName,
            Mathf.FloorToInt(ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].priceToBuy / 5), "판매");
        }
        else
        {
            buySellConfirm.text = string.Format("<color=green>{0}</color> 을(를)\r\n<color=red>{1:N0} 골드</color>에 {2}하시겠습니까",
            ItemManager.Instance.itemDataArr[shopItems[selectedId - 24]].itemName,
            ItemManager.Instance.itemDataArr[shopItems[selectedId - 24]].priceToBuy, "구매");
        }
    }

    public void OnConfirm(bool confirm)
    {
        if (confirm)
        {
            if (selectedId < 24)
            {
                GameManager.Instance.gold += Mathf.FloorToInt(ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[selectedId]].priceToBuy / 5);
                GameManager.Instance.inventoryItemsId[selectedId] = -1;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Gold);
            }
            else
            {
                if (GameManager.Instance.gold < Mathf.FloorToInt(ItemManager.Instance.itemDataArr[shopItems[selectedId - 24]].priceToBuy))
                {
                    help.Show(InventoryControlHelp.ActionType.NotEnoughMoney);
                }
                else
                {
                    int emptySlotId = -1;
                    for (int i = 0; GameManager.Instance.maxInventory > i; i++)
                    {
                        if (GameManager.Instance.inventoryItemsId[i] == -1)
                        {
                            emptySlotId = i;
                            break;
                        }
                    }

                    //인벤토리 공간이 부족하면
                    if (emptySlotId == -1)
                    {
                        help.Show(InventoryControlHelp.ActionType.ToFullInventory);
                    }
                    else
                    {
                        GameManager.Instance.gold -= Mathf.FloorToInt(ItemManager.Instance.itemDataArr[shopItems[selectedId - 24]].priceToBuy);
                        GameManager.Instance.inventoryItemsId[emptySlotId] = shopItems[selectedId - 24];
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.Gold);
                    }
                }
            }
        }
        else
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
        }

        buySellConfirm.transform.parent.gameObject.SetActive(false);
        isBuySell = false;
        EventSystem.current.SetSelectedGameObject(buttons[selectedId].gameObject);
    }

    // 인벤토리 창에서 아무 버튼도 선택되지 않은 경우에 Menu키 (키보드: Esc, 게임패드: Start)로 빠져나올 수 있도록.
    public void OnMenu()
    {
        if (!gameObject.activeSelf) return;

        if (GameManager.Instance.workingInventory)
        {

                if (isBuySell)
                {
                    isBuySell = false;
                    buySellConfirm.transform.parent.gameObject.SetActive(false);
                    EventSystem.current.SetSelectedGameObject(buttons[selectedId].gameObject);
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
                }
                else
                {
                    shopNPC.Open(ShopNPC.ActionType.Inventory);
                }
            
        }
    }

    public void OnCancel()
    {
        if (!gameObject.activeSelf) return;

        if (GameManager.Instance.workingInventory)
        {
            if (isBuySell)
            {
                isBuySell = false;
                buySellConfirm.transform.parent.gameObject.SetActive(false);
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
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i < 24)
            {
                if (GameManager.Instance.inventoryItemsId[i] == -1)
                {
                    itemImages[i].color = blankAlpha;
                    continue;
                }
            }
            else
            {
                if (shopItems[i - 24] == -1)
                {
                    itemImages[i].color = blankAlpha;
                    continue;
                }
            }
            itemImages[i].color = targetColor;
        }
    }
}
