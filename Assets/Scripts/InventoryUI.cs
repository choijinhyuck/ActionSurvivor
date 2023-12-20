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

    List<Button> buttons;
    Canvas[] canvases;
    Image[] itemImages;
    GameObject currentSelect;

    bool isFirstStart;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>(true).ToList<Button>();
        itemImages = new Image[24];
        canvases = new Canvas[24];

        for (int i = 0; i < buttons.Count; i++)
        {
            if (i > 23)
            {
                buttons[i].gameObject.SetActive(true);
                continue;
            }
            buttons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < 24; i++)
        {
            canvases[i] = buttons[i].GetComponentInParent<Canvas>();
            itemImages[i] = buttons[i].GetComponentsInChildren<Image>()[1];
            int temp = i;
            buttons[i].onClick.AddListener(() => OnPress(temp));
        }
    }

    void OnEnable()
    {
        for (int i = 0; i < GameManager.Instance.maxInventory; i++)
        {
            buttons[i].gameObject.SetActive(true);
        }
        
        ChangeAlpha(1f);
        foreach (var canvas in canvases)
        {
            canvas.sortingOrder = 1;
        }
        EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
        currentSelect = buttons[0].gameObject;
        canvases[0].sortingOrder = 2;
        baseUI.sortingOrder = 0;

        for (int i = 0; i < GameManager.Instance.maxInventory; i++)
        {
            if (GameManager.Instance.inventoryItemsId[i] == -1)
            {
                itemImages[i].sprite = null;
            }
            else
            {
                itemImages[i].sprite = ItemManager.Instance.itemDataArr[GameManager.Instance.inventoryItemsId[i]].itemIcon;
            }
        }
    }

    private void LateUpdate()
    {
        if (currentSelect == EventSystem.current.currentSelectedGameObject)
        {
            return;
        }
        currentSelect.GetComponentInParent<Canvas>().sortingOrder = 1;
        currentSelect = EventSystem.current.currentSelectedGameObject;
        currentSelect.GetComponentInParent<Canvas>().sortingOrder = 2;
        baseUI.sortingOrder = 0;
    }

    void OnPress(int buttonIndex)
    {
        ChangeAlpha(.5f);
        itemImages[buttonIndex].color = new Color(1f, 1f, 1f, 1f);
    }

    // �κ��丮 â���� �ƹ� ��ư�� ���õ��� ���� ��쿡 MenuŰ (Ű����: Esc, �����е�: Start)�� �������� �� �ֵ���.
    public void OnMenu()
    {
        if (buttons.Contains(EventSystem.current.currentSelectedGameObject?.GetComponent<Button>()))
        {
            // �κ��丮 â�� ���� ���� ���� ���� �޴� â�� ����� �ϱ� ������ �κ�â�� �����ִ� ��츸 ���� �۵��ϵ���
            if (GameManager.Instance.workingInventory) GameManager.Instance.OnInventory();
        }
    }

    void ChangeAlpha(float targetAlpha)
    {
        Color targetColor = new Color(1f, 1f, 1f, targetAlpha);

        for (int i = 0; i < 24; i++)
        {
            itemImages[i].color = targetColor;
        }
    }
}
