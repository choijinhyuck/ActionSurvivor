using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("# Input System")]
    public InputActionAsset actions;

    [Header("# Game Control")]
    public StageManager stage;
    public int stageId;
    public bool isLive;
    public float gameTime;
    public float maxGameTime;

    [Header("# Player Info")]
    public int playerId;
    public float health;
    public float[] maxHealth;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp;
    public float dodgeTime;
    public int maxChargeCount;
    public int chargeCount; // How many skills you can use. Player 스크립트의 chargeCount와는 별개.
    public int maxChargibleCount; // 한 번에 얼마나 차지 가능?
    public float chargeTime;
    public float chargeCooltime;

    [Header("# Inventory")]
    public InventoryUI inventoryUI;
    public bool workingInventory;
    public int gold;
    public int maxInventory;
    public List<int> inventoryItemsId;
    public int storedGold;
    public List<int> storedItemsId;

    [Header("# Equipment ")]
    // -1 이면 무장착
    public int[] mainWeaponItem;
    public int[] rangeWeaponItem;
    public int[] magicItem;
    public int[] shoesItem;

    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    public Result uiResult;

    GameObject enemyCleaner;
    InputAction inventoryAction;
    InputAction menuAction;
    


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
        //Cursor.lockState = CursorLockMode.Locked;

        gold = 0;
        inventoryItemsId = new List<int>();
        storedGold = 0;
        storedItemsId = new List<int>();

        mainWeaponItem = new int[maxHealth.Length];
        rangeWeaponItem = new int[maxHealth.Length];
        magicItem = new int[maxHealth.Length];
        shoesItem = new int[maxHealth.Length];
        for (int i = 0; i < maxHealth.Length; i++)
        {
            mainWeaponItem[i] = -1;
            rangeWeaponItem[i] = -1;
            magicItem[i] = -1;
            shoesItem[i] = -1;
        }

        inventoryAction = actions.FindActionMap("UI").FindAction("Inventory");
        menuAction = actions.FindActionMap("UI").FindAction("Menu");

        inventoryAction.performed += _ => OnInventory();
        menuAction.performed += _ => inventoryUI.OnMenu();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        actions.Enable();
        if (inventoryUI.gameObject.activeSelf)
        {
            inventoryUI.gameObject.SetActive(false);
        }
        workingInventory = false;
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameStart();
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

    public void OnInventory()
    {
        if (workingInventory)
        {
            //AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
            AudioManager.instance.EffectBgm(false);
            workingInventory = false;
            inventoryUI.gameObject.SetActive(false);
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

        health = maxHealth[playerId];
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
        if (!isLive) return;

        exp += enemyExp;

        if (exp == nextExp[Mathf.Min(level, nextExp.Length - 1)])
        {
            level++;
            exp = 0;
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
