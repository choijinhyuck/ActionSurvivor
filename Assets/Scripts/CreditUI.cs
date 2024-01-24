using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class CreditUI : MonoBehaviour
{
    [SerializeField] InputActionAsset actions;
    [SerializeField] Scrollbar scrollBar;
    [SerializeField] Text closeHelp;
    [SerializeField] float scrollSpeed;

    GameObject lastSelectedObject;
    InputSystemUIInputModule input;
    InputAction cancelAction;
    InputAction menuAction;
    InputAction moveAction;

    private void Awake()
    {
        lastSelectedObject = null;

        cancelAction = actions.FindActionMap("UI").FindAction("Cancel");
        menuAction = actions.FindActionMap("UI").FindAction("Menu");
        moveAction = actions.FindActionMap("UI").FindAction("Move");

        cancelAction.performed += CancelHandler;
        menuAction.performed += CancelHandler;


        try
        {
            input = (InputSystemUIInputModule)EventSystem.current.currentInputModule;
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            Debug.Log("Credit UI GameObject�� ��Ȱ��ȭ �� ���·� Title ���� �����ؾ��մϴ�.");
        }


    }

    void CancelHandler(InputAction.CallbackContext context)
    {
        if (gameObject.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedObject);
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        cancelAction.performed -= CancelHandler;
        menuAction.performed -= CancelHandler;
    }


    private void OnEnable()
    {
        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
        {
            GetComponentInChildren<Text>(true).text = "ũ����";
        }
        else
        {
            GetComponentInChildren<Text>(true).text = "Credits";
        }
        lastSelectedObject = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(null);
    }


    private void LateUpdate()
    {
        Vector2 inputVector = moveAction.ReadValue<Vector2>();
        if (inputVector.y > 0f)
        {
            float nextValue = scrollBar.value + scrollSpeed * scrollBar.size * Time.unscaledDeltaTime;
            nextValue = Mathf.Min(1f, nextValue);
            scrollBar.value = nextValue;
        }
        else if (inputVector.y < 0f)
        {
            float nextValue = scrollBar.value - scrollSpeed * scrollBar.size * Time.unscaledDeltaTime;
            nextValue = Mathf.Max(0f, nextValue);
            scrollBar.value = nextValue;
        }

        switch (ControllerManager.instance.CurrentScheme)
        {
            case ControllerManager.scheme.Keyboard:
                if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
                {
                    closeHelp.text = "â �ݱ�: <color=yellow>Esc</color> �Ǵ� <color=yellow>S</color>";
                }
                else
                {
                    closeHelp.text = "Close: <color=yellow>Esc</color> or <color=yellow>S</color>";
                }
                break;
            case ControllerManager.scheme.Gamepad:
                if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
                {
                    closeHelp.text = "Close: <color=yellow>Start</color> or <color=yellow>B</color>";
                }
                else
                {
                    closeHelp.text = "Close: <color=yellow>Start</color> or <color=yellow>B</color>";
                }
                break;
        }
    }
}
