using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    public Vector2 inputVector;
    public Scanner scanner;
    public Hand[] hands;
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


    InputAction moveAction;
    InputAction fireAction;
    InputAction dodgeAction;
    InputAction rangeWeaponAction;
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    WaitForSeconds waitSec;
    WaitForFixedUpdate waitFix;

    Vector3 skillDir;
    Vector3 rangeDir;
    public float chargeTimer;
    public int chargeCount; // 현재 사용가능한 스킬 카운트와 다름. 실제 플레이어가 차지한 스킬 수.
    bool isRight;
    bool isAttack;
    public bool isCharging;
    bool isDodge;
    //Hit 관련 변수
    public bool isHit;
    public bool readyDodge;
    Rigidbody2D target;
    bool canRangeFire;


    private void Awake()
    {
        Player[] scripts = GameObject.FindObjectsByType<Player>(FindObjectsSortMode.None);
        if (scripts.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        //Key 입력 처리

        // 코루틴 사용하여 chargeTimer 누적 시킨 후 
        moveAction = actions.FindActionMap("Player").FindAction("Move");
        fireAction = actions.FindActionMap("Player").FindAction("Fire");
        dodgeAction = actions.FindActionMap("Player").FindAction("Dodge");
        rangeWeaponAction = actions.FindActionMap("Player").FindAction("RangeWeapon");

        fireAction.started += _ => StartCharging();
        fireAction.performed += _ => ChargedFire(0);
        fireAction.canceled += _ => ChargedFire(1);

        dodgeAction.performed += _ => OnDodge();

        rangeWeaponAction.started += _ => StartRangeWeapon();
        rangeWeaponAction.performed += _ => OnRangeWeapon(canRangeFire);
        rangeWeaponAction.canceled += _ => OnRangeWeapon(canRangeFire);

        chargeTimer = 0f;

        // 초기화
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true);
        waitSec = new WaitForSeconds(.01f);
        waitFix = new WaitForFixedUpdate();
    }

    private void OnEnable()
    {
        readyDodge = true;
        isDodge = false;
        isAttack = false;
        isHit = false;

        actions.Enable();

        for (int i = 0; i < chargeEffects.Length; i++)
        {
            chargeEffects[i].gameObject.SetActive(false);
        }


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
        Vector2 nextVec = inputVector * GameManager.Instance.playerSpeed * Time.fixedDeltaTime;
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
        // 피해면역인 상태면?
        if (isImmune) return;

        // Layer 6: Enemy
        if (collision.gameObject.layer == 6)
        {
            target = collision.rigidbody;
            // Player Mass 5 -> 1000 으로 변경
            // 적을 적당히 밀어낼 수 있도록? 현재 난이도 너무 어려움
            // Knockback에 사용되는 force는 Mass 의 10배가 적당 (Player 한정)
            StartCoroutine(KnockBack(10000));
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

    bool isImmune;
    // 넉백이 완료되고 나서도 일정시간 무적 부여
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

        // 피해 면역 추가
        isImmune = true;
        yield return new WaitForSeconds(.1f);
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

        //Dodge 도중 반투명
        //Color currColor = spriteRenderer.color;
        //currColor.a = .6f;
        //spriteRenderer.color = currColor;

        rigid.velocity = inputVector * dodgeSpeed;
        // 0.2 초
        yield return new WaitForSeconds(.3f);

        //Dodge 도중 반투명
        //currColor.a = 1f;
        //spriteRenderer.color = currColor;

        isDodge = false;
        rigid.velocity = Vector2.zero;
        dodgeEffects[GameManager.Instance.playerId].gameObject.SetActive(false);
    }

    void Attack(int playerId)
    {
        // 모은 기에 따른 공격 패턴과 데미지 변화 변수 추가해야함
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
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") || anim.GetCurrentAnimatorStateInfo(0).IsName("SkillMotion") || anim.GetCurrentAnimatorStateInfo(0).IsName("Dead") || isHit)
            return;

        StartCoroutine("StartCharge");
    }

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
        // 이동속도 변경
        GameManager.Instance.playerSpeed /= 2;

        // 기 모으는 Effect 추가
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
        if (status == 1)
        {
            // 차징 중 interrupt로 인해 cancel 된 경우? performed와 동일한 작업
            if (!isCharging) StopCoroutine("StartCharge");
            if (GameManager.Instance.isLive) Fire();
            // 0.2 - 0.5 초 사이의 키 입력을 시도한 경우 - 한 번의 공격 나가도록
            return;
        }

        if (!isCharging)
        {
            StopCoroutine("StartCharge");
            return;   // 공격중 차징을 시도하고 키를 뗐을 때 공격 한 번 나가는 것 방지,
        }

        if (!isHit && GameManager.Instance.isLive)
        {
            // 총 3 칸으로 구성된 기 카운트 사용
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



        //기 되돌리기

        chargeEffects[Mathf.Min(chargeCount, 2)].gameObject.SetActive(false);
        isCharging = false;
        chargeTimer = 0;
        chargeCount = 0;
        GameManager.Instance.playerSpeed *= 2;
        //스킬 방향 지시 중단
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


            if (GameManager.Instance.chargeCount == chargeCount)
            {
                if (chargeCount < 3)
                {
                    chargeEffects[chargeCount].gameObject.SetActive(false);
                }
                yield return null;
                continue;
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

            // 스킬 방향 설정 : 최소 0.5 초 이상 기모으는 시점부터 보이도록.
            // 일반 공격 시에도 방향 나오는 것 방지
            if (totalTime > GameManager.Instance.chargeTime)
            {
                StartCoroutine("SkillArrow");
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



    // 스킬 방향 화살표 부드러운 전환 위해 매 프레임당 회전 설정
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
        Rigidbody2D skillRigid;
        skillRigid = skills[GameManager.Instance.playerId].GetComponentsInChildren<Rigidbody2D>(true)[level];
        skillRigid.transform.localRotation = Quaternion.FromToRotation(Vector3.right, skillDir);

        skillRigid.gameObject.SetActive(true);
        skillRigid.velocity = skillDir * 15;
        StartCoroutine(StopSkill(skillRigid));

    }

    IEnumerator StopSkill(Rigidbody2D skillRigid)
    {
        yield return new WaitForSeconds(.6f);
        skillRigid.velocity = Vector3.zero;
        skillRigid.gameObject.SetActive(false);
        skillRigid.transform.localPosition = Vector3.zero;
        skillRigid.transform.localRotation = Quaternion.identity;
    }


    void StartRangeWeapon()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") || anim.GetCurrentAnimatorStateInfo(0).IsName("SkillMotion") || anim.GetCurrentAnimatorStateInfo(0).IsName("Dead") || isHit)
            return;
        //if (anim.GetCurrentAnimatorStateInfo(0).IsName("Dead") || isHit)
        //    return;
        if (!GameManager.Instance.isLive) return;
        // 장비하고 있으면 실행
        if (GameManager.Instance.rangeWeaponItem == -1)
            return;
        // 차징 캔슬 가능하도록 start Action으로 옮기도록. 조준만 해도 차징 취소.
        // 일단 아래 기능 임시로 취소.
        // 계속 스킬 차징을 유지하는 게 더 액션성을 강조할 수 있을 것이라 테스트 해보고 판단하게 됨.
        //if (isCharging)
        //{
        //    ChargedFire(1);
        //}

        if (rangeWeapon.readyRangeWeapon)
        {
            canRangeFire = true;
            //궤적 생성
            StartCoroutine("RangeArrow");
            //궤적 중 다른 키입력 오면 취소 (공격버튼, 마법버튼.)

        }
        else
        {
            StartCoroutine(FailMotion());
            // 준비 안 됐으면 실패모션 띄우고 리턴
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

            rangeArrow.transform.localRotation = Quaternion.FromToRotation(Vector3.right, rangeDir);
            yield return waitFix;
        }
    }

    void OnRangeWeapon(bool ready)
    {
        if (ready && GameManager.Instance.isLive)
        {
            rangeWeapon.Fire(rangeDir);
        }
        canRangeFire = false;
        StopCoroutine("RangeArrow");
        rangeArrow.transform.localRotation = Quaternion.identity;
        rangeArrow.SetActive(false);
    }
}
