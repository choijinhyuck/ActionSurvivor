using System.Collections;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;
    public Slider hpBar;
    public Sprite[] barSprites;
    public Image barImage;

    bool isLive;
    bool lookLeft;
    bool isHit;
    int exp;
    int[] dropItemsId;
    float[] dropProbability;
    Vector2 shadowOrigin;
    Vector2 shadowFlip;

    Rigidbody2D rigid;
    CapsuleCollider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    Transform shadow;
    WaitForSeconds waitSec;
    WaitForSeconds waitShortTime;
    Coroutine knockbackCoroutine;
    

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        shadow = GetComponentsInChildren<Transform>()[1];
        waitSec = new WaitForSeconds(.1f);
        waitShortTime = new WaitForSeconds(.01f);
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

        hpBar.value = health / maxHealth;
        if (hpBar.value < .3f)
        {
            barImage.sprite = barSprites[2];
        }
        else if (hpBar.value  < .6f)
        {
            barImage.sprite = barSprites[1];
        }
    }

    private void OnEnable()
    {
        isHit = false;
        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        health = maxHealth;
        dropItemsId = new int[] { };
        if (hpBar.gameObject.activeSelf) hpBar.gameObject.SetActive(false);
        barImage.sprite = barSprites[0];
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        enemyName = data.name;
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        exp = data.exp;
        Positioning(data);
        dropItemsId = data.dropItemsId;
        dropProbability = data.dropProbability;
        hpBar.GetComponent<RectTransform>().anchoredPosition = data.hpBarPos;
        hpBar.GetComponent<RectTransform>().sizeDelta = data.hpBarSize;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLive) return;
        if (!collision.CompareTag("Projectile") && !collision.CompareTag("Skill"))
            return;


        if (collision.CompareTag("Projectile"))
        {
            health -= collision.GetComponent<Projectile>().damage;
        }
        else if (collision.CompareTag("Skill"))
        {
            health -= collision.GetComponent<Skill>().damage;
        }

        if (!hpBar.gameObject.activeSelf) hpBar.gameObject.SetActive(true);

        if (isHit) StopCoroutine(knockbackCoroutine);
        knockbackCoroutine = StartCoroutine(KnockBack(10));


        if (health > 0)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else
        {
            hpBar.value = 0f;
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            StartCoroutine(Dead());
            GameManager.Instance.kill++;
            GameManager.Instance.GetExp(exp);

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
        yield return waitShortTime; // 0.02초
        rigid.velocity = Vector2.zero;
        Vector2 dirVec = rigid.position - target.position;
        rigid.AddForce(dirVec.normalized * force, ForceMode2D.Impulse);
        spriter.material.SetFloat("_FlashAmount", 0.5f);
        yield return waitShortTime;
        spriter.material.SetFloat("_FlashAmount", 0.75f);
        yield return waitShortTime;
        spriter.material.SetFloat("_FlashAmount", 1.0f);
        yield return waitShortTime;
        spriter.material.SetFloat("_FlashAmount", 0.75f);
        yield return waitShortTime;
        spriter.material.SetFloat("_FlashAmount", 0.5f);
        yield return waitShortTime;
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

        DropItem();
        currColor.a = .4f;
        spriter.color = currColor;
        yield return waitSec;
        currColor.a = .2f;
        spriter.color = currColor;
        yield return waitSec;
        currColor.a = 1f;
        spriter.color = currColor;
        gameObject.SetActive(false);
        
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

    void DropItem()
    {
        if (dropItemsId.Length == 0)
            return;

        float randValue = Random.value;
        float accumulatedValue = 0f;
        int selectedItemId = -1;
        for (int i = 0;  i < dropItemsId.Length; i++)
        {
            accumulatedValue += dropProbability[i];
            if (randValue < accumulatedValue)
            {
                selectedItemId = dropItemsId[i];
                break;
            }
        }
        if (selectedItemId == -1) return;
        int prefabId = -1;
        for (int i = 0; i < GameManager.Instance.pool.prefabs.Length; i++)
        {
            if (GameManager.Instance.pool.prefabs[i] == ItemManager.Instance.itemDataArr[selectedItemId].dropItem)
            {
                prefabId = i;
                break;
            }
        }
        if (prefabId == -1)
        {
            Debug.Log("드랍 아이템에 해당하는 PrefabId를 Pool에서 찾을 수 없습니다.");
            return;
        }

        GameObject selectedItem = GameManager.Instance.pool.Get(prefabId);
        selectedItem.transform.parent = GameManager.Instance.pool.dropItemsPool;
        selectedItem.transform.position = transform.position;
        selectedItem.GetComponent<DropItem>().itemId = selectedItemId;
        selectedItem.GetComponent<DropItem>().Init();
    }
}
