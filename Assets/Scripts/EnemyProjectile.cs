using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public enum projectileType { Seed };

    projectileType projectileName;

    Rigidbody2D rigid;
    Renderer sprite;
    Vector3 dir;
    float speed;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<Renderer>();
    }

    private void FixedUpdate()
    {
        Vector3 targetPos = Player.instance.transform.position + new Vector3(0, 0.25f, 0);
        Vector3 newDir = targetPos - transform.position;
        if (Vector2.Angle(newDir, dir) < 60)
        {
            dir = dir.normalized + (newDir.normalized - dir.normalized) * Time.fixedDeltaTime / 1.5f;
            dir = dir.normalized;
            rigid.velocity = dir * speed;
        }
    }

    public void Init(projectileType projectileName, Vector3 dir, float speed)
    {
        this.projectileName = projectileName;
        this.dir = dir;
        this.speed = speed;
        rigid.velocity = dir * speed;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("EnemyProjectileArea"))
            return;

        Done();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        Player.instance.HitByProjectile(projectileName, GetComponent<CircleCollider2D>());
    }

    public void Done()
    {
        rigid.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}
