using UnityEngine;

public class Projectile : MonoBehaviour
{
    public TrailRenderer trail;
    public LayerMask targetLayer;
    public float damage;
    int pierceCount;
    int itemId;
    float speed;


    Rigidbody2D rigid;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int pierceCount, Vector3 dir, float speed, int itemId)
    {
        this.damage = damage;
        this.pierceCount = pierceCount;
        this.itemId = itemId;
        this.speed = speed;

        switch (itemId)
        {
            case (int)ItemData.Items.Kunai:
            case (int)ItemData.Items.KunaiPlus:
            case (int)ItemData.Items.Shuriken:
            case (int)ItemData.Items.ShurikenPlus:
            case (int)ItemData.Items.Arrow:
            case (int)ItemData.Items.ArrowPlus:
                if (pierceCount >= 0)
                {
                    rigid.velocity = dir * speed;
                }
                break;

            default:
                //추후 마법 구현
                break;
        }
    }


    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (!collision.CompareTag("Enemy"))
    //        return;


    //    pierceCount--;
    //    Debug.Log(pierceCount);
    //    if (pierceCount < 0)
    //    {
    //        rigid.velocity = Vector3.zero;
    //        trail.Clear();
    //        gameObject.SetActive(false);
    //    }
    //    else if (itemId == (int)ItemData.Items.Shuriken || itemId == (int)ItemData.Items.ShurikenPlus)
    //    {
    //        RaycastHit2D[] targets = Physics2D.CircleCastAll(transform.position, 1f, Vector2.zero, 0f, targetLayer);
    //        Transform nearestTarget = null;
    //        float distance = 2f;
    //        foreach (var target in targets)
    //        {
    //            if (((target.transform.position - transform.position).magnitude < distance) && (target.transform.gameObject != collision.gameObject))
    //            {
    //                distance = (target.transform.position - transform.position).magnitude;
    //                nearestTarget = target.transform;
    //            }
    //        }
    //        if (nearestTarget != null)
    //        {
    //            rigid.velocity = (nearestTarget.transform.position - transform.position).normalized * speed;
    //        }
    //        else
    //        {
    //            rigid.velocity = Quaternion.AngleAxis(Random.Range(-70f, 70f), Vector3.forward) * rigid.velocity;
    //        }
    //    }
    //}

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
            return;

        rigid.velocity = Vector3.zero;
        trail.Clear();
        gameObject.SetActive(false);
    }

    public bool StillLive()
    {
        return pierceCount >= 0;
    }

    public void Pierce(Transform collision)
    {
        pierceCount--;

        if (pierceCount < 0)
        {
            rigid.velocity = Vector3.zero;
            trail.Clear();
            gameObject.SetActive(false);
        }
        else if (itemId == (int)ItemData.Items.Shuriken || itemId == (int)ItemData.Items.ShurikenPlus)
        {
            RaycastHit2D[] targets = Physics2D.CircleCastAll(transform.position, 1f, Vector2.zero, 0f, targetLayer);
            Transform nearestTarget = null;
            float distance = 2f;
            foreach (var target in targets)
            {
                if (((target.transform.position - transform.position).magnitude < distance) && (target.transform.gameObject != collision.gameObject))
                {
                    distance = (target.transform.position - transform.position).magnitude;
                    nearestTarget = target.transform;
                }
            }
            if (nearestTarget != null)
            {
                rigid.velocity = (nearestTarget.transform.position - transform.position).normalized * speed;
            }
            else
            {
                rigid.velocity = Quaternion.AngleAxis(Random.Range(-70f, 70f), Vector3.forward) * rigid.velocity;
            }
        }
    }
}
