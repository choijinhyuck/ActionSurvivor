using System.Collections;
using System.Collections.Generic;
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
    public Rigidbody2D target;
    public Slider hpBar;
    public Sprite[] barSprites;
    public Image barImage;
    public Text hitDamage;

    bool isLive;
    bool lookLeft;
    bool isHit;
    float hitTextPosXrange;
    float hitTextPosYstart;
    float hitTextPosYend;
    int exp;
    DropItems[] dropItems;
    Vector2 shadowOrigin;
    Vector2 shadowFlip;

    Rigidbody2D rigid;
    CapsuleCollider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    Transform shadow;
    WaitForSeconds waitSec;
    WaitForSeconds waitShortTime;
    WaitForFixedUpdate waitFix;
    Coroutine knockbackCoroutine;
    List<GameObject> hitText;
    

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        shadow = GetComponentsInChildren<Transform>()[1];
        waitSec = new WaitForSeconds(.1f);
        waitShortTime = new WaitForSeconds(.01f);
        waitFix = new WaitForFixedUpdate();
        isHit = false;
        hitText = new List<GameObject>() {hitDamage.gameObject};
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.isLive) return;

        if (!isLive || isHit)
            return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed / 5 * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    private void LateUpdate()
    {
        if (!GameManager.instance.isLive) return;

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
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        health = maxHealth;
        dropItems = new DropItems[] { };
        if (hpBar.gameObject.activeSelf) hpBar.gameObject.SetActive(false);
        barImage.sprite = barSprites[0];
        foreach (var hit in hitText)
        {
            if (hit.activeSelf)
            {
                hit.SetActive(false);
            }
        }
    }

    public void Init(EnemyData data)
    {
        anim.runtimeAnimatorController = data.anim;
        enemyName = data.enemyName;
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        exp = data.exp;
        rigid.mass = data.mass;
        Positioning(data);
        dropItems = data.dropItems;
        hpBar.GetComponent<RectTransform>().anchoredPosition = data.hpBarPos;
        hpBar.GetComponent<RectTransform>().sizeDelta = data.hpBarSize;
        hitTextPosXrange = data.hpBarSize.x / 4;
        hitTextPosYstart = data.hpBarPos.y * .9f;
        hitTextPosYend = data.hpBarPos.y * 1.25f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLive) return;
        if (!collision.CompareTag("Projectile") && !collision.CompareTag("Skill"))
            return;


        if (collision.CompareTag("Projectile"))
        {
            health -= collision.GetComponent<Projectile>().damage;
            HitDamageText(collision.GetComponent<Projectile>().damage);
        }
        else if (collision.CompareTag("Skill"))
        {
            if (GameManager.instance.playerId == 0)
            {
                if (collision.GetComponent<Skill>().hitList.Contains(gameObject)) return;
            }

            health -= collision.GetComponent<Skill>().damageRate * GameManager.instance.playerDamage;
            HitDamageText(collision.GetComponent<Skill>().damageRate * GameManager.instance.playerDamage);
        }

        if (!hpBar.gameObject.activeSelf) hpBar.gameObject.SetActive(true);

        if (isHit) StopCoroutine(knockbackCoroutine);

        if (collision.transform.parent.name == "Level 1")
        {
            knockbackCoroutine = StartCoroutine(KnockBack(15));
        }
        else
        {
            knockbackCoroutine = StartCoroutine(KnockBack(10));
        }


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
            

            if (GameManager.instance.isLive)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
            }
        }
    }

    void HitDamageText(float damage)
    {
        int selectId = -1;
        for (int i = 0; i < hitText.Count; i++)
        {
            if (!hitText[i].activeSelf)
            {
                selectId = i;
                break;
            }
        }
        if (selectId == -1)
        {
            var newTextObject = Instantiate<GameObject>(hitText[0], hpBar.transform.parent);
            hitText.Add(newTextObject);
            //newTextObject.transform.parent = hpBar.transform.parent;
            selectId = hitText.Count - 1;
        }
        hitText[selectId].SetActive(true);
        hitText[selectId].GetComponent<Text>().text = damage.ToString("N0") + "<size=12> 피해</size>";
        hitText[selectId].GetComponent<Text>().color = Color.red;
        //hitText[selectId].transform.localScale = new Vector3(1f, 1f, 1f);
        Vector2 textPos = new Vector2(Random.Range(-hitTextPosXrange, hitTextPosXrange), hitTextPosYstart);
        hitText[selectId].GetComponent<RectTransform>().anchoredPosition = textPos;
        StartCoroutine(MoveText(hitText[selectId]));
    }

    IEnumerator MoveText(GameObject targetObject)
    {
        float timer = 0f;
        Color textColor = Color.red;
        while (timer < 1f)
        {
            yield return waitFix;
            timer += Time.fixedDeltaTime;
            targetObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, (hitTextPosYend - hitTextPosYstart) * Time.fixedDeltaTime);
            if (timer > .5f)
            {
                textColor.a -= Time.fixedDeltaTime * 2;
                targetObject.GetComponent<Text>().color = textColor;
            }
        }
        targetObject.SetActive(false);
    }

    IEnumerator KnockBack(int force)
    {
        isHit = true;
        spriter.material.SetFloat("_FlashAmount", 0.25f);
        yield return waitShortTime;
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
        yield return waitSec;
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
        GameManager.instance.kill++;
        GameManager.instance.GetExp(exp);
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

    void Positioning(EnemyData data)
    {
        shadow.gameObject.SetActive(true);
        shadowOrigin = data.shadowOrigin;
        shadowFlip = data.shadowFlip;
        shadow.localPosition = shadowOrigin;
        shadow.localScale = new Vector3(data.shadowScale, data.shadowScale, data.shadowScale);

        lookLeft = data.lookLeft;

        coll.size = data.collSize;
        coll.offset = data.collPos;
    }

    void DropItem()
    {
        if (dropItems.Length == 0) return;

        float totalProbability = 0f;
        foreach (var dropItem in dropItems)
        {
            totalProbability += dropItem.probability;
        }

        float randValue = Random.Range(0,Mathf.Max(1f, totalProbability));
        float accumulatedValue = 0f;
        int selectedItemId = -1;
        for (int i = 0;  i < dropItems.Length; i++)
        {
            accumulatedValue += dropItems[i].probability;
            if (randValue < accumulatedValue)
            {
                selectedItemId = (int)dropItems[i].itemType;
                break;
            }
        }
        if (selectedItemId == -1) return;
        int prefabId = -1;
        for (int i = 0; i < PoolManager.instance.prefabs.Length; i++)
        {
            if (PoolManager.instance.prefabs[i] == ItemManager.Instance.itemDataArr[selectedItemId].dropItem)
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

        GameObject selectedItem = PoolManager.instance.Get(prefabId);
        selectedItem.transform.parent = PoolManager.instance.transform.GetChild(2);
        selectedItem.transform.position = transform.position;
        selectedItem.GetComponent<DropItem>().itemId = selectedItemId;
        selectedItem.GetComponent<DropItem>().Init();
    }
}
