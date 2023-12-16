using System.Collections;
using System.Threading;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    bool isLive;
    bool lookLeft;
    bool isHit;
    Vector2 shadowOrigin;
    Vector2 shadowFlip;

    Rigidbody2D rigid;
    CapsuleCollider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    Transform shadow;
    WaitForFixedUpdate wait;
    WaitForSeconds waitSec;
    


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        shadow = GetComponentsInChildren<Transform>()[1];
        wait = new WaitForFixedUpdate();
        waitSec = new WaitForSeconds(.1f);
        isHit = false;
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.isLive) return;

        if (!isLive || isHit)
            return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    private void LateUpdate()
    {
        if (!GameManager.Instance.isLive) return;

        if (!isLive) return;
        
        if (lookLeft)
        {
            spriter.flipX = target.position.x > rigid.position.x;
            if (spriter.flipX)
            {
                shadow.localPosition = shadowFlip;
            }
            else
            {
                shadow.localPosition = shadowOrigin;
            }
        }
        else
        {
            spriter.flipX = target.position.x < rigid.position.x;
            if (spriter.flipX)
            {
                shadow.localPosition = shadowFlip;
            }
            else
            {
                shadow.localPosition = shadowOrigin;
            }
        }
        
    }

    private void OnEnable()
    {
        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        health = maxHealth;
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        enemyName = data.name;
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        Positioning(data);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLive) return;
        if (!collision.CompareTag("Bullet") && !collision.CompareTag("Skill"))
            return;

        if (collision.CompareTag("Bullet"))
        {
            health -= collision.GetComponent<Projectile>().damage;
            StartCoroutine(KnockBack(3));
        }
        else if (collision.CompareTag("Skill"))
        {
            health -= collision.GetComponent<Skill>().damage;
            StartCoroutine(KnockBack(10));
        }



        if (health > 0)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            StartCoroutine(Dead());
            GameManager.Instance.kill++;
            GameManager.Instance.GetExp();

            if (GameManager.Instance.isLive)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);

            }
        }
    }

    IEnumerator KnockBack(int force)
    {
        isHit = true;

        spriter.material.SetFloat("_FlashAmount", 0.25f);
        yield return wait; // 다음 하나의 물리 프레임
        rigid.velocity = Vector2.zero;
        Vector2 dirVec = rigid.position - target.position;
        rigid.AddForce(dirVec.normalized * force, ForceMode2D.Impulse);
        spriter.material.SetFloat("_FlashAmount", 0.5f);
        yield return wait; // 다음 하나의 물리 프레임
        spriter.material.SetFloat("_FlashAmount", 0.75f);
        yield return wait; // 다음 하나의 물리 프레임
        spriter.material.SetFloat("_FlashAmount", 1.0f);
        yield return wait;
        spriter.material.SetFloat("_FlashAmount", 0.75f);
        yield return wait;
        spriter.material.SetFloat("_FlashAmount", 0.5f);
        yield return wait;
        spriter.material.SetFloat("_FlashAmount", 0f);
        rigid.velocity = Vector2.zero;
        yield return new WaitForSeconds(.1f);
        isHit = false;
    }

    IEnumerator Dead()
    {
        Color currColor = spriter.color;
        currColor.a = .8f;
        spriter.color = currColor;
        yield return waitSec;
        currColor.a = .6f;
        spriter.color = currColor;
        yield return waitSec;
        currColor.a = .4f;
        spriter.color = currColor;
        yield return waitSec;
        currColor.a = .2f;
        spriter.color = currColor;
        yield return waitSec;
        gameObject.SetActive(false);
        currColor.a = 1f;
        spriter.color = currColor;
    }

    void Positioning(SpawnData data)
    {
        shadow.gameObject.SetActive(true);
        shadowOrigin = data.shadowOrigin;
        shadowFlip = data.shadowFlip;
        shadow.localPosition = shadowOrigin;
        shadow.localScale = new Vector3(data.shadowScale,data.shadowScale, data.shadowScale);

        lookLeft = data.lookLeft;
        
        coll.size = data.collSize;
        coll.offset = data.collPos;
    }
}
