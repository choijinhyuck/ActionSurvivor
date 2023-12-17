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
            Vector3 dir = transform.position - target.transform.position;
            if (dir.magnitude < acquireRange)
            {
                target.transform.gameObject.SetActive(false);
                continue;
            }

            dir = dir.normalized;
            target.transform.position += dir * speed * Time.fixedDeltaTime;
        }
    }
}
