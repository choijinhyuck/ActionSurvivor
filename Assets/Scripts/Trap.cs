using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Trap : MonoBehaviour
{
    [SerializeField] Sprite trapOn;
    [SerializeField] Sprite trapOff;

    SpriteRenderer trapRenderer;
    Collider2D trapCollider;
    bool isOn;
    bool isTurning;

    private void Awake()
    {
        trapRenderer = GetComponent<SpriteRenderer>();
        trapCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        trapRenderer.sprite = trapOff;
        isOn = false;
        isTurning = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (Player.instance.IsDodge()) return;
        if (isOn)
        {
            Player.instance.HitByTrap(trapCollider);
        }
        else if (!isTurning)
        {
            StartCoroutine(TurningOnOffCoroutine());
        }
    }

public bool IsOn()
    {
        return isOn;
    }

    IEnumerator TurningOnOffCoroutine()
    {
        isTurning = true;
        yield return new WaitForSeconds(0.5f);
        isOn = true;
        trapRenderer.sprite = trapOn;
        // 효과음 추가
        AudioManager.instance.PlaySfx(AudioManager.Sfx.TrapOn);
        yield return new WaitForSeconds(1.5f);
        isOn = false;
        trapRenderer.sprite = trapOff;
        // 효과음 추가
        AudioManager.instance.PlaySfx(AudioManager.Sfx.TrapOff);
        isTurning = false;
    }
}
