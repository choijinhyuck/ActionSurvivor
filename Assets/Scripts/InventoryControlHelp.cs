using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryControlHelp : MonoBehaviour
{
    public enum ActionType
    {
        Empty, Equip, UnEquip, Pressed, Destroy, Use, FullMsg, FullHeart, NotEquippable, WrongPosition, WrongItem,
        ToStorage, ToInventory, ToFullStorageMsg, ToFullInventory, Unlock, NotEnoughMoney, Buy, Sell, Upgrade, NotUpgradable,
        WrongClass,
    }

    public GameObject message;
    public GameObject changeSlot;
    public GameObject unequip;
    public GameObject equipUse;
    public GameObject destroy;
    public GameObject cancel;
    public GameObject select;
    public GameObject close;
    public Sprite[] keyboard;
    public Sprite[] gamepad;

    GameObject[] objArr;
    Coroutine messageCoroutine;
    bool showMessage;
    ControllerManager.scheme currentScheme;


    private void Awake()
    {
        objArr = new GameObject[] { changeSlot, unequip, equipUse, destroy, cancel, select, close };
        currentScheme = ControllerManager.scheme.Undefined;
    }

    private void OnEnable()
    {
        foreach (var obj in objArr)
        {
            if (obj == close)
            {
                if (!obj.activeSelf) obj.SetActive(true);
                continue;
            }
            else if (obj.activeSelf) obj.SetActive(false);
        }
        if (message.activeSelf) message.SetActive(false);
        showMessage = false;
        messageCoroutine = null;
    }

    private void Update()
    {
        if (currentScheme == ControllerManager.instance.CurrentScheme) return;
        currentScheme = ControllerManager.instance.CurrentScheme;
        switch (currentScheme)
        {
            case ControllerManager.scheme.Keyboard:
                select.GetComponentInChildren<Image>().sprite = keyboard[0];
                cancel.GetComponentInChildren<Image>().sprite = keyboard[1];
                equipUse.GetComponentInChildren<Image>().sprite = keyboard[2];
                unequip.GetComponentInChildren<Image>().sprite = keyboard[2];
                destroy.GetComponentInChildren<Image>().sprite = keyboard[3];
                close.GetComponentInChildren<Image>().sprite = keyboard[4];
                break;

            case ControllerManager.scheme.Gamepad:
                select.GetComponentInChildren<Image>().sprite = gamepad[0];
                cancel.GetComponentInChildren<Image>().sprite = gamepad[1];
                equipUse.GetComponentInChildren<Image>().sprite = gamepad[2];
                unequip.GetComponentInChildren<Image>().sprite = gamepad[2];
                destroy.GetComponentInChildren<Image>().sprite = gamepad[3];
                close.GetComponentInChildren<Image>().sprite = gamepad[4];
                break;

            default:
                Debug.Log("Undefined Control Scheme!");
                break;
        }

    }

    public void Show(ActionType actionType)
    {
        if (actionType == ActionType.Buy)
        {
            select.GetComponentInChildren<Text>().text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "����" : "Buy";
        }
        else if (actionType == ActionType.Sell)
        {
            select.GetComponentInChildren<Text>().text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "�Ǹ�" : "Sell";
        }
        else if (actionType == ActionType.Upgrade)
        {
            select.GetComponentInChildren<Text>().text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "��ȭ" : "Upgrade";
        }
        else
        {
            select.GetComponentInChildren<Text>().text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "����" : "Select";
        }

        switch (actionType)
        {
            case ActionType.Empty:
                Filter(new List<GameObject> { close });
                break;

            case ActionType.Equip:
                equipUse.GetComponentInChildren<Text>().text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "����" : "Equip";
                Filter(new List<GameObject> { close, select, equipUse, destroy });
                break;

            case ActionType.UnEquip:
                Filter(new List<GameObject> { close, select, unequip });
                break;

            case ActionType.Pressed:
                Filter(new List<GameObject> { close, select, cancel, changeSlot });
                break;

            case ActionType.Destroy:
                Filter(new List<GameObject> { close, select, destroy });
                break;

            case ActionType.Use:
                equipUse.GetComponentInChildren<Text>().text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "���" : "Use";
                Filter(new List<GameObject> { close, select, equipUse, destroy });
                break;

            case ActionType.FullMsg:
                if (showMessage) StopCoroutine(messageCoroutine);
                messageCoroutine = StartCoroutine(Message(ActionType.FullMsg));
                break;

            case ActionType.FullHeart:
                if (showMessage) StopCoroutine(messageCoroutine);
                messageCoroutine = StartCoroutine(Message(ActionType.FullHeart));
                break;

            case ActionType.NotEquippable:
                if (showMessage) StopCoroutine(messageCoroutine);
                messageCoroutine = StartCoroutine(Message(ActionType.NotEquippable));
                break;

            case ActionType.WrongPosition:
                if (showMessage) StopCoroutine(messageCoroutine);
                messageCoroutine = StartCoroutine(Message(ActionType.WrongPosition));
                break;

            case ActionType.WrongItem:
                if (showMessage) StopCoroutine(messageCoroutine);
                messageCoroutine = StartCoroutine(Message(ActionType.WrongItem));
                break;

            case ActionType.ToFullStorageMsg:
                if (showMessage) StopCoroutine(messageCoroutine);
                messageCoroutine = StartCoroutine(Message(ActionType.ToFullStorageMsg));
                break;

            case ActionType.ToFullInventory:
                if (showMessage) StopCoroutine(messageCoroutine);
                messageCoroutine = StartCoroutine(Message(ActionType.ToFullInventory));
                break;

            case ActionType.NotEnoughMoney:
                if (showMessage) StopCoroutine(messageCoroutine);
                messageCoroutine = StartCoroutine(Message(ActionType.NotEnoughMoney));
                break;

            case ActionType.NotUpgradable:
                if (showMessage) StopCoroutine(messageCoroutine);
                messageCoroutine = StartCoroutine(Message(ActionType.NotUpgradable));
                break;

            case ActionType.WrongClass:
                if (showMessage) StopCoroutine(messageCoroutine);
                messageCoroutine = StartCoroutine(Message(ActionType.WrongClass));
                break;

            case ActionType.ToStorage:
                equipUse.GetComponentInChildren<Text>().text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "����" : "Store";
                Filter(new List<GameObject> { close, select, equipUse, destroy });
                break;

            case ActionType.ToInventory:
                equipUse.GetComponentInChildren<Text>().text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "ã��" : "Retrieve";
                Filter(new List<GameObject> { close, select, equipUse, destroy });
                break;

            case ActionType.Unlock:
                Filter(new List<GameObject> { close, select });
                break;

            case ActionType.Buy:
                Filter(new List<GameObject> { close, select });
                break;

            case ActionType.Sell:
                Filter(new List<GameObject> { close, select });
                break;

            case ActionType.Upgrade:
                Filter(new List<GameObject> { close, select });
                break;
        }
    }

    void Filter(List<GameObject> showObjects)
    {
        foreach (var obj in objArr)
        {
            if (showObjects.Contains(obj))
            {
                if (!obj.activeSelf) obj.SetActive(true);
            }
            else
            {
                if (obj.activeSelf) obj.SetActive(false);
            }
        }
    }

    IEnumerator Message(ActionType actionType)
    {
        showMessage = true;
        switch (actionType)
        {
            case ActionType.FullMsg:
                if (!message.activeSelf) message.SetActive(true);
                message.GetComponent<Text>().text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "�κ��丮 ������ �����մϴ�." : "Intentory is Full.";
                break;

            case ActionType.FullHeart:
                if (!message.activeSelf) message.SetActive(true);
                message.GetComponent<Text>().text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "�̹� ü���� ���� �� �ֽ��ϴ�." : "Heart is already Full.";
                break;

            case ActionType.NotEquippable:
                if (!message.activeSelf) message.SetActive(true);
                message.GetComponent<Text>().text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "������ �� ���� ĳ�����Դϴ�." : "The character is unable to equip the item.";
                break;

            case ActionType.WrongPosition:
                if (!message.activeSelf) message.SetActive(true);
                message.GetComponent<Text>().text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "������ �� ���� �����Դϴ�." : "Can't be equipped on this slot.";
                break;

            case ActionType.WrongItem:
                if (!message.activeSelf) message.SetActive(true);
                message.GetComponent<Text>().text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "������ �� ���� ����Դϴ�." : "The Item can't be equipped.";
                break;

            case ActionType.ToFullInventory:
                if (!message.activeSelf) message.SetActive(true);
                message.GetComponent<Text>().text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "�κ��丮 ������ �����մϴ�." : "Inventory is Full.";
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                break;

            case ActionType.ToFullStorageMsg:
                if (!message.activeSelf) message.SetActive(true);
                message.GetComponent<Text>().text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "â�� ������ �����մϴ�." : "Storage is Full.";
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                break;

            case ActionType.NotEnoughMoney:
                if (!message.activeSelf) message.SetActive(true);
                message.GetComponent<Text>().text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "�������� �����մϴ�." : "Not enough gold";
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                break;

            case ActionType.NotUpgradable:
                if (!message.activeSelf) message.SetActive(true);
                message.GetComponent<Text>().text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "��ȭ�� �� ���� �������Դϴ�" : "The item can't be upgraded.";
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                break;

            case ActionType.WrongClass:
                if (!message.activeSelf) message.SetActive(true);
                message.GetComponent<Text>().text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "������ �� ���� ĳ�����Դϴ�." : "The character is unable to equip the item.";
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
                break;

            default:
                Debug.Log("Message�� �߸��� ����");
                break;
        }

        yield return new WaitForSecondsRealtime(2f);
        message.SetActive(false);
        showMessage = false;
    }
}
