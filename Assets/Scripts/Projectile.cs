using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public int pierceCount;

    Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int pierceCount, Vector3 dir)
    {
        this.damage = damage;
        this.pierceCount = pierceCount;

        if (pierceCount >= 0)
        {
            rigid.velocity = dir * 15f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || pierceCount == -100)
            return;

        pierceCount--;

        if (pierceCount < 0)
        {
            rigid.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || pierceCount == -100)
            return;

        gameObject.SetActive(false);
    }
}
