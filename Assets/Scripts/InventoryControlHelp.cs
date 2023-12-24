using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryControlHelp : MonoBehaviour
{
    public enum ActionType { Empty, Equip, UnEquip, Pressed, Destroy, Use, FullMsg, FullHeart, NotEquippable, WrongPosition, WrongItem }

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
        switch (actionType)
        {
            case ActionType.Empty:
                Filter(new List<GameObject> { close });
                break;

            case ActionType.Equip:
                equipUse.GetComponentInChildren<Text>().text = "����";
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
                equipUse.GetComponentInChildren<Text>().text = "���";
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
                message.GetComponent<Text>().text = "�κ��丮 ������ �����մϴ�.";
                break;

            case ActionType.FullHeart:
                if (!message.activeSelf) message.SetActive(true);
                message.GetComponent<Text>().text = "�̹� ü���� ���� �� �ֽ��ϴ�.";
                break;

            case ActionType.NotEquippable:
                if (!message.activeSelf) message.SetActive(true);
                message.GetComponent<Text>().text = "������ �� ���� ĳ�����Դϴ�.";
                break;

            case ActionType.WrongPosition:
                if (!message.activeSelf) message.SetActive(true);
                message.GetComponent<Text>().text = "������ �� ���� �����Դϴ�.";
                break;

            case ActionType.WrongItem:
                if (!message.activeSelf) message.SetActive(true);
                message.GetComponent<Text>().text = "������ �� ���� ����Դϴ�.";
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