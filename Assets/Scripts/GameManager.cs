using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("# Input System")]
    public InputActionAsset actions;

    [Header("# Camera")]
    public int originPPU;

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
    public int maxChargeCount; // �ִ� ���� �?
    public int chargeCount; // How many skills you can use, Right Now! Player ��ũ��Ʈ�� chargeCount�ʹ� ����. ���� ���� �� �� ����?
    public float chargeCooltime; // 1 ���� �����ϴµ� �ɸ��� �ð�
    public int maxChargibleCount; // �� ���� �󸶳� ���� ����?
    public float chargeTime; // 1 ���� �ϴµ� �ɸ��� �ð�
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
    public int storedGold;
    public List<int> storedItemsId;

    [Header("# Equipment ")]
    // -1 �̸� ������
    public int[] mainWeaponItem;
    public int[] necklaceItem;
    public int[] shoesItem;
    public int rangeWeaponItem;
    public int magicItem;


    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    public Result uiResult;

    GameObject enemyCleaner;
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

        //Ŀ�� ���
        //Cursor.lockState = CursorLockMode.Locked;

        gold = 0;
        inventoryItemsId = new int[24];
        // -1 means an empty slot!
        for (int i = 0; i < inventoryItemsId.Length; i++)
        {
            inventoryItemsId[i] = -1;
        }

        storedGold = 0;
        storedItemsId = new List<int>();

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
        GameStart();
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

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameVictory();
        }
    }

    public void StatusUpdate()
    {
        // ���߿� �������� �ʿ��� ��� ������ ����ϴ� ���̸� ���� �迭�� ����� ���� ����
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
            // ü���� �÷��ִ� ����� ������ ���ؼ� �۵��ϵ��� ���� ���� �����.
            maxHealth = playerBasicMaxHealth[playerId] + MathF.Min(playerHealthLevel, 3) + ItemManager.Instance.itemDataArr[necklaceItem[playerId]].baseAmount;
            // ü�� ���� �������� �������� ��, �ִ�ü�º��� ����ü���� �Ѿ�� ������ �����ϱ� ���� MaxHeath�� ���� ü���� ����
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

        if (workingInventory)
        {
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
        gameTime = 0;
        maxGameTime = stage.stageDataArr[stageId].gameTime;

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
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);

    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;
        yield return new WaitForSeconds(0.5f);
        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    IEnumerator GameVictoryRoutine()
    {
        enemyCleaner = GameObject.FindObjectsByType<EnemyCleaner>(FindObjectsInactive.Include, FindObjectsSortMode.None)[0].gameObject;
        isLive = false;
        enemyCleaner.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        uiResult.gameObject.SetActive(true);
        uiResult.Win();
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
                    
                    // ������ �� �ܿ� Exp�� �����ϴ� ��� �� �� �� ���� �����߸� �� ȣ���ϴ� �ڷ�ƾ�� ȣ��
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
