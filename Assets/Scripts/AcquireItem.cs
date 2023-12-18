using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcquireItem : MonoBehaviour
{
    public float scanRange;
    public LayerMask targetLayer;
    RaycastHit2D[] targets;
    float speed;
    float acquireRange;

    private void Awake()
    {
        speed = 5;
        acquireRange = .5f;
    }

    private void FixedUpdate()
    {
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.up, 0.35f, targetLayer);

        foreach (var target in targets)
        {
            target.transform.GetComponent<DropItem>().isDropping = false;
            Vector3 dir = transform.position - target.transform.position;
            if (dir.magnitude < acquireRange)
            {
                GoldOrItem(target);
                continue;
            }

            dir = dir.normalized;
            target.transform.position += dir * speed * Time.fixedDeltaTime;
        }
    }
    
    void GoldOrItem(RaycastHit2D target)
    {
        // ����� ��� ������ ������ ���� ȹ���ϴ� ���� ������ ��� ȹ��
        switch (target.transform.GetComponent<DropItem>().itemId)
        {
            case 12:
                if (Random.Range(1, 11) > 7)
                {
                    GameManager.Instance.gold += Random.Range(4, 6);
                }
                else
                {
                    GameManager.Instance.gold += Random.Range(1, 4);
                }
                target.transform.gameObject.SetActive(false);
                break;
            case 13:
                if (Random.Range(1, 11) > 7)
                {
                    GameManager.Instance.gold += Random.Range(6, 10);
                }
                else
                {
                    if (Random.Range(1, 11) > 7)
                    {
                        GameManager.Instance.gold += Random.Range(10, 16);
                    }
                    else
                    {
                        GameManager.Instance.gold += Random.Range(17, 20);
                    }
                }
                target.transform.gameObject.SetActive(false);
                break;
            case 14:
                if (Random.Range(1, 11) > 9)
                {
                    GameManager.Instance.gold += Random.Range(20, 26);
                }
                else if (Random.Range(1, 11) > 9)
                {
                    GameManager.Instance.gold += Random.Range(26, 30);
                }
                else if (Random.Range(1, 11) > 9)
                {
                    GameManager.Instance.gold += Random.Range(30, 40);
                }
                else
                {
                    GameManager.Instance.gold += Random.Range(40, 50);
                }
                target.transform.gameObject.SetActive(false);
                break;

            // ��尡 �ƴ� �������� ȹ���� ��� �κ��丮�� ����
            default:
                // �κ��丮 �������� �������� ȹ������ �ʰ� ��Ȱ��ȭ�� ��Ű�� ����
                // �� �� ��� �ε��� ��. ��� ��������� ���� �ϴ� ���߰� ������ ���.
                if (GameManager.Instance.maxInventory == GameManager.Instance.inventoryItemsId.Count)
                    return;
                GameManager.Instance.inventoryItemsId.Add(target.transform.GetComponent<DropItem>().itemId);
                target.transform.gameObject.SetActive(false);
                break;
        }
    }
}
