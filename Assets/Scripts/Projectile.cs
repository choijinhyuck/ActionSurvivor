using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    int pierceCount;

    Rigidbody2D rigid;
    
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int pierceCount, Vector3 dir, float speed)
    {
        this.damage = damage;
        this.pierceCount = pierceCount;

        if (pierceCount >= 0)
        {
            rigid.velocity = dir * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
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
        if (!collision.CompareTag("Area"))
            return;

        rigid.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}
