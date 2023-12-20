using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    List<Button> buttons;
    Canvas[] canvases;
    Image[] itemImages;
    GameObject currentSelect;

    bool isFirstStart;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>().ToList<Button>();
        itemImages = new Image[buttons.Count];
        canvases = new Canvas[buttons.Count];

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
        ChangeAlpha(1f);
        foreach (var canvas in canvases)
        {
            canvas.sortingOrder = 1;
        }
        EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
        currentSelect = buttons[0].gameObject;
        canvases[0].sortingOrder = 2;
    }

    private void Update()
    {
        if (currentSelect == EventSystem.current.currentSelectedGameObject)
        {
            return;
        }
        currentSelect.GetComponentInParent<Canvas>().sortingOrder = 1;
        currentSelect = EventSystem.current.currentSelectedGameObject;
        currentSelect.GetComponentInParent<Canvas>().sortingOrder = 2;
    }

    void OnPress(int buttonIndex)
    {
        ChangeAlpha(.5f);
        itemImages[buttonIndex].color = new Color(1f, 1f, 1f, 1f);
    }

    // 인벤토리 창에서 아무 버튼도 선택되지 않은 경우에 Menu키 (키보드: Esc, 게임패드: Start)로 빠져나올 수 있도록.
    public void OnMenu()
    {
        if (buttons.Contains(EventSystem.current.currentSelectedGameObject?.GetComponent<Button>()))
        {
            // 인벤토리 창이 열려 있지 않은 경우는 메뉴 창을 띄워야 하기 때문에 인벤창이 켜져있는 경우만 끄는 작동하도록
            if (GameManager.Instance.workingInventory) GameManager.Instance.OnInventory();
        }
    }

    void ChangeAlpha(float targetAlpha)
    {
        Color targetColor = new Color(1f, 1f, 1f, targetAlpha);

        foreach (var itemImage in itemImages)
        {
            itemImage.color = targetColor;
        }
    }
}
