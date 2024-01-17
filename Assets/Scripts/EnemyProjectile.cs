using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public enum projectileType { Seed };

    projectileType projectileName;

    Rigidbody2D rigid;
    Transform shadow;
    Vector3 dir;
    Vector3 shadowLocalPos;
    float speed;
    float timer;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        shadow = transform.GetChild(0);
        shadowLocalPos = shadow.localPosition;
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer > 3f)
        {
            Done();
            return;
        }

        transform.Rotate(Vector3.forward, -360 * Time.fixedDeltaTime * 2);
        shadow.rotation = Quaternion.identity;
        shadow.position = transform.position + shadowLocalPos;
        Vector3 targetPos = Player.instance.transform.position + new Vector3(0, 0.25f, 0);
        Vector3 newDir = targetPos - transform.position;
        if (Vector2.Angle(newDir, dir) < 45)
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
        timer = 0f;
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
