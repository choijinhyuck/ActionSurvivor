using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StorageChest : MonoBehaviour
{
    public LayerMask playerLayer;
    public Sprite[] equipSprites;

    [SerializeField]
    Image buttonImage;

    InputAction openAction;
    bool isNear;

    private void Start()
    {
        isNear = false;
        openAction = GameManager.Instance.actions.FindActionMap("UI").FindAction("OpenChest");
        openAction.performed += _ => Open();
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, .5f, Vector2.zero, 0, playerLayer);

        if (hit.collider is null)
        {
            isNear = false;
            buttonImage.transform.parent.gameObject.SetActive(false);
            return;
        }

        isNear = true;

        switch (ControllerManager.instance.CurrentScheme)
        {
            case ControllerManager.scheme.Keyboard:
                buttonImage.sprite = equipSprites[0];
                buttonImage.transform.parent.gameObject.SetActive(true);
                break;

            case ControllerManager.scheme.Gamepad:
                buttonImage.sprite = equipSprites[1];
                buttonImage.transform.parent.gameObject.SetActive(true);
                break;
        }

    }

    void Open()
    {
        if (!isNear) return;

        
    }
}
