using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
        closeAction.performed += _ => Close();
        closeAction2.performed += _ => Close();
    }

    // Ÿ��Ʋ ȭ�鿡�� �ٷ� Stage 0 ������ ��, �� ������ ���
    // ��湮�ϴ� ���� Ʃ�丮�� ������ ����� ���� (�⺻��: Disable ����)
    private void Start()
    {
        AudioManager.instance.PlayBgm(false);
        GameManager.Instance.Stop();
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
        GameManager.Instance.Resume();
        AudioManager.instance.PlayBgm(true);
        gameObject.SetActive(false);
    }
}