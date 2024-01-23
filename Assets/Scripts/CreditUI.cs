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

    private void Awake()
    {
        lastSelectedObject = null;

        cancelAction = actions.FindActionMap("UI").FindAction("Cancel");
        menuAction = actions.FindActionMap("UI").FindAction("Menu");

        cancelAction.performed += CancelHandler;
        menuAction.performed += CancelHandler;


        try
        {
            input = (InputSystemUIInputModule)EventSystem.current.currentInputModule;
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            Debug.Log("Credit UI GameObject를 비활성화 한 상태로 Title 씬을 시작해야합니다.");
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
        lastSelectedObject = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void FixedUpdate()
    {
        Vector2 inputVector = input.move.action.ReadValue<Vector2>();
        if (inputVector.y > 0f)
        {
            float nextValue = scrollBar.value + scrollSpeed * scrollBar.size;
            nextValue = Mathf.Min(1f, nextValue);
            scrollBar.value = nextValue;
        }
        else if (inputVector.y < 0f)
        {
            float nextValue = scrollBar.value - scrollSpeed * scrollBar.size;
            nextValue = Mathf.Max(0f, nextValue);
            scrollBar.value = nextValue;
        }
    }

    private void LateUpdate()
    {
        switch (ControllerManager.instance.CurrentScheme)
        {
            case ControllerManager.scheme.Keyboard:
                closeHelp.text = "창 닫기: <color=yellow>Esc</color> 또는 <color=yellow>S</color>";
                break;
            case ControllerManager.scheme.Gamepad:
                closeHelp.text = "창 닫기: <color=yellow>Start</color> 또는 <color=yellow>B</color>";
                break;
        }
    }
}
