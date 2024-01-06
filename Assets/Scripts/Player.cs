using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public static Player instance;

    public Vector2 inputVector;
    public RuntimeAnimatorController[] animCon;
    public Transform shadow;
    public RangeWeapon rangeWeapon;
    public Collider2D[] attackColl;
    public InputActionAsset actions;
    public ParticleSystem[] chargeEffects;
    public ParticleSystemRenderer[] dodgeEffects;
    public GameObject[] skills;
    public GameObject skillArrow;
    public GameObject rangeArrow;
    public GameObject rightBash;
    public GameObject leftBash;
    public Animator anim;

    InputAction moveAction;
    InputAction fireAction;
    InputAction dodgeAction;
    InputAction rangeWeaponAction;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    WaitForSeconds waitSec;
    WaitForFixedUpdate waitFix;

    Vector3 skillDir;
    Vector3 rangeDir;
    public float chargeTimer;
    public int chargeCount; // ���� ��밡���� ��ų ī��Ʈ�� �ٸ�. ���� �÷��̾ ������ ��ų ��.
    bool isRight;
    bool isAttack;
    public bool isCharging;
    bool isDodge;
    //Hit ���� ����
    public bool isHit;
    public bool readyDodge;
    Rigidbody2D target;
    bool canRangeFire;
    bool isImmune;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);


        // �ڷ�ƾ ����Ͽ� chargeTimer ���� ��Ų �� 
        moveAction = actions.FindActionMap("Player").FindAction("Move");
        fireAction = actions.FindActionMap("Player").FindAction("Fire");
        dodgeAction = actions.FindActionMap("Player").FindAction("Dodge");
        rangeWeaponAction = actions.FindActionMap("Player").FindAction("RangeWeapon");

        chargeTimer = 0f;

        // �ʱ�ȭ
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        waitSec = new WaitForSeconds(.01f);
        waitFix = new WaitForFixedUpdate();

        PlayerActionAdd();
    }

    private void OnEnable()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Title" || scene.name == "Loading") gameObject.SetActive(false);

        readyDodge = true;
        isDodge = false;
        isAttack = false;
        isHit = false;
        isImmune = false;

        actions.Enable();

        for (int i = 0; i < chargeEffects.Length; i++)
        {
            chargeEffects[i].gameObject.SetActive(false);
        }


        if (GameManager.Instance is null) return;

        anim.runtimeAnimatorController = animCon[GameManager.Instance.playerId];
        if (GameManager.Instance.playerId == 0)
        {
            shadow.gameObject.SetActive(false);
        }
        else
        {
            shadow.gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        PlayerActionRemove();
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.isLive || isHit) return;
        if (GameManager.Instance.health < .1f) return;

        inputVector = moveAction.ReadValue<Vector2>();

        if (isDodge) return;
        Vector2 nextVec = inputVector * GameManager.Instance.playerSpeed / 5 * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }
    private void LateUpdate()
    {
        if (!GameManager.Instance.isLive || isHit) return;

        anim.SetFloat("Speed", inputVector.magnitude);

        if (inputVector.x != 0)
        {
            spriteRenderer.flipX = inputVector.x < 0;
        }

        Shadow();
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.Instance.isLive || isHit || isDodge || GameManager.Instance.health < 0.1f) return;
        // ���ظ鿪�� ���¸�?
        if (isImmune) return;

        // Layer 6: Enemy
        if (collision.gameObject.layer == 6)
        {
            target = collision.rigidbody;
            // Player Mass 5 -> 1000 ���� ����
            // ���� ������ �о �� �ֵ���? ���� ���̵� �ʹ� �����
            // Knockback�� ���Ǵ� force�� Mass �� 10�谡 ���� (Player ����)
            StartCoroutine(KnockBack(10000));
            AudioManager.instance.PlaySfx(AudioManager.Sfx.PlayerHit);
            GameManager.Instance.health -= 0.5f;
        }

        if (GameManager.Instance.health < 0.1f)
        {
            //for (int i = 2; i < transform.childCount; i++)
            //{
            //    transform.GetChild(i).gameObject.SetActive(false);
            //}

            //Time.timeScale = .5f;
            //anim.SetTrigger("Dead");

            StartCoroutine(DeadCoroutine());

            //GameManager.Instance.GameOver();
        }
    }

    IEnumerator DeadCoroutine()
    {
        GameManager.Instance.CameraDamping(0f);

        int targetPPU = 66;
        float timer = 0f;
        Time.timeScale = 0f;

        GameObject.FindGameObjectWithTag("Light").GetComponent<Light2D>().color = Color.white;

        while (timer < 1.5f)
        {
            GameManager.Instance.ZoomCamera(GameManager.Instance.originPPU + Mathf.FloorToInt((targetPPU - GameManager.Instance.originPPU) * timer / 2));
            yield return null;
            timer += Time.unscaledDeltaTime;
        }

        Time.timeScale = 1f;

        GameManager.Instance.CameraDamping();

        // ��Ȱ�� ����� ���� �� ����̴� �ı��ǰ� ���� �ʰ� ü�� +2 ȹ��
        // �����̸鼭 �׵��� ������ �Ǵ� �ڷ�ƾ ����

        //GameManager.Instance.health += 2;
        //GameManager.Instance.ZoomCamera(GameManager.Instance.originPPU);
        //AudioManager.instance.PauseBGM(false);
        //yield break;

        anim.SetTrigger("Dead");
    }

    public void DeadEnd()
    {
        StartCoroutine(DeadEndCoroutine());
    }

    IEnumerator DeadEndCoroutine()
    {
        List<int> itemList = new List<int>();

        for (int i = 0; i < GameManager.Instance.inventoryItemsId.Length; i++)
        {
            if (GameManager.Instance.inventoryItemsId[i] != -1)
            {
                itemList.Add(GameManager.Instance.inventoryItemsId[i]);
                GameManager.Instance.inventoryItemsId[i] = -1;
            }
        }
        if (GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId] != -1)
        {
            itemList.Add(GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId]);
            GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId] = -1;
        }
        if (GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId] != -1)
        {
            itemList.Add(GameManager.Instance.necklaceItem[GameManager.Instance.playerId]);
            GameManager.Instance.necklaceItem[GameManager.Instance.playerId] = -1;
        }
        if (GameManager.Instance.mainWeaponItem[GameManager.Instance.playerId] != -1)
        {
            itemList.Add(GameManager.Instance.shoesItem[GameManager.Instance.playerId]);
            GameManager.Instance.shoesItem[GameManager.Instance.playerId] = -1;
        }
        if (GameManager.Instance.rangeWeaponItem != -1)
        {
            itemList.Add(GameManager.Instance.rangeWeaponItem);
            GameManager.Instance.rangeWeaponItem = -1;
        }
        if (GameManager.Instance.magicItem != -1)
        {
            itemList.Add(GameManager.Instance.magicItem);
            GameManager.Instance.magicItem = -1;
        }

        if (itemList.Count == 0)
        {
            GameManager.Instance.GameOver();
            yield break;
        }

        yield return new WaitForSecondsRealtime(0.3f);
        foreach (var itemId in itemList)
        {
            int prefabId = -1;
            for (int i = 0; i < PoolManager.instance.prefabs.Length; i++)
            {
                if (PoolManager.instance.prefabs[i] == ItemManager.Instance.itemDataArr[itemId].dropItem)
                {
                    prefabId = i;
                    break;
                }
            }
            if (prefabId == -1)
            {
                Debug.Log($"��� �����ۿ� �ش��ϴ� PrefabId�� Pool���� ã�� �� �����ϴ�. itemId: {itemId} ");
                continue;
            }

            GameObject selectedItem = PoolManager.instance.Get(prefabId);
            selectedItem.transform.parent = PoolManager.instance.transform.GetChild(2);
            selectedItem.transform.position = transform.position;
            selectedItem.GetComponent<DropItem>().itemId = itemId;
            selectedItem.GetComponent<DropItem>().Scatter();
        }
        yield return new WaitForSecondsRealtime(0.3f);
        GameManager.Instance.GameOver();
    }


    // �˹��� �Ϸ�ǰ� ������ �����ð� ���� �ο�
    IEnumerator KnockBack(int force)
    {
        isHit = true;
        spriteRenderer.color = new Color(1f, 1f, 1f, .7f);
        spriteRenderer.material.SetColor("_FlashColor", new Color(1, 1, 1, 0));
        spriteRenderer.material.SetFloat("_FlashAmount", 0.25f);
        yield return waitSec;
        rigid.velocity = Vector2.zero;
        Vector2 dirVec = rigid.position - target.position;
        rigid.AddForce(dirVec.normalized * force, ForceMode2D.Impulse);
        spriteRenderer.material.SetFloat("_FlashAmount", 0.5f);
        yield return waitSec;
        spriteRenderer.material.SetFloat("_FlashAmount", 0.75f);
        yield return waitSec;
        spriteRenderer.material.SetFloat("_FlashAmount", 1.0f);
        yield return waitSec;
        spriteRenderer.material.SetFloat("_FlashAmount", 0.75f);
        yield return waitSec;
        spriteRenderer.material.SetFloat("_FlashAmount", 0.5f);
        yield return waitSec;
        spriteRenderer.material.SetFloat("_FlashAmount", 0f);
        rigid.velocity = Vector2.zero;
        yield return new WaitForSeconds(.1f);
        isHit = false;

        // ���� �鿪 �߰�
        isImmune = true;
        yield return new WaitForSeconds(GameManager.Instance.playerImmuneTime);
        spriteRenderer.color = Color.white;
        isImmune = false;
    }

    void Shadow()
    {
        switch (GameManager.Instance.playerId)
        {
            case 0:
                if (shadow.gameObject.activeSelf)
                {
                    shadow.gameObject.SetActive(false);
                }
                break;

            default:
                if (!shadow.gameObject.activeSelf)
                {
                    shadow.gameObject.SetActive(true);
                }

                //if (spriteRenderer.flipX)
                //{
                //    shadow.localPosition = new Vector3(-0.033f, 0.005f, 0);
                //}
                //else
                //{
                //    shadow.localPosition = new Vector3(0.033f, 0.005f, 0);
                //}
                if (spriteRenderer.flipX)
                {
                    shadow.localPosition = new Vector3(-0.015f, 0.02f, 0);
                }
                else
                {
                    shadow.localPosition = new Vector3(0.015f, 0.02f, 0);
                }
                break;

        }
    }

    void OnDodge()
    {
        if (!GameManager.Instance.isLive) return;
        if (GameManager.Instance.health < .1) return;
        if (inputVector.magnitude == 0) return;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") || anim.GetCurrentAnimatorStateInfo(0).IsName("SkillMotion") || anim.GetCurrentAnimatorStateInfo(0).IsName("Dead") || isHit || isDodge)
            return;
        //if (anim.GetCurrentAnimatorStateInfo(0).IsName("Dead") || isHit || isDodge)
        //return;
        if (!readyDodge) return;

        StartCoroutine("Dodge");
    }

    IEnumerator Dodge()
    {
        readyDodge = false;
        isDodge = true;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Dodge);
        dodgeEffects[GameManager.Instance.playerId].gameObject.SetActive(true);

        if (spriteRenderer.flipX)
        {
            dodgeEffects[GameManager.Instance.playerId].flip = new Vector3(1f, 0f, 0f);
        }
        else
        {
            dodgeEffects[GameManager.Instance.playerId].flip = new Vector3(0f, 0f, 0f);
        }

        //Dodge ���� ������
        //Color currColor = spriteRenderer.color;
        //currColor.a = .6f;
        //spriteRenderer.color = currColor;

        rigid.velocity = inputVector * GameManager.Instance.dodgeSpeed;
        // 0.2 ��
        yield return new WaitForSeconds(.3f);

        //Dodge ���� ������
        //currColor.a = 1f;
        //spriteRenderer.color = currColor;

        isDodge = false;
        rigid.velocity = Vector2.zero;
        dodgeEffects[GameManager.Instance.playerId].gameObject.SetActive(false);
    }

    void Attack(int playerId)
    {
        // ���� �⿡ ���� ���� ���ϰ� ������ ��ȭ ���� �߰��ؾ���
        if (!isAttack)
        {
            if (!spriteRenderer.flipX)
            {
                attackColl[playerId * 2].gameObject.SetActive(true);
                isRight = true;
            }
            else
            {
                attackColl[playerId * 2 + 1].gameObject.SetActive(true);
                isRight = false;
            }
            isAttack = true;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.WarriorAttack);
        }
        else
        {
            if (isRight)
            {
                attackColl[playerId * 2].gameObject.SetActive(false);
            }
            else
            {
                attackColl[playerId * 2 + 1].gameObject.SetActive(false);
            }
            isAttack = false;
        }
    }

    void StartCharging()
    {
        if (!GameManager.Instance.isLive) return;
        if (GameManager.Instance.health < .1) return;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") || anim.GetCurrentAnimatorStateInfo(0).IsName("SkillMotion") || anim.GetCurrentAnimatorStateInfo(0).IsName("Dead") || isHit)
            return;

        StartCoroutine("StartCharge");
    }

    float originSpeed;
    IEnumerator StartCharge()
    {
        float chargeStartTimer = 0f;

        while (true)
        {
            chargeStartTimer += Time.fixedDeltaTime;
            if (chargeStartTimer > .3f) break;
            yield return waitFix;
        }

        if (GameManager.Instance.chargeCount == 0)
        {
            StartCoroutine(FailMotion());
            yield break;
        }

        isCharging = true;
        chargeEffects[0].gameObject.SetActive(true);
        originSpeed = GameManager.Instance.playerSpeed;
        // �̵��ӵ� ����
        GameManager.Instance.playerSpeed = originSpeed * .8f;

        // �� ������ Effect �߰�
        StartCoroutine("Charging");
    }

    IEnumerator FailMotion()
    {
        Vector3 deltaVec = new Vector3(.05f, 0f, 0f);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
        for (int i = 0; i < 3; i++)
        {
            //transform.localPosition += deltaVec;
            //yield return waitSec;
            //transform.localPosition -= deltaVec;
            //yield return waitSec;
            //transform.localPosition -= deltaVec;
            //yield return waitSec;
            //transform.localPosition += deltaVec;
            //yield return waitSec;

            rigid.position = rigid.position + new Vector2(deltaVec.x, deltaVec.y);
            yield return waitSec;
            rigid.position = rigid.position - new Vector2(deltaVec.x, deltaVec.y);
            yield return waitSec;
            rigid.position = rigid.position - new Vector2(deltaVec.x, deltaVec.y);
            yield return waitSec;
            rigid.position = rigid.position + new Vector2(deltaVec.x, deltaVec.y);
            yield return waitSec;
        }
    }


    /// <summary>
    /// 0 : performed, 1: canceled
    /// </summary>
    void ChargedFire(int status)
    {
        if (GameManager.Instance.health < .1) return;

        if (status == 1)
        {
            // ��¡ �� interrupt�� ���� cancel �� ���? performed�� ������ �۾�
            if (!isCharging) StopCoroutine("StartCharge");
            if (GameManager.Instance.isLive) Fire();
            // 0.2 - 0.5 �� ������ Ű �Է��� �õ��� ��� - �� ���� ���� ��������
            return;
        }

        if (!isCharging)
        {
            StopCoroutine("StartCharge");
            return;   // ������ ��¡�� �õ��ϰ� Ű�� ���� �� ���� �� �� ������ �� ����,
        }

        if (!isHit && GameManager.Instance.isLive)
        {
            // �� 3 ĭ���� ������ �� ī��Ʈ ���
            switch (chargeCount)
            {
                case 0:

                    Fire();
                    break;
                case 1:
                    AttackSkill(0);
                    anim.SetTrigger("SkillMotion");
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.WarriorSkill);
                    GameManager.Instance.chargeCount--;
                    break;
                case 2:
                    anim.SetTrigger("SkillMotion");
                    AttackSkill(1);
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.WarriorSkill);
                    GameManager.Instance.chargeCount -= 2;
                    break;
                case 3:
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.WarriorSkill);
                    GameManager.Instance.chargeCount -= 3;
                    break;
            }
            StopCoroutine("Charging");
        }



        //�� �ǵ�����

        chargeEffects[Mathf.Min(chargeCount, 2)].gameObject.SetActive(false);
        isCharging = false;
        chargeTimer = 0;
        chargeCount = 0;
        GameManager.Instance.playerSpeed = originSpeed;
        //��ų ���� ���� �ߴ�
        StopCoroutine("SkillArrow");
        skillArrow.transform.localRotation = Quaternion.identity;
        skillArrow.SetActive(false);

    }

    void Fire()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") || anim.GetCurrentAnimatorStateInfo(0).IsName("SkillMotion") || anim.GetCurrentAnimatorStateInfo(0).IsName("Dead") || isHit)
            return;

        anim.SetTrigger("Attack");
    }

    IEnumerator Charging()
    {
        float totalTime = 0f;
        chargeTimer = 0;
        while (true)
        {
            if (isHit || (!GameManager.Instance.isLive))
            {
                ChargedFire(0);
                break;
            }


            if (GameManager.Instance.chargeCount == chargeCount || chargeCount == GameManager.Instance.maxChargibleCount)
            {
                if (chargeCount < 3)
                {
                    chargeEffects[chargeCount].gameObject.SetActive(false);
                }
                yield return null;
                continue;
            }

            if (chargeCount == 1)
            {
                GameManager.Instance.playerSpeed = originSpeed * 0.6f;
            }
            else if (chargeCount == 2)
            {
                GameManager.Instance.playerSpeed = originSpeed * 0.4f;
            }


            if (chargeCount < 3 && !chargeEffects[chargeCount].gameObject.activeSelf)
            {
                chargeEffects[chargeCount].gameObject.SetActive(true);
            }

            if (chargeTimer > GameManager.Instance.chargeTime && chargeCount < GameManager.Instance.maxChargibleCount)
            {
                if (chargeCount < 2) chargeEffects[chargeCount].gameObject.SetActive(false);
                chargeCount++;

                if (!chargeEffects[Mathf.Min(2, chargeCount)].gameObject.activeSelf)
                {
                    chargeEffects[Mathf.Min(2, chargeCount)].gameObject.SetActive(true);
                }

                //if (chargeCount < 3)
                //{
                //    chargeEffects[chargeCount].gameObject.SetActive(true);
                //}
                chargeTimer = 0;
                if (chargeCount == 1) StartCoroutine("PlayerBlink");
            }

            // ��ų ���� ���� : 1ȸ ������� �������� ���̵���.

            if (totalTime > GameManager.Instance.chargeTime)
            {
                if (GameManager.Instance.playerId == 0 && chargeCount > 1)
                {
                    StartCoroutine("SkillArrow");
                }
            }

            yield return null;
            chargeTimer += Time.fixedDeltaTime;
            totalTime += Time.fixedDeltaTime;
        }
    }

    IEnumerator PlayerBlink()
    {
        Color origin = new Color(1, 1, 1, 0);
        Color applyColor = new Color(1, 1, 1, 0);
        //Color skill_1 = new Color(1, 1, 0, 0);
        //Color skill_2 = new Color(1, .7f, 0, 0);
        //Color skill_3 = new Color(1, .4f, 0, 0);
        float skill_1_flashAmount = .5f;
        float skill_2_flashAmount = .5f;
        float skill_3_flashAmount = .5f;
        float targetFlashAmount;

        while (chargeCount != 0)
        {
            int frameCount;
            int count = 1;
            switch (chargeCount)
            {
                case 1:
                    targetFlashAmount = skill_1_flashAmount;
                    frameCount = 20;
                    break;
                case 2:
                    targetFlashAmount = skill_2_flashAmount;
                    frameCount = 15;
                    break;
                case 3:
                    targetFlashAmount = skill_3_flashAmount;
                    frameCount = 10;
                    break;
                default:
                    yield break;
            }


            float flashAmountUnit = targetFlashAmount / frameCount;
            float currFlashAmount;
            spriteRenderer.material.SetColor("_FlashColor", applyColor);
            while (count <= frameCount)
            {
                if (chargeCount == 0) break;
                currFlashAmount = flashAmountUnit * count;
                spriteRenderer.material.SetFloat("_FlashAmount", currFlashAmount);
                yield return waitSec;
                count++;
            }
            count -= 2;
            while (count > 0)
            {
                if (chargeCount == 0) break;
                currFlashAmount = flashAmountUnit * count;
                spriteRenderer.material.SetFloat("_FlashAmount", currFlashAmount);
                yield return waitSec;
                count--;
            }

            spriteRenderer.material.SetColor("_FlashColor", origin);
            spriteRenderer.material.SetFloat("_FlashAmount", 0f);
            yield return waitSec;
        }
    }



    // ��ų ���� ȭ��ǥ �ε巯�� ��ȯ ���� �� �����Ӵ� ȸ�� ����
    IEnumerator SkillArrow()
    {
        skillArrow.SetActive(true);

        while (true)
        {
            if (inputVector.magnitude > 0)
            {
                skillDir = inputVector;
            }
            else if (spriteRenderer.flipX)
            {
                skillDir = Vector3.left;
            }
            else
            {
                skillDir = Vector3.right;
            }

            skillArrow.transform.localRotation = Quaternion.FromToRotation(Vector3.right, skillDir);
            yield return waitFix;
        }
    }


    void AttackSkill(int level)
    {
        switch (level)
        {
            case 0:
                rightBash.transform.parent.gameObject.SetActive(true);
                if (!spriteRenderer.flipX)
                {
                    rightBash.SetActive(true);
                }
                else
                {
                    leftBash.SetActive(true);
                }
                break;

            case 1:
                Rigidbody2D skillRigid;
                skillRigid = skills[GameManager.Instance.playerId].transform.GetChild(level).GetComponentsInChildren<Rigidbody2D>(true)[0];
                skillRigid.transform.parent.localRotation = Quaternion.FromToRotation(Vector3.right, skillDir);
                skillRigid.transform.parent.gameObject.SetActive(true);
                skillRigid.velocity = skillDir * 15;
                StartCoroutine(StopSkill(skillRigid));
                break;
        }
    }

    public void StopBash()
    {
        if (rightBash.activeSelf) rightBash.SetActive(false);
        if (leftBash.activeSelf) leftBash.SetActive(false);
        rightBash.transform.parent.gameObject.SetActive(false);
    }

    IEnumerator StopSkill(Rigidbody2D skillRigid)
    {
        yield return new WaitForSeconds(.6f);
        skillRigid.velocity = Vector3.zero;
        skillRigid.transform.parent.gameObject.SetActive(false);
        skillRigid.transform.localPosition = Vector3.zero;
        skillRigid.transform.parent.localRotation = Quaternion.identity;
    }


    void StartRangeWeapon()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") || anim.GetCurrentAnimatorStateInfo(0).IsName("SkillMotion") || anim.GetCurrentAnimatorStateInfo(0).IsName("Dead") || isHit)
            return;
        //if (anim.GetCurrentAnimatorStateInfo(0).IsName("Dead") || isHit)
        //    return;
        if (!GameManager.Instance.isLive) return;
        if (GameManager.Instance.health < .1) return;
        // ����ϰ� ������ ����
        if (GameManager.Instance.rangeWeaponItem == -1)
            return;
        // ��¡ ĵ�� �����ϵ��� start Action���� �ű⵵��. ���ظ� �ص� ��¡ ���.
        // �ϴ� �Ʒ� ��� �ӽ÷� ���.
        // ��� ��ų ��¡�� �����ϴ� �� �� �׼Ǽ��� ������ �� ���� ���̶� �׽�Ʈ �غ��� �Ǵ��ϰ� ��.
        //if (isCharging)
        //{
        //    ChargedFire(1);
        //}

        if (rangeWeapon.readyRangeWeapon)
        {
            canRangeFire = true;
            //���� ����
            StartCoroutine("RangeArrow");
            //���� �� �ٸ� Ű�Է� ���� ��� (���ݹ�ư, ������ư.)

        }
        else
        {
            StartCoroutine(FailMotion());
            // �غ� �� ������ ���и�� ���� ����
        }

    }

    IEnumerator RangeArrow()
    {
        rangeArrow.SetActive(true);

        while (true)
        {
            if (!GameManager.Instance.isLive) break;
            if (inputVector.magnitude > 0)
            {
                rangeDir = inputVector;
            }
            else if (spriteRenderer.flipX)
            {
                rangeDir = Vector3.left;
            }
            else
            {
                rangeDir = Vector3.right;
            }

            rangeArrow.transform.localEulerAngles = new Vector3(0f, 0f, Vector3.SignedAngle(Vector3.right, rangeDir, Vector3.forward));
            yield return waitFix;
        }
    }

    void OnRangeWeapon(bool ready)
    {
        if (GameManager.Instance.health < .1) return;
        if (ready && GameManager.Instance.isLive)
        {
            rangeWeapon.Fire(rangeDir);
        }
        canRangeFire = false;
        StopCoroutine("RangeArrow");
        rangeArrow.transform.localRotation = Quaternion.identity;
        rangeArrow.SetActive(false);
    }

    public void PlayerActionRemove()
    {
        fireAction.started -= FireStartHandler;
        fireAction.performed -= FirePerformedHandler;
        fireAction.canceled -= FireCancelHandler;

        dodgeAction.performed -= DodgePerformedHandler;

        rangeWeaponAction.started -= RangeStartHandler;
        rangeWeaponAction.performed -= RangePerformedHandler;
        rangeWeaponAction.canceled -= RangeCancelHandler;
    }

    public void PlayerActionAdd()
    {
        fireAction.started += FireStartHandler;
        fireAction.performed += FirePerformedHandler;
        fireAction.canceled += FireCancelHandler;

        dodgeAction.performed += DodgePerformedHandler;

        rangeWeaponAction.started += RangeStartHandler;
        rangeWeaponAction.performed += RangePerformedHandler;
        rangeWeaponAction.canceled += RangeCancelHandler;
    }

    void FireStartHandler(InputAction.CallbackContext context)
    {
        StartCharging();
    }
    void FirePerformedHandler(InputAction.CallbackContext context)
    {
        ChargedFire(0);
    }
    void FireCancelHandler(InputAction.CallbackContext context)
    {
        ChargedFire(1);
    }
    void DodgePerformedHandler(InputAction.CallbackContext context)
    {
        OnDodge();
    }
    void RangeStartHandler(InputAction.CallbackContext context)
    {
        StartRangeWeapon();
    }
    void RangePerformedHandler(InputAction.CallbackContext context)
    {
        OnRangeWeapon(canRangeFire);
    }
    void RangeCancelHandler(InputAction.CallbackContext context)
    {
        OnRangeWeapon(canRangeFire);
    }
}
