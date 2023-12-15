using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    public float speed;
    public Vector2 inputVector;
    public Scanner scanner;
    public Hand[] hands;
    public RuntimeAnimatorController[] animCon;
    public Transform shadow;
    public Collider2D[] attackColl;
    public InputActionAsset actions;
    public ParticleSystem[] chargeEffects;
    public ParticleSystemRenderer[] dodgeEffects;
    public GameObject[] skills;
    public GameObject skillArrow;


    InputAction moveAction;
    InputAction fireAction;
    InputAction dodgeAction;
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    WaitForSeconds waitSec;
    WaitForFixedUpdate waitFix;

    public float chargeTimer;
    float chargeTime; // Gamanager ���� �Ҵ�
    public int chargeCount; // ���� ��밡���� ��ų ī��Ʈ�� �ٸ�. ���� �÷��̾ ������ ��ų ��.
    bool isRight;
    bool isAttack;
    public bool isCharging;
    bool isDodge;
    //Hit ���� ����
    public bool isHit;
    public bool readyDodge;
    Rigidbody2D target;


    private void Awake()
    {
        //Key �Է� ó��

        // �ڷ�ƾ ����Ͽ� chargeTimer ���� ��Ų �� 
        moveAction = actions.FindActionMap("Player").FindAction("Move");
        fireAction = actions.FindActionMap("Player").FindAction("Fire");
        dodgeAction = actions.FindActionMap("Player").FindAction("Dodge");

        fireAction.started += _ => StartCharging();
        fireAction.performed += _ => ChargedFire(0);
        fireAction.canceled += _ => ChargedFire(1);

        dodgeAction.performed += _ => { OnDodge(); };


        chargeTimer = 0f;

        // �ʱ�ȭ
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true);
        waitSec = new WaitForSeconds(.2f);
        waitFix = new WaitForFixedUpdate();
    }

    private void OnEnable()
    {
        readyDodge = true;
        isDodge = false;
        isAttack = false;
        isHit = false;
        chargeTime = GameManager.Instance.chargeTime;

        actions.Enable();

        for (int i = 0; i < chargeEffects.Length; i++)
        {
            chargeEffects[i].gameObject.SetActive(false);
        }


        speed *= Character.Speed;
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

    private void FixedUpdate()
    {
        if (!GameManager.Instance.isLive || isHit) return;

        inputVector = moveAction.ReadValue<Vector2>();

        if (isDodge) return;
        Vector2 nextVec = inputVector * speed * Time.fixedDeltaTime;
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
        if (!GameManager.Instance.isLive || isHit || isDodge) return;

        // Layer 6: Enemy
        if (collision.gameObject.layer == 6)
        {
            target = collision.rigidbody;
            StartCoroutine(KnockBack(50));
            AudioManager.instance.PlaySfx(AudioManager.Sfx.PlayerHit);
            GameManager.Instance.health -= 0.5f;
        }

        if (GameManager.Instance.health < 0.1f)
        {
            for (int i = 2; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            anim.SetTrigger("Dead");
            GameManager.Instance.GameOver();
        }
    }



    IEnumerator KnockBack(int force)
    {
        isHit = true;

        spriteRenderer.material.SetColor("_FlashColor", new Color(1, 1, 1, 0));
        spriteRenderer.material.SetFloat("_FlashAmount", 0.25f);
        yield return waitFix; // ���� �ϳ��� ���� ������
        rigid.velocity = Vector2.zero;
        Vector2 dirVec = rigid.position - target.position;
        rigid.AddForce(dirVec.normalized * force, ForceMode2D.Impulse);
        spriteRenderer.material.SetFloat("_FlashAmount", 0.5f);
        yield return waitFix; // ���� �ϳ��� ���� ������
        spriteRenderer.material.SetFloat("_FlashAmount", 0.75f);
        yield return waitFix; // ���� �ϳ��� ���� ������
        spriteRenderer.material.SetFloat("_FlashAmount", 1.0f);
        yield return waitFix;
        spriteRenderer.material.SetFloat("_FlashAmount", 0.75f);
        yield return waitFix;
        spriteRenderer.material.SetFloat("_FlashAmount", 0.5f);
        yield return waitFix;
        spriteRenderer.material.SetFloat("_FlashAmount", 0f);
        rigid.velocity = Vector2.zero;
        yield return new WaitForSeconds(.1f);
        isHit = false;
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
        if (inputVector.magnitude == 0) return;
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") || anim.GetCurrentAnimatorStateInfo(0).IsName("Dead") || isHit || isDodge)
            return;
        if (!readyDodge) return;

        StartCoroutine("Dodge");
    }

    IEnumerator Dodge()
    {
        float dodgeSpeed;
        dodgeSpeed = 7f;

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

        rigid.velocity = inputVector * dodgeSpeed;
        // 0.2 ��
        yield return new WaitForSeconds(.3f);

        //Dodge ���� ������
        //currColor.a = 1f;
        //spriteRenderer.color = currColor;

        isDodge = false;
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
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") || anim.GetCurrentAnimatorStateInfo(0).IsName("Dead") || isHit)
            return;

        StartCoroutine("StartCharge");
    }

    IEnumerator StartCharge()
    {
        float chargeStartTimer = 0f;

        while (true)
        {
            chargeStartTimer += Time.deltaTime;
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
        // �̵��ӵ� ����
        speed /= 2;

        // �� ������ Effect �߰�
        StartCoroutine("Charging");
    }

    IEnumerator FailMotion()
    {
        Vector3 deltaVec = new Vector3(0.05f, 0f, 0f);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
        for (int i = 0; i < 3; i++)
        {
            transform.localPosition += deltaVec;
            yield return waitFix;
            transform.localPosition -= deltaVec;
            yield return waitFix;
            transform.localPosition -= deltaVec;
            yield return waitFix;
            transform.localPosition += deltaVec;
            yield return waitFix;
        }
    }

    /// <summary>
    /// 0 : performed, 1: canceled
    /// </summary>
    void ChargedFire(int status)
    {
        if (status == 1)
        {
            // ��¡ �� interrupt�� ���� cancel �� ���? performed�� ������ �۾�

            if (!isCharging)
            {
                StopCoroutine("StartCharge");
                // 0.2 - 0.5 �� ������ Ű �Է��� �õ��� ��� - �� ���� ���� ��������
                Fire();
                return;
            }
        }

        if (!isCharging)
        {
            StopCoroutine("StartCharge");
            return;   // ������ ��¡�� �õ��ϰ� Ű�� ���� �� ���� �� �� ������ �� ����,
        }

        if (!isHit)
        {
            // �� 3 ĭ���� ������ �� ī��Ʈ ���
            switch (chargeCount)
            {
                case 0:

                    Fire();
                    break;
                case 1:
                    AttackSkill(0);
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.WarriorSkill);
                    GameManager.Instance.chargeCount--;
                    Debug.Log(chargeCount);
                    break;
                case 2:

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
        speed *= 2;
        //��ų ���� ���� �ߴ�
        StopCoroutine("SkillArrow");
        skillArrow.transform.localRotation = Quaternion.identity;
        skillArrow.SetActive(false);

    }

    void Fire()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") || anim.GetCurrentAnimatorStateInfo(0).IsName("Dead") || isHit)
            return;

        anim.SetTrigger("Attack");
    }

    IEnumerator Charging()
    {
        float totalTime = 0f;
        chargeTimer = 0;
        while (true)
        {
            if (isHit)
            {
                ChargedFire(0);
                break;
            }


            if (GameManager.Instance.chargeCount == chargeCount)
            {
                if (chargeCount < 3)
                {
                    chargeEffects[chargeCount].gameObject.SetActive(false);
                }
                yield return waitFix;
                continue;
            }

            if (chargeCount < 3 && !chargeEffects[chargeCount].gameObject.activeSelf)
            {
                chargeEffects[chargeCount].gameObject.SetActive(true);
            }

            if (chargeTimer > chargeTime && chargeCount < GameManager.Instance.maxChargibleCount)
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

            // ��ų ���� ���� : �ּ� 0.5 �� �̻� ������� �������� ���̵���.
            // �Ϲ� ���� �ÿ��� ���� ������ �� ����
            if (totalTime > chargeTime)
            {
                StartCoroutine("SkillArrow");
            }

            yield return waitFix;
            chargeTimer += Time.deltaTime;
            totalTime += Time.deltaTime;
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
                yield return waitFix;
                count++;
            }
            count -= 2;
            while (count > 0)
            {
                if (chargeCount == 0) break;
                currFlashAmount = flashAmountUnit * count;
                spriteRenderer.material.SetFloat("_FlashAmount", currFlashAmount);
                yield return waitFix;
                count--;
            }

            spriteRenderer.material.SetColor("_FlashColor", origin);
            spriteRenderer.material.SetFloat("_FlashAmount", 0f);
            yield return waitFix;
        }
    }



    // ��ų ���� ȭ��ǥ �ε巯�� ��ȯ ���� �� �����Ӵ� ȸ�� ����
    IEnumerator SkillArrow()
    {
        Vector3 dirAttack;
        skillArrow.SetActive(true);

        while (true)
        {
            if (inputVector.magnitude > 0)
            {
                dirAttack = inputVector;
            }
            else if (spriteRenderer.flipX)
            {
                dirAttack = Vector3.left;
            }
            else
            {
                dirAttack = Vector3.right;
            }

            skillArrow.transform.localRotation = Quaternion.FromToRotation(Vector3.right, dirAttack);
            yield return waitFix;
        }
    }


    void AttackSkill(int level)
    {


        Rigidbody2D skillRigid;
        skillRigid = skills[GameManager.Instance.playerId].GetComponentsInChildren<Rigidbody2D>(true)[level];

        Vector3 dirAttack;
        if (inputVector.magnitude > 0)
        {
            dirAttack = inputVector;
        }
        else if (spriteRenderer.flipX)
        {
            dirAttack = Vector3.left;
        }
        else
        {
            dirAttack = Vector3.right;
        }

        skillRigid.transform.localRotation = Quaternion.FromToRotation(Vector3.right, dirAttack);


        skillRigid.gameObject.SetActive(true);
        skillRigid.velocity = dirAttack * 10;
        StartCoroutine(StopSkill(skillRigid));

    }

    IEnumerator StopSkill(Rigidbody2D skillRigid)
    {
        yield return new WaitForSeconds(.9f);
        skillRigid.velocity = Vector3.zero;
        skillRigid.gameObject.SetActive(false);
        skillRigid.transform.localPosition = Vector3.zero;
        skillRigid.transform.localRotation = Quaternion.identity;
    }
}
