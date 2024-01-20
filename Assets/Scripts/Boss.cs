using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public enum AnimType { Idle, Move, Attack, Fire}

    public string enemyName;
    public float speed;
    public float health;
    public float maxHealth;
    public Rigidbody2D target;
    public Slider hpBar;
    public Sprite[] barSprites;
    public Image barImage;
    public Text hitDamage;

    [SerializeField] EnemyData enemyData;
    [SerializeField] GameObject goblinDashEffect;
    [SerializeField] GameObject dashTextBox;
    [SerializeField] GameObject fireTextBox;

    bool isLive;
    bool lookLeft;
    bool isHit;
    bool isInit;
    bool isFire;
    bool isDash;
    float hitTextPosXrange;
    float hitTextPosYstart;
    float hitTextPosYend;
    float fireTimer;
    float fireInterval;
    float fireDistance;
    int fireCount;
    int fireMaxCount;
    float dashTimer;
    float dashInterval;
    float attackDistance;
    int exp;
    int phase;

    DropItems[] dropItems;

    Rigidbody2D rigid;
    CapsuleCollider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    WaitForSeconds waitSec;
    WaitForSeconds waitShortTime;
    WaitForFixedUpdate waitFix;
    Coroutine knockbackCoroutine;
    List<GameObject> hitText;
    GameObject selectedObject;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        waitSec = new WaitForSeconds(.1f);
        waitShortTime = new WaitForSeconds(.01f);
        waitFix = new WaitForFixedUpdate();
        isHit = false;
        hitText = new List<GameObject>() { hitDamage.gameObject };
        isInit = false;
        isFire = false;
        isDash = false;

        fireTimer = 0f;
        fireInterval = 10f;
        fireDistance = 5f;
        fireCount = 0;
        fireMaxCount = 1;

        dashTimer = 0f;
        dashInterval = 5f;

        phase = 3;
        attackDistance = 1.3f;

        selectedObject = null;
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.isLive) return;

        if (!isLive || isHit)
            return;

        if (enemyName == "Goblin")
        {
            if (!CheckAnim(AnimType.Move) || isDash || isFire)
            {
                if (!isDash && !isFire)
                {
                    dashTimer += Time.fixedDeltaTime;
                    fireTimer += Time.fixedDeltaTime;
                }
                rigid.velocity = Vector2.zero;
                return;
            }
            else
            {
                PhaseController(phase);
            }
        }
    }

    void PhaseController(int phase)
    {
        if (phase > 0)
        {
            // 대시
            dashTimer += Time.fixedDeltaTime;
            if (dashTimer > dashInterval)
            {
                StartCoroutine(DashCoroutine());
                return;
            }

            if (phase > 1)
            {
                fireTimer += Time.fixedDeltaTime;
                if (fireTimer > fireInterval && (target.position - rigid.position).magnitude < fireDistance)
                {
                    StartCoroutine(FireCoroutine());
                    return;
                }
            }
        }

        if ((target.position - rigid.position).magnitude < attackDistance &&
            target.position.y + 1f > rigid.position.y)
        {
            if (phase > 2)
            {
                anim.speed = 1.5f;
            }
            else
            {
                anim.speed = 1f;
            }
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) anim.SetTrigger("Attack");
            rigid.velocity = Vector2.zero;
            return;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Move"))
        {
            Vector2 dirVec = target.position - rigid.position;
            Vector2 nextVec = dirVec.normalized * speed / 5 * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);
            rigid.velocity = Vector2.zero;
        }
    }

    IEnumerator FireCoroutine()
    {
        isFire = true;
        fireCount = 1;
        fireTextBox.SetActive(true);
        if (phase > 2)
        {
            anim.speed = 1.5f;
            anim.SetTrigger("Idle");
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            anim.speed = 1f;
            anim.SetTrigger("Idle");
            yield return new WaitForSeconds(1f);
        }

        fireTextBox.GetComponent<Animator>().SetTrigger("Out");
        anim.SetTrigger("Fire");
    }

    void FireFinished()
    {
        if (fireCount == fireMaxCount)
        {
            isFire = false;
            fireCount = 0;
            fireTimer = 0f;
            anim.speed = 1f;
            anim.SetTrigger("Idle");
            fireTextBox.SetActive(false);
            StartCoroutine(DelayAfterAttackCoroutine());
        }
        else if (fireCount < fireMaxCount)
        {
            fireCount++;
            anim.SetTrigger("Fire");
        }
    }
    
    bool CheckAnim(AnimType animType)
    {
        string state;
        switch (animType)
        {
            case AnimType.Attack:
                state = "Attack";
                break;
            case AnimType.Move:
                state = "Move";
                break;
            case AnimType.Fire:
                state = "Fire";
                break;
            case AnimType.Idle:
                state = "Idle";
                break;
            default:
                Debug.Log($"알 수 없는 AnimType이 입력되었습니다. AnimType: {animType}");
                return false;
        }
        return anim.GetCurrentAnimatorStateInfo(0).IsName(state);
    }

    IEnumerator DashCoroutine()
    {
        isDash = true;
        dashTextBox.SetActive(true);
        float boost;
        if (phase > 2)
        {
            anim.speed = 1.5f;
            anim.SetTrigger("Idle");
            yield return new WaitForSeconds(0.5f);
            boost = 6f;
        }
        else
        {
            anim.speed = 1f;
            anim.SetTrigger("Idle");
            yield return new WaitForSeconds(1f);
            boost = 5f;
        }
        
        dashTextBox.GetComponent<Animator>().SetTrigger("Out");
        goblinDashEffect.SetActive(true);
        anim.SetTrigger("Move");
        while (true)
        {
            Vector2 destPos = spriter.flipX ? new Vector2(target.position.x + 0.7f, target.position.y - 0.35f)
                                            : new Vector2(target.position.x - 0.7f, target.position.y - 0.35f);
            Vector2 dirVec = destPos - rigid.position;
            Vector2 nextVec = dirVec.normalized * speed * boost / 5 * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);
            rigid.velocity = Vector2.zero;
            yield return new WaitForFixedUpdate();
            if (Mathf.Abs(destPos.x - rigid.position.x) < 0.1f && Mathf.Abs(rigid.position.y - destPos.y) < 0.1f)
            {
                isDash = false;
                dashTimer = 0f;
                anim.speed = 1f;
                dashTextBox.SetActive(false);
                goblinDashEffect.SetActive(false);
                yield break;
            }
        }
    }

    // Attack Animation에서 사용
    void Attack(int turnOnIndex)
    {
        if (turnOnIndex == 1)
        {
            selectedObject = null;
            for (int i = 0; i < PoolManager.instance.prefabs.Length; i++)
            {
                if (PoolManager.instance.prefabs[i].CompareTag("GoblinMelee"))
                {
                    selectedObject = PoolManager.instance.Get(i);
                }
            }
            if (selectedObject == null)
            {
                Debug.Log("GoblinMelee Tag를 갖는 Prefab을 Pool에서 찾을 수 없습니다.");
                return;
            }
            selectedObject.transform.parent = PoolManager.instance.transform.GetChild(1);
            float angleY = spriter.flipX ? 180f : 0f;
            selectedObject.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0f, angleY, 0f));
            selectedObject.GetComponent<EnemyProjectile>().Init(EnemyProjectile.projectileType.GoblinMelee, Vector3.zero, 0f);
        }
        else if (turnOnIndex == 2)
        {
            if (selectedObject == null)
            {
                Debug.Log("현재 참조 중인 GoblinMelee Projectile Pool이 없습니다.");
                return;
            }
            selectedObject.GetComponent<EnemyProjectile>().Done();

            selectedObject = null;
            for (int i = 0; i < PoolManager.instance.prefabs.Length; i++)
            {
                if (PoolManager.instance.prefabs[i].CompareTag("GoblinMeleeDown"))
                {
                    selectedObject = PoolManager.instance.Get(i);
                }
            }
            if (selectedObject == null)
            {
                Debug.Log("GoblinMelee Tag를 갖는 Prefab을 Pool에서 찾을 수 없습니다.");
                return;
            }
            selectedObject.transform.parent = PoolManager.instance.transform.GetChild(1);
            float angleY = spriter.flipX ? 180f : 0f;
            selectedObject.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0f, angleY, 0f));
            selectedObject.GetComponent<EnemyProjectile>().Init(EnemyProjectile.projectileType.GoblinMelee, Vector3.zero, 0f);
        }
        else
        {
            if (selectedObject == null)
            {
                Debug.Log("현재 참조 중인 GoblinMelee Projectile Pool이 없습니다.");
                return;
            }
            selectedObject.GetComponent<EnemyProjectile>().Done();
        }
    }

    void AttackFinished()
    {
        anim.speed = 1f;
        anim.SetTrigger("Idle");
        StartCoroutine(DelayAfterAttackCoroutine());
    }

    IEnumerator DelayAfterAttackCoroutine()
    {
        if (phase > 2)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.8f);
        }
        anim.SetTrigger("Move");
    }

    // Fire Animation에서 사용
    void Fire()
    {
        selectedObject = null;
        for (int i = 0; i < PoolManager.instance.prefabs.Length; i++)
        {
            if (PoolManager.instance.prefabs[i].CompareTag("FireBall"))
            {
                selectedObject = PoolManager.instance.Get(i);
            }
        }
        if (selectedObject == null)
        {
            Debug.Log("EnemyProjectile 레이어를 갖는 Prefab을 Pool에서 찾을 수 없습니다.");
            return;
        }
        selectedObject.transform.parent = PoolManager.instance.transform.GetChild(1);
        var rot = Quaternion.FromToRotation(Vector2.down, target.position + new Vector2(0f, 0.5f) - rigid.position - new Vector2(0, 0.7f));
        selectedObject.transform.position = (rot * Vector3.down) * 0.6f + (Vector3)rigid.position + new Vector3(0, 0.7f, 0);
        selectedObject.transform.rotation = rot;

        Vector3 dir = (Vector3)target.position + new Vector3(0, 0.5f, 0) - selectedObject.transform.position;
        selectedObject.GetComponent<EnemyProjectile>().Init(EnemyProjectile.projectileType.FireBall, dir.normalized, 7f);
    }

    private void LateUpdate()
    {
        if (!GameManager.instance.isLive) return;

        if (!isLive) return;

        if (phase > 2)
        {
            dashInterval = 3f;
            fireInterval = 6f;
            fireMaxCount = 5;
        }

        if (lookLeft)
        {
            spriter.flipX = target.position.x > rigid.position.x;
        }
        else
        {
            spriter.flipX = target.position.x < rigid.position.x;
        }

        hpBar.value = health / maxHealth;
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
        fireTimer = 0f;
        dashTimer = 0f;
        isFire = false;
        isDash = false;

        if (dashTextBox.activeSelf) dashTextBox.SetActive(false);
        if (fireTextBox.activeSelf) fireTextBox.SetActive(false);
        if (goblinDashEffect.activeSelf) goblinDashEffect.SetActive(false);

        Init(enemyData);

        anim.SetTrigger("Move");
    }

    void Init(EnemyData data)
    {
        enemyName = data.enemyName;
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        exp = data.exp;
        rigid.mass = data.mass;

        lookLeft = data.lookLeft;

        dropItems = new DropItems[data.dropItems.Length];
        for (int i = 0; i < data.dropItems.Length; i++)
        {
            dropItems[i] = data.dropItems[i];
            dropItems[i].probability /= 100f;
        }

        hitTextPosXrange = hpBar.GetComponent<RectTransform>().sizeDelta.x / 4;
        hitTextPosYstart = hpBar.GetComponent<RectTransform>().anchoredPosition.y * 0.9f;
        hitTextPosYend = hpBar.GetComponent<RectTransform>().anchoredPosition.y * 1.25f;

        isInit = true;
        StartCoroutine(IsInitOffCoroutine());

        if (enemyName == "Bat" || enemyName == "BlueBird")
        {
            CollisionIgnore(true);
        }
        else
        {
            CollisionIgnore(false);
        }
    }

    IEnumerator IsInitOffCoroutine()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        if (isInit) { isInit = false; }
    }

    void CollisionIgnore(bool ignore)
    {
        Grid[] allGrids = GameObject.FindWithTag("Ground").transform.parent.parent.GetComponentsInChildren<Grid>();
        foreach (var allGrid in allGrids)
        {
            if (allGrid.CompareTag("OnlyFlyingPass"))
            {
                var colliders = allGrid.GetComponentsInChildren<Collider2D>();
                foreach (var item in colliders)
                {
                    Physics2D.IgnoreCollision(coll, item, ignore);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isLive) return;
        if (!collision.CompareTag("PierceTrap")) return;
        if (isHit) return;
        if (!collision.GetComponent<Trap>().IsOn()) return;
        health -= 5f;
        HitDamageText(5f);

        if (!hpBar.gameObject.activeSelf) hpBar.gameObject.SetActive(true);

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

            if (GameManager.instance.isLive)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInit)
        {
            if (!collision.CompareTag("AltarArea")) return;
            var spawnPoints = collision.GetComponentsInChildren<Transform>();
            transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
            isInit = false;
            return;
        }

        if (!isLive) return;
        if (!collision.CompareTag("Projectile") && !collision.CompareTag("Skill"))
            return;
        if (collision.CompareTag("Projectile") && !collision.GetComponent<Projectile>().StillLive())
            return;

        if (collision.CompareTag("Projectile"))
        {
            collision.GetComponent<Projectile>().Pierce(transform);

            health -= collision.GetComponent<Projectile>().damage;
            HitDamageText(collision.GetComponent<Projectile>().damage);
        }
        else if (collision.CompareTag("Skill"))
        {
            if (collision.GetComponent<Skill>().hitList.Contains(gameObject)) return;
            collision.GetComponent<Skill>().SetHitList(gameObject);
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
            var newTextObject = Instantiate(hitText[0], hpBar.transform.parent);
            hitText.Add(newTextObject);
            //newTextObject.transform.parent = hpBar.transform.parent;
            selectId = hitText.Count - 1;
        }
        hitText[selectId].SetActive(true);
        hitText[selectId].GetComponent<Text>().text = damage.ToString("N0") + "<size=12> 피해</size>";
        hitText[selectId].GetComponent<Text>().color = Color.red;
        //hitText[selectId].transform.localScale = new Vector3(1f, 1f, 1f);
        Vector2 textPos = new(Random.Range(-hitTextPosXrange, hitTextPosXrange), hitTextPosYstart);
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
        yield return new WaitForFixedUpdate();
        rigid.velocity = Vector2.zero;
        Vector2 dirVec = rigid.position - target.position;
        rigid.AddForce(dirVec.normalized * force, ForceMode2D.Impulse);
        spriter.material.SetFloat("_FlashAmount", 0.5f);
        yield return new WaitForFixedUpdate();
        spriter.material.SetFloat("_FlashAmount", 0.75f);
        yield return new WaitForFixedUpdate();
        spriter.material.SetFloat("_FlashAmount", 1.0f);
        yield return new WaitForFixedUpdate();
        spriter.material.SetFloat("_FlashAmount", 0.75f);
        yield return new WaitForFixedUpdate();
        spriter.material.SetFloat("_FlashAmount", 0.5f);
        yield return new WaitForFixedUpdate();
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

    void DropItem()
    {
        if (dropItems.Length == 0) return;

        float totalProbability = 0f;

        foreach (var dropItem in dropItems)
        {
            totalProbability += dropItem.probability;
        }

        float randValue = Random.Range(0, Mathf.Max(1f, totalProbability));
        float accumulatedValue = 0f;
        int selectedItemId = -1;
        for (int i = 0; i < dropItems.Length; i++)
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
