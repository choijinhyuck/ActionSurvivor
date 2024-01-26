using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [Serializable]
    public struct UpgradeInfo
    {
        public ItemData.Items before;
        public ItemData.Items after;
        public int goldRequired;
        public float probability;
    }

    public Canvas upgradeCanvas;
    public Text itemName;
    public Text itemDesc;
    public Text itemEffect;
    public Text upgradeConfirm;
    public Button confirmNo;
    public InventoryControlHelp help;
    public List<UpgradeInfo> upgradableItems;
    public GameObject hammering;
    public GameObject upgradeResult;

    [SerializeField] UpgradeNPC upgradeNPC;
    [SerializeField] Text npcDialogue;
    [SerializeField] Image beforeItemImg;
    [SerializeField] Image AfterItemImg;
    [SerializeField] Text probability;
    [SerializeField] Text goldRequired;
    Image beforeItemSlotImg;

    List<Button> buttons;
    List<Canvas> canvases;
    List<Image> itemImages;
    GameObject currentSelect;
    bool isUpgrade;
    int selectedId;
    GameObject selectedObjectOnDestroy;
    Color originAlpha;
    Color blankAlpha;
    Color halfAlpha;
    Color greenColor;
    Color redColor;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>(true).ToList<Button>();
        itemImages = new List<Image>();
        canvases = new List<Canvas>();
        originAlpha = new Color(1f, 1f, 1f, 1f);
        blankAlpha = new Color(1f, 1f, 1f, 0f);
        halfAlpha = new Color(1f, 1f, 1f, .5f);
        greenColor = new Color(.6f, 1f, .6f, 1f);
        redColor = new Color(1f, .4f, .4f, 1f);


        beforeItemSlotImg = beforeItemImg.transform.parent.GetComponent<Image>();
        beforeItemSlotImg.color = originAlpha;
        beforeItemImg.color = blankAlpha;
        AfterItemImg.color = blankAlpha;

        // buttons 0-23: Inventory
        for (int i = 0; i < buttons.Count; i++)
        {
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
        isUpgrade = false;

        if (upgradeConfirm.transform.parent.gameObject.activeSelf)
        {
            upgradeConfirm.transform.parent.gameObject.SetActive(false);
        }

        for (int i = 0; i < GameManager.instance.maxInventory; i++)
        {
            buttons[i].gameObject.SetActive(true);
        }

        ChangeAlpha();
        foreach (var canvas in canvases)
        {
            canvas.sortingOrder = 2;
        }
        EventSystem.current.SetSelectedGameObject(buttons[selectedId].gameObject);
        currentSelect = buttons[selectedId].gameObject;
        canvases[selectedId].sortingOrder = 3;
        upgradeCanvas.sortingOrder = 1;

        Init();
        InitLanguage();
    }

    private void LateUpdate()
    {
        InitLanguage();

        if (isUpgrade)
        {
            if (selectedObjectOnDestroy != EventSystem.current.currentSelectedGameObject)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonChange);
                selectedObjectOnDestroy = EventSystem.current.currentSelectedGameObject;
            }
            //  upgrade canvas sortig order =4 ����
            upgradeConfirm.transform.parent.GetComponent<Canvas>().sortingOrder = 4;
            upgradeCanvas.sortingOrder = 1;
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
        upgradeCanvas.sortingOrder = 1;

        selectedId = buttons.IndexOf(currentSelect.GetComponent<Button>());

        ShowHelp();

        Init();

        if (GameManager.instance.inventoryItemsId[selectedId] == -1)
        {
            itemName.text = "";
            itemDesc.text = "";
            itemEffect.text = "";

            npcDialogue.text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "�� �� \n\r��ȭ�� ����?" : "Upgrade What?";
            beforeItemImg.sprite = null;
            beforeItemImg.color = blankAlpha;
            beforeItemSlotImg.color = originAlpha;
            AfterItemImg.sprite = null;
            AfterItemImg.color = blankAlpha;

            if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
            {
                probability.text = string.Format("���� Ȯ��: <color=red>{0}</color> %", "-");
                goldRequired.text = string.Format("���: <color=blue>{0}</color> ���", "-");
            }
            else
            {
                probability.text = string.Format("Success Rate: <color=red>{0}</color> %", "-");
                goldRequired.text = string.Format("Cost: <color=blue>{0}</color> gold", "-");
            }
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
            

            if (CanUpgrade())
            {
                npcDialogue.text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "�� �� \n\r��ȭ�� ����?" : "Upgrade\n\rthe item?";
                beforeItemImg.sprite = ItemManager.Instance.itemDataArr[(int)upgradableItems[GetUpgradeIndex()].before].itemIcon;
                beforeItemImg.color = originAlpha;
                beforeItemSlotImg.color = greenColor;
                AfterItemImg.sprite = ItemManager.Instance.itemDataArr[(int)upgradableItems[GetUpgradeIndex()].after].itemIcon;
                AfterItemImg.color = halfAlpha;
                if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
                {
                    probability.text = string.Format("���� Ȯ��: <color=red>{0}</color> %", Mathf.FloorToInt(upgradableItems[GetUpgradeIndex()].probability * 100));
                    goldRequired.text = string.Format("���: <color=blue>{0}</color> ���", upgradableItems[GetUpgradeIndex()].goldRequired);
                }
                else
                {
                    probability.text = string.Format("Success Rate: <color=red>{0}</color> %", Mathf.FloorToInt(upgradableItems[GetUpgradeIndex()].probability * 100));
                    goldRequired.text = string.Format("Cost: <color=blue>{0}</color> gold", upgradableItems[GetUpgradeIndex()].goldRequired);
                }
            }
            else
            {
                npcDialogue.text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "�� ���� \n\r��ȭ �Ұ���" : "Can't be\n\rupgraded";
                beforeItemImg.sprite = ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[selectedId]].itemIcon;
                beforeItemImg.color = originAlpha;
                beforeItemSlotImg.color = redColor;
                AfterItemImg.sprite = null;
                AfterItemImg.color = blankAlpha;
                if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
                {
                    probability.text = string.Format("���� Ȯ��: <color=red>{0}</color> %", "-");
                    goldRequired.text = string.Format("���: <color=blue>{0}</color> ���", "-");
                }
                else
                {
                    probability.text = string.Format("Success Rate: <color=red>{0}</color> %", "-");
                    goldRequired.text = string.Format("Cost: <color=blue>{0}</color> gold", "-");
                }
            }
        }
    }

    void ShowHelp()
    {
        // ��Ʈ�� ����
        int itemId;

        itemId = GameManager.instance.inventoryItemsId[selectedId];

        if (itemId != -1)
        {
            help.Show(InventoryControlHelp.ActionType.Upgrade);
        }
        else
        {
            help.Show(InventoryControlHelp.ActionType.Empty);
        }
    }

    void Init()
    {
        for (int i = 0; i < GameManager.instance.maxInventory; i++)
        {
            if (GameManager.instance.inventoryItemsId[i] == -1)
            {
                EmptySlot(i);
            }
            else
            {
                itemImages[i].sprite = ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[i]].itemIcon;
                SlotAlpha(i);
            }
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].GetComponent<Image>().color = originAlpha;
        }
    }

    void EmptySlot(int id)
    {
        itemImages[id].sprite = null;
        itemImages[id].color = blankAlpha;
    }

    void SlotAlpha(int id)
    {
        itemImages[id].color = originAlpha;
    }

    bool CanUpgrade()
    {
        for (int i = 0; i < upgradableItems.Count; i++)
        {
            if (upgradableItems[i].before == (ItemData.Items)GameManager.instance.inventoryItemsId[selectedId])
            {
                return true;
            }
        }

        return false;
    }

    int GetUpgradeIndex()
    {
        for (int i = 0; i < upgradableItems.Count; i++)
        {
            if (upgradableItems[i].before == (ItemData.Items)GameManager.instance.inventoryItemsId[selectedId])
            {
                return i;
            }
        }
        Debug.Log("Try to get an Upgrade Index that can't be upgraded");
        return -1;

    }

    void OnPress(int buttonIndex)
    {
        if (GameManager.instance.inventoryItemsId[buttonIndex] == -1)
        {
            return;
        }
        else
        {
            if (CanUpgrade())
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
                UpgradeItem();
            }
            else
            {
                help.Show(InventoryControlHelp.ActionType.NotUpgradable);
            }
        }
    }

    public void UpgradeItem()
    {
        if (!gameObject.activeSelf) return;
        if (!GameManager.instance.workingInventory) return;

        EventSystem.current.SetSelectedGameObject(confirmNo.gameObject);
        selectedObjectOnDestroy = confirmNo.gameObject;
        isUpgrade = true;
        upgradeConfirm.transform.parent.gameObject.SetActive(true);

        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
        {
            upgradeConfirm.text = string.Format("<color=green>{0}</color> ��(��)\r\n<color=red>{1:N0} ���</color>�� {2}�Ͻðڽ��ϱ�?\r\n<color=red><size=6>(���� �� �������� �ı��˴ϴ�.)</size></color>",
        ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[selectedId]].itemName,
        upgradableItems[GetUpgradeIndex()].goldRequired, "��ȭ");
        }
        else
        {
            upgradeConfirm.text = string.Format("Want to {2} for <color=red>{1:N0} gold</color>?\r\n<color=green>{0}</color>\r\n<color=red><size=6>(Failure will result in destruction.)</size></color>",
        ItemManager.Instance.itemDataArr[GameManager.instance.inventoryItemsId[selectedId]].itemNameEng,
        upgradableItems[GetUpgradeIndex()].goldRequired, "Upgrade");
            
        }
        
    }

    public void OnConfirm(bool confirm)
    {
        if (confirm)
        {
            if (GameManager.instance.gold < upgradableItems[GetUpgradeIndex()].goldRequired)
            {
                help.Show(InventoryControlHelp.ActionType.NotEnoughMoney);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
                hammering.SetActive(true);
                return;
            }
        }
        else
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
        }

        upgradeConfirm.transform.parent.gameObject.SetActive(false);
        isUpgrade = false;
        EventSystem.current.SetSelectedGameObject(buttons[selectedId].gameObject);
    }

    public void FinishUpgrade()
    {
        GameManager.instance.gold -= upgradableItems[GetUpgradeIndex()].goldRequired;
        upgradeResult.SetActive(true);

        if (UnityEngine.Random.value < upgradableItems[GetUpgradeIndex()].probability)
        {
            GameManager.instance.inventoryItemsId[selectedId] = (int)upgradableItems[GetUpgradeIndex()].after;
            if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
            {
                upgradeResult.GetComponentInChildren<Text>().text = "<color=blue>�����մϴ�!</color>\r\n\r\n��ȭ�� �����߽��ϴ�";
            }
            else
            {
                upgradeResult.GetComponentInChildren<Text>().text = "<color=blue>Congratulations!</color>\r\n\r\nUpgraded Successfully!";
            }
            
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Success);
        }
        else
        {
            GameManager.instance.inventoryItemsId[selectedId] = -1;
            if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
            {
                upgradeResult.GetComponentInChildren<Text>().text = "��ȭ�� �����߽��ϴ�.\r\n\r\n<color=red>�������� �ı��ƽ��ϴ�.</color>";
            }
            else
            {
                upgradeResult.GetComponentInChildren<Text>().text = "Failed to Upgrade.\r\n\r\n<color=red>The item has been destroyed.</color>";
            }
            
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Destroy);
        }

        StopCoroutine("CloseResultCoroutine");
        StartCoroutine("CloseResultCoroutine");
    }

    IEnumerator CloseResultCoroutine()
    {
        yield return new WaitForSecondsRealtime(1f);

        isUpgrade = false;
        upgradeResult.SetActive(false);
        upgradeConfirm.transform.parent.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(buttons[selectedId].gameObject);
    }

    // �κ��丮 â���� �ƹ� ��ư�� ���õ��� ���� ��쿡 MenuŰ (Ű����: Esc, �����е�: Start)�� �������� �� �ֵ���.
    public void OnMenu()
    {
        if (!gameObject.activeSelf) return;

        if (GameManager.instance.workingInventory)
        {

            if (isUpgrade)
            {
                if (upgradeResult.activeSelf) return;
                isUpgrade = false;
                hammering.SetActive(false);
                upgradeConfirm.transform.parent.gameObject.SetActive(false);
                EventSystem.current.SetSelectedGameObject(buttons[selectedId].gameObject);
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
            }
            else
            {
                upgradeNPC.Open(UpgradeNPC.ActionType.Inventory);
            }

        }
    }

    public void OnCancel()
    {
        if (!gameObject.activeSelf) return;

        if (GameManager.instance.workingInventory)
        {
            if (isUpgrade)
            {
                if (upgradeResult.activeSelf) return;
                isUpgrade = false;
                hammering.SetActive(false);
                upgradeConfirm.transform.parent.gameObject.SetActive(false);
                EventSystem.current.SetSelectedGameObject(buttons[selectedId].gameObject);
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
            }
            else
            {
                //storageChest.Open(StorageChest.ActionType.Inventory);
            }
        }
    }

    void ChangeAlpha()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (GameManager.instance.inventoryItemsId[i] == -1)
            {
                itemImages[i].color = blankAlpha;
                continue;
            }
            else
            {
                itemImages[i].color = originAlpha;
            }
        }
    }

    void InitLanguage()
    {
        Dictionary<string, string[]> nameDic = new();
        nameDic["After Label"] = new string[] { "��ȭ", "After" };
        nameDic["Present Label"] = new string[] { "����", "Now" };
        nameDic["Close Text"] = new string[] { "â �ݱ�", "Close" };
        nameDic["Unlock2"] = new string[] { "<color=blue>â��</color>���� ��� ���� : <color=yellow>1000</color>   ", " Unlock in <color=blue>Storage</color> : <color=yellow>1000</color>   " };
        nameDic["Unlock3"] = new string[] { "<color=blue>â��</color>���� ��� ���� : <color=yellow>3000</color>   ", " Unlock in <color=blue>Storage</color> : <color=yellow>3000</color>   " };
        nameDic["Upgrade Title"] = new string[] { "��ȭ", "Upgrade" };
        nameDic["Hammering Text"] = new string[] { "��ȭ ��...", "Upgrading..." };
        nameDic["Yes Label"] = new string[] { "��", "Yes" };
        nameDic["No Label"] = new string[] { "�ƴϿ�", "No" };


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
