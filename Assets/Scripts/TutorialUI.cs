using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] InputActionAsset actions;
    [SerializeField] Sprite[] keySprites;
    [SerializeField] Image[] keyImages;

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

    // Ÿ��Ʋ ȭ�鿡�� �ٷ� Stage 0 ������ ��, �� ������ ���
    // ��湮�ϴ� ���� Ʃ�丮�� ������ ����� ���� (�⺻��: Disable ����)
    private void Start()
    {
        //AudioManager.instance.PlayBgm(false);
        GameManager.instance.Stop();
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
        GameManager.instance.Resume();
        AudioManager.instance.PlayBgm(true);
        Destroy(gameObject);
        //gameObject.SetActive(false);
    }
}
