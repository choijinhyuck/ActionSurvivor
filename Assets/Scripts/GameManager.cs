using Cinemachine;
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
    public static GameManager instance;

    [Header("# Save Info")]
    public int stage0_ClearCount;
    public int stage1_ClearCount;
    public int stage2_ClearCount;
    // �رݰ� ĳ���� ��ü ���� â�� �ݰ� �� �� true�� ��ȯ
    // �÷��̾� ��ü�� Camp ������ ����
    // ĳ���ʹ� ������� �رݵǰ� ���������� �ر��� Id�� �Ҵ�
    public int newCharacterUnlock;
    Coroutine saveCoroutine;

    [Header("Loading Info")]
    public string sceneName;

    [Header("# Input System")]
    public InputActionAsset actions;

    [Header("# Camera")]
    public int originPPU;
    public GameObject fadeInPrefab;
    public GameObject fadeOutPrefab;

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
    public int[] storedItemsId;

    [Header("# Equipment ")]
    // -1 �̸� ������
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
    public GameObject stageName;

    InputAction inventoryAction;
    InputAction menuAction;
    InputAction cancelAction;
    InputAction equipAction;
    InputAction destroyAction;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
        Application.targetFrameRate = 60;

        //Ŀ�� ���
        //Cursor.lockState = CursorLockMode.Locked;

        InfoInit();

        saveCoroutine = null;

        inventoryAction = actions.FindActionMap("UI").FindAction("Inventory");
        menuAction = actions.FindActionMap("UI").FindAction("Menu");
        cancelAction = actions.FindActionMap("UI").FindAction("Cancel");
        equipAction = actions.FindActionMap("UI").FindAction("Equip");
        destroyAction = actions.FindActionMap("UI").FindAction("Destroy");

        GameManagerActionAdd();
    }

    public void InfoInit()
    {
        playerId = 0;

        gold = 0;
        inventoryItemsId = new int[24];
        // -1 means an empty slot!
        for (int i = 0; i < inventoryItemsId.Length; i++)
        {
            inventoryItemsId[i] = -1;
        }

        storedItemsId = new int[24];
        for (int i = 0; i < storedItemsId.Length; i++)
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

        stage0_ClearCount = 0;
        stage1_ClearCount = 0;
        stage2_ClearCount = 0;
        newCharacterUnlock = 0;
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

    private void OnDestroy()
    {
        GameManagerActionRemove();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SaveCoroutineManager(scene.name);
        switch (scene.name)
        {
            case "Title":
                stageId = -1;
                FadeIn();
                player.transform.position = new Vector3(0, 0, 0);
                player.gameObject.SetActive(true);
                if (hud.activeSelf) hud.SetActive(false);
                BGMInit(AudioManager.Bgm.Title, .3f);
                ZoomCamera();
                actions.Disable();
                break;

            case "Loading":
                stageId = -1;
                player.transform.position = new Vector3(0, 0, 0);
                player.gameObject.SetActive(true);
                if (hud.activeSelf) hud.SetActive(false);
                AudioManager.instance.PlayBgm(false);
                actions.Disable();
                break;

            case "Camp":
                stageId = -1;
                FadeIn();
                player.gameObject.SetActive(false);
                if (!stageName.activeSelf) stageName.SetActive(true);
                if (timer.activeSelf) timer.SetActive(false);
                if (killText.activeSelf) killText.SetActive(false);
                player.transform.position = new Vector3(0, -3, 0);
                BGMInit(AudioManager.Bgm.Camp, .3f);
                ZoomCamera();
                GameStart();
                break;

            case "Stage_0":
                stageId = 0;
                FadeIn();
                player.transform.position = new Vector3(0, 0, 0);
                player.gameObject.SetActive(false);
                if (!stageName.activeSelf) stageName.SetActive(true);
                if (!timer.activeSelf) timer.SetActive(true);
                if (!killText.activeSelf) killText.SetActive(true);
                BGMInit(AudioManager.Bgm.Stage0, .5f);
                if (!PlayerPrefs.HasKey("maxInventory"))
                {
                    AudioManager.instance.PlayBgm(false);
                    FindAnyObjectByType<TutorialUI>(FindObjectsInactive.Include).gameObject.SetActive(true);
                }
                ZoomCamera();
                GameStart();
                break;

            case "Stage_1":
                stageId = 1;
                FadeIn();
                player.transform.position = new Vector3(0, 0, 0);
                player.gameObject.SetActive(false);
                if (stageName.activeSelf) stageName.SetActive(true);
                if (!timer.activeSelf) timer.SetActive(true);
                if (!killText.activeSelf) killText.SetActive(true);
                BGMInit(AudioManager.Bgm.Stage1, .5f);
                ZoomCamera();
                GameStart();
                break;
        }
    }

    IEnumerator SaveCoroutine()
    {
        while (true)
        {
            SaveManager.Save();
            Debug.Log("Save Completed");
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    void SaveCoroutineManager(string name)
    {
        bool isCamp = name == "Camp";
        if (!isCamp)
        {
            if (saveCoroutine != null)
            {
                StopCoroutine(saveCoroutine);
                saveCoroutine = null;
            }
        }
        else
        {
            if (saveCoroutine == null)
            {
                saveCoroutine = StartCoroutine(SaveCoroutine());
            }
        }
    }

    void BGMInit(AudioManager.Bgm BgmType, float volume, bool isLoop = true)
    {
        AudioManager.instance.EffectBgm(false);
        AudioManager.instance.ChangeBGM(BgmType, volume, isLoop);
        AudioManager.instance.PlayBgm(true);
    }

    void FadeIn()
    {
        GameObject fadeIn = Instantiate<GameObject>(fadeInPrefab);
        fadeIn.SetActive(true);
    }

    public void FadeOut()
    {
        GameObject fadeOut = Instantiate<GameObject>(fadeOutPrefab);
        fadeOut.SetActive(true);
    }

    public void ZoomCamera(int targetPPU)
    {
        Camera.main.transform.GetComponent<PixelPerfectCamera>().assetsPPU = targetPPU;
    }

    public void ZoomCamera()
    {
        Camera.main.transform.GetComponent<PixelPerfectCamera>().assetsPPU = originPPU;
    }

    public void CameraDamping(float value = 1f)
    {
        var transposer = VirtualCamera.instance.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();
        transposer.m_XDamping = value;
        transposer.m_YDamping = value;
    }

    private void Update()
    {
        if (!isLive)
            return;

        gameTime += Time.deltaTime;
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
        if (FindAnyObjectByType<TutorialUI>() != null) return;
        if (health < .1f) return;
        if (BaseUI.Instance.victory.gameObject.activeSelf) return;

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

        level = 0;
        exp = 0;

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

        BGMInit(AudioManager.Bgm.Death, .5f, false);
    }

    public void GameVictory()
    {
        Stop();

        BGMInit(AudioManager.Bgm.Victory, .3f, false);
        BaseUI.Instance.Victory();
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

    void OnMenu()
    {
        if (InventoryUI.instance == null) return;
        InventoryUI.instance.OnMenu();
    }

    void OnCancel()
    {
        if (InventoryUI.instance == null) return;
        InventoryUI.instance.OnCancel();
    }

    void EquipUnequip()
    {
        if (InventoryUI.instance == null) return;
        InventoryUI.instance.EquipUnequip();
    }

    void DestroyItem()
    {
        if (InventoryUI.instance == null) return;
        InventoryUI.instance.DestroyItem();
    }

    public void GameManagerActionRemove()
    {
        inventoryAction.performed -= InventoryHandler;
        menuAction.performed -= MenuHandler;
        cancelAction.performed -= CancelHandler;
        equipAction.performed -= EquipHandler;
        destroyAction.performed -= DestroyHandler;
    }

    public void GameManagerActionAdd()
    {
        inventoryAction.performed += InventoryHandler;
        menuAction.performed += MenuHandler;
        cancelAction.performed += CancelHandler;
        equipAction.performed += EquipHandler;
        destroyAction.performed += DestroyHandler;
    }

    void InventoryHandler(InputAction.CallbackContext context)
    {
        OnInventory();
    }
    void MenuHandler(InputAction.CallbackContext context)
    {
        OnMenu();
    }
    void CancelHandler(InputAction.CallbackContext context)
    {
        OnCancel();
    }
    void EquipHandler(InputAction.CallbackContext context)
    {
        EquipUnequip();
    }
    void DestroyHandler(InputAction.CallbackContext context)
    {
        DestroyItem();
    }
}
