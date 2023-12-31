using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcquireItem : MonoBehaviour
{
    public float scanRange;
    public LayerMask targetLayer;
    RaycastHit2D[] targets;
    List<GameObject> redundancies;
    Vector3 offset;
    float speed;
    float acquireRange;
    bool invenFull;


    private void Awake()
    {
        speed = 5;
        acquireRange = .1f;
        offset = new Vector3(0f, .25f, 0f);
        redundancies = new List<GameObject>();
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.health < .1f) return;

        invenFull = true;
        for (int i = 0; i < GameManager.instance.maxInventory; i++)
        {
            if (GameManager.instance.inventoryItemsId[i] == -1)
            {
                invenFull = false;
                break;
            }
        }
        if (!invenFull)
        {
            redundancies.Clear();
        }

        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.up, 0.25f, targetLayer);

        foreach (var target in targets)
        {
            target.transform.GetComponent<DropItem>().isDropping = false;
            Vector3 dir = transform.position + offset - target.transform.position;
            if (dir.magnitude < acquireRange)
            {
                GoldOrItem(target);
            }

            dir = dir.normalized;
            if (redundancies.Contains(target.transform.gameObject))
            {
                continue;
            }
            else
            {
                target.transform.position += dir * speed * Time.fixedDeltaTime;
            }
        }
    }


    void GoldOrItem(RaycastHit2D target)
    {
        // 드랍된 골드 아이템 종류에 따라 획득하는 순간 랜덤한 골드 획득
        switch (target.transform.GetComponent<DropItem>().itemId)
        {
            case 12:
                if (Random.Range(1, 11) > 7)
                {
                    GameManager.instance.gold += Random.Range(4, 6);
                }
                else
                {
                    GameManager.instance.gold += Random.Range(1, 4);
                }
                target.transform.gameObject.SetActive(false);
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Gold);
                break;
            case 13:
                if (Random.Range(1, 11) > 7)
                {
                    GameManager.instance.gold += Random.Range(6, 10);
                }
                else
                {
                    if (Random.Range(1, 11) > 7)
                    {
                        GameManager.instance.gold += Random.Range(10, 16);
                    }
                    else
                    {
                        GameManager.instance.gold += Random.Range(17, 20);
                    }
                }
                target.transform.gameObject.SetActive(false);
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Gold);
                break;
            case 14:
                if (Random.Range(1, 11) > 9)
                {
                    GameManager.instance.gold += Random.Range(20, 26);
                }
                else if (Random.Range(1, 11) > 9)
                {
                    GameManager.instance.gold += Random.Range(26, 30);
                }
                else if (Random.Range(1, 11) > 9)
                {
                    GameManager.instance.gold += Random.Range(30, 40);
                }
                else
                {
                    GameManager.instance.gold += Random.Range(40, 50);
                }
                target.transform.gameObject.SetActive(false);
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Gold);
                break;

            // 골드가 아닌 아이템을 획득한 경우 인벤토리에 넣음
            default:
                // 인벤토리 가득차면 아이템을 획득하지 않고 비활성화도 시키지 않음
                // 맵 상에 계속 두도록 함. 계속 따라오도록 할지 일단 멈추게 할지는 고민.
                bool isFull = true;

                for (int i = 0; i < GameManager.instance.maxInventory; i++)
                {
                    if (GameManager.instance.inventoryItemsId[i] == -1)
                    {
                        GameManager.instance.inventoryItemsId[i] = target.transform.GetComponent<DropItem>().itemId;
                        if (redundancies.Contains(target.transform.gameObject))
                            redundancies.Remove(target.transform.gameObject);
                        target.transform.gameObject.SetActive(false);
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.AcquireItem);
                        isFull = false;
                        break;
                    }
                }

                if (isFull)
                {
                    if (!redundancies.Contains(target.transform.gameObject))
                    {
                        redundancies.Add(target.transform.gameObject);
                    }
                }

                break;
        }
    }
}
