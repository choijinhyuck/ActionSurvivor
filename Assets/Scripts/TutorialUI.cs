using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public enum UIType { FirstTutorial, ChangeTutorial, Ending }

    [SerializeField] InputActionAsset actions;
    [SerializeField] Sprite[] keySprites;
    [SerializeField] Image[] keyImages;
    [SerializeField] UIType uiType;

    InputAction closeAction;
    InputAction closeAction2;

    private void Awake()
    {
        closeAction = actions.FindActionMap("UI").FindAction("Menu");
        closeAction2 = actions.FindActionMap("UI").FindAction("Cancel");
        closeAction.performed += CloseHandler;
        closeAction2.performed += CloseHandler;
    }

    void CloseHandler(InputAction.CallbackContext context)
    {
        Close();
    }

    // 타이틀 화면에서 바로 Stage 0 씬으로 온, 새 게임인 경우
    // 재방문하는 경우는 튜토리얼 도움말을 띄우지 않음 (기본값: Disable 유지)
    private void Start()
    {
        //AudioManager.instance.PlayBgm(false);
        if (SceneManager.GetActiveScene().name == "Camp")
        {
            GameManager.instance.isLive = false;
        }
        else
        {
            GameManager.instance.Stop();
        }
    }

    private void OnDestroy()
    {
        closeAction.performed -= CloseHandler;
        closeAction2.performed -= CloseHandler;
    }

    private void Update()
    {
        switch (ControllerManager.instance.CurrentScheme)
        {
            case ControllerManager.scheme.Keyboard:
                keyImages[0].sprite = keySprites[0];
                keyImages[1].sprite = keySprites[1];
                break;

            case ControllerManager.scheme.Gamepad:
                keyImages[0].sprite = keySprites[2];
                keyImages[1].sprite = keySprites[3];
                break;
        }
    }

    void Close()
    {
        if (!gameObject.activeSelf) return;

        if (SceneManager.GetActiveScene().name == "Camp")
        {
            if (uiType == UIType.ChangeTutorial)
            {
                GameManager.instance.newCharacterUnlock = 1;
                GameManager.instance.isLive = true;
            }
            else if (uiType == UIType.Ending)
            {
                GameManager.instance.gameClear = 1;
                GameManager.instance.isLive = true;
            }

        }
        else
        {
            GameManager.instance.Resume();
            AudioManager.instance.PlayBgm(true);

        }
        Destroy(gameObject);
    }
}
