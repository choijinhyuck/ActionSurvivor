using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Loading Info")]
    public string sceneName;

    [Header("# Input System")]
    public InputActionAsset actions;

    [Header("# Camera")]
    public int originPPU;
    public GameObject fadeInPrefab;

    [Header("# Game Control")]
    public StageManager stage;
    public int stageId;
    public bool isLive;
    public float gameTime;
    public float maxGameTime;

    [Header("# Player Info")]
    public int playerId;
    public float health;
    public float maxHealth;
    
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp;
    public float dodgeTime;
    public float dodgeSpeed;
    public int maxChargeCount; // 최대 번개 몇개?
    public int chargeCount; // How many skills you can use, Right Now! Player 스크립트의 chargeCount와는 별개. 현재 번개 몇 개 충전?
    public float chargeCooltime; // 1 번개 충전하는데 걸리는 시간
    public int maxChargibleCount; // 한 번에 얼마나 차지 가능?
    public float chargeTime; // 1 차지 하는데 걸리는 시간
    public float playerDamage;
    public float playerSpeed;
    public float playerImmuneTime;
    public int playerDashLevel;
    public int playerSkillLevel;
    public int playerSpeedLevel;
    public int playerHealthLevel;
    public int playerDamageLevel;

    [Header("# Basic Player Info")]
    public float[] playerBasicMaxHealth;
    public float[] playerBasicDamage;
    public float[] playerBasicSpeed;

    [Header("# Inventory")]
    public InventoryUI inventoryUI;
    public bool workingInventory;
    public int gold;
    public int maxInventory;
    public int[] inventoryItemsId;
    public int[] storedItemsId;

    [Header("# Equipment ")]
    // -1 이면 무장착
    public int[] mainWeaponItem;
    public int[] necklaceItem;
    public int[] shoesItem;
    public int rangeWeaponItem;
    public int magicItem;


    [Header("# Game Object")]
    public Player player;
    public GameObject hud;
    public GameObject timer;
    public GameObject killText;

    InputAction inventoryAction;
    InputAction menuAction;
    InputAction cancelAction;
    InputAction equipAction;
    InputAction destroyAction;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
        Application.targetFrameRate = 60;

        //커서 잠금
        //Cursor.lockState = CursorLockMode.Locked;

        gold = 0;
        inventoryItemsId = new int[24];
        // -1 means an empty slot!
        for (int i = 0; i < inventoryItemsId.Length; i++)
        {
            inventoryItemsId[i] = -1;
        }

        storedItemsId = new int[24];
        for (int i = 0; i <  storedItemsId.Length; i++)
        {
            storedItemsId[i] = -1;
        }

        mainWeaponItem = new int[playerBasicDamage.Length];
        necklaceItem = new int[playerBasicDamage.Length];
        shoesItem = new int[playerBasicDamage.Length];
        rangeWeaponItem = -1;
        magicItem = -1;

        for (int i = 0; i < playerBasicDamage.Length; i++)
        {
            mainWeaponItem[i] = -1;
            necklaceItem[i] = -1;
            shoesItem[i] = -1;
        }

        inventoryAction = actions.FindActionMap("UI").FindAction("Inventory");
        menuAction = actions.FindActionMap("UI").FindAction("Menu");
        cancelAction = actions.FindActionMap("UI").FindAction("Cancel");
        equipAction = actions.FindActionMap("UI").FindAction("Equip");
        destroyAction = actions.FindActionMap("UI").FindAction("Destroy");

        inventoryAction.performed += _ => OnInventory();
        menuAction.performed += _ => inventoryUI.OnMenu();
        cancelAction.performed += _ => inventoryUI.OnCancel();
        equipAction.performed += _ => inventoryUI.EquipUnequip();
        destroyAction.performed += _ => inventoryUI.DestroyItem();

    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        actions.Enable();
        if (inventoryUI.gameObject.activeSelf)
        {
            inventoryUI.gameObject.SetActive(false);
            inventoryUI.destroyDesc.transform.parent.gameObject.SetActive(false);
        }
        workingInventory = false;
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Camp":
                if (timer.activeSelf) timer.SetActive(false);
                if (killText.activeSelf) killText.SetActive(false);
                player.transform.position = new Vector3(0, -3, 0);
                AudioManager.instance.changeBGM(AudioManager.Bgm.Camp, 0.3f);
                GameStart();
                break;

            case "Title":
                if (hud.activeSelf) hud.SetActive(false);
                player.gameObject.SetActive(true);
                AudioManager.instance.changeBGM(AudioManager.Bgm.Title, 0.3f);
                AudioManager.instance.PlayBgm(true);
                actions.Disable();
                break;

            case "Loading":
                if (hud.activeSelf) hud.SetActive(false);
                AudioManager.instance.PlayBgm(false);
                actions.Disable();
                break;

            case "Stage_0":
                GameObject.FindAnyObjectByType<TutorialUI>(FindObjectsInactive.Include).gameObject.SetActive(true);
                FadeIn();
                stageId = 0;
                if (!timer.activeSelf) timer.SetActive(true);
                if (!killText.activeSelf) killText.SetActive(true);
                AudioManager.instance.changeBGM(AudioManager.Bgm.Stage0, 0.5f);
                GameStart();
                break;

            case "Stage_1":
                FadeIn();
                stageId = 1;
                if (!timer.activeSelf) timer.SetActive(true);
                if (!killText.activeSelf) killText.SetActive(true);
                AudioManager.instance.changeBGM(AudioManager.Bgm.Stage1, 0.5f);
                GameStart();
                break;

            default:
                if (!timer.activeSelf) timer.SetActive(true);
                if (!killText.activeSelf) killText.SetActive(true);
                AudioManager.instance.changeBGM(AudioManager.Bgm.Stage0, 0.5f);
                GameStart();
                break;
        }
    }

    void FadeIn()
    {
        GameObject fadeIn = Instantiate<GameObject>(fadeInPrefab);
        fadeIn.SetActive(true);
    }

    public void ZoomCamera(int targetPPU)
    {
        Camera.main.transform.GetComponent<PixelPerfectCamera>().assetsPPU = targetPPU;
    }

    public void ZoomCamera()
    {
        Camera.main.transform.GetComponent<PixelPerfectCamera>().assetsPPU = originPPU;
    }


    private void Update()
    {
        if (!isLive)
            return;

        gameTime += Time.deltaTime;

        //if (gameTime > maxGameTime)
        //{
        //    gameTime = maxGameTime;
        //    GameVictory();
        //}
    }

    public void StatusUpdate()
    {
        // 나중에 곱연산이 필요한 경우 레벨에 비례하는 길이를 갖는 배열을 만들어 배율 적용
        if (mainWeaponItem[playerId] == -1)
        {
            playerDamage = playerBasicDamage[playerId] + playerDamageLevel;
        }
        else
        {
            playerDamage = playerBasicDamage[playerId] + playerDamageLevel + ItemManager.Instance.itemDataArr[mainWeaponItem[playerId]].baseAmount;
        }
        if (necklaceItem[playerId] == -1)
        {
            maxHealth = playerBasicMaxHealth[playerId] + MathF.Min(playerHealthLevel, 3);
        }
        else
        {
            // 체력을 올려주는 목걸이 종류에 한해서 작동하도록 로직 추후 세우기.
            maxHealth = playerBasicMaxHealth[playerId] + MathF.Min(playerHealthLevel, 3) + ItemManager.Instance.itemDataArr[necklaceItem[playerId]].baseAmount;
            // 체력 증가 아이템을 해제했을 때, 최대체력보다 현재체력이 넘어가는 현상을 방지하기 위해 MaxHeath로 현재 체력을 제한
            health = Mathf.Clamp(health, 0, maxHealth);
        }
        
        if (shoesItem[playerId] == -1)
        {
            playerSpeed = playerBasicSpeed[playerId] + playerSpeedLevel;
        }
        else
        {
            playerSpeed = (playerBasicSpeed[playerId] + playerSpeedLevel) * (1 + ItemManager.Instance.itemDataArr[shoesItem[playerId]].baseAmount / 100);
        }
        
    }

    public void OnInventory()
    {
        if (LevelUp.instance.isLevelUp) return;
        if (GameObject.FindAnyObjectByType<TutorialUI>() is not null) return;
        if (health < .1f) return;

        if (workingInventory)
        {
            if (!inventoryUI.gameObject.activeSelf) return;
            //AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
            AudioManager.instance.EffectBgm(false);
            workingInventory = false;
            inventoryUI.gameObject.SetActive(false);
            inventoryUI.destroyDesc.transform.parent.gameObject.SetActive(false);
            Resume();
        }
        else
        {
            //AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
            AudioManager.instance.EffectBgm(true);
            workingInventory = true;
            inventoryUI.gameObject.SetActive(true);
            Stop();

        }
    }


    public void GameStart()
    {
        if (!hud.activeSelf) hud.SetActive(true);

        actions.Enable();
        if (inventoryUI.gameObject.activeSelf)
        {
            inventoryUI.gameObject.SetActive(false);
            inventoryUI.destroyDesc.transform.parent.gameObject.SetActive(false);
        }
        workingInventory = false;

        gameTime = 0;
        if (stageId == -1)
        {
            maxGameTime = 0f;
        }
        else
        {
            maxGameTime = stage.stageDataArr[stageId].gameTime;
        }

        playerImmuneTime = .15f;

        chargeCooltime = 6f;
        chargeTime = 1f;
        dodgeTime = 2f;
        dodgeSpeed = 7f;

        maxChargibleCount = 1;
        maxChargeCount = 2;

        playerDamageLevel = 0;
        playerSpeedLevel = 0;
        playerHealthLevel = 0;
        playerSkillLevel = 0;
        playerDashLevel = 0;

        StatusUpdate();

        health = maxHealth;
        chargeCount = maxChargeCount;

        player.gameObject.SetActive(true);
        Resume();

        AudioManager.instance.PlayBgm(true);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;
        yield return new WaitForSeconds(0.5f);
        Stop();
        BaseUI.Instance.Death();

        AudioManager.instance.changeBGM(AudioManager.Bgm.Death, .5f);
        AudioManager.instance.PlayBgm(true);
    }

    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        yield return new WaitForSeconds(0.5f);
        Stop();

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
    }
    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }

    public void GameQuit()
    {
        Application.Quit();
    }



    public void GetExp(int enemyExp)
    {
        StartCoroutine(ExpCoroutine(enemyExp));
    }

    IEnumerator ExpCoroutine(int enemyExp)
    {
        while (true)
        {
            if (LevelUp.instance.isLevelUp)
            {
                yield return null;
                continue;
            }
            else if (level == 21)
            {
                exp = 0;
                yield break;
            }
            else
            {
                exp += enemyExp;

                if (exp >= nextExp[Mathf.Min(level, nextExp.Length - 1)])
                {
                    int restExp = nextExp[Mathf.Min(level, nextExp.Length - 1)] - exp;
                    level++;
                    exp = 0;
                    Stop();
                    LevelUp.instance.Do();
                    
                    // 레벨업 후 잔여 Exp가 존재하는 경우 한 번 더 적을 쓰러뜨린 후 호출하는 코루틴을 호출
                    if (restExp != 0)
                    {
                        StartCoroutine(ExpCoroutine(restExp));
                    }
                }
                break;
            }
        }
        
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }
}
