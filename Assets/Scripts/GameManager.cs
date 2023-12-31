using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Save Info")]
    public int stage0_ClearCount;
    public int stage1_ClearCount;
    public int stage2_ClearCount;
    // 해금과 캐릭터 교체 도움말 창을 닫고 난 후 true로 전환
    // 플레이어 교체는 Camp 에서만 가능
    // 캐릭터는 순서대로 해금되고 마지막으로 해금한 Id가 할당
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

        //커서 잠금
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
                break;

            case "Loading":
                stageId = -1;
                player.transform.position = new Vector3(0, 0, 0);
                player.gameObject.SetActive(true);
                if (hud.activeSelf) hud.SetActive(false);
                AudioManager.instance.PlayBgm(false);
                break;

            case "Camp":
                stageId = -1;
                FadeIn();
                player.gameObject.SetActive(false);
                if (!stageName.activeSelf) stageName.SetActive(true);
                if (timer.activeSelf) timer.SetActive(false);
                if (killText.activeSelf) killText.SetActive(false);
                player.transform.position = new Vector3(0, -2.5f, 0);
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

        var test = (InputSystemUIInputModule)EventSystem.current.currentInputModule;
        if (test.cancel.action.WasPerformedThisFrame())
        {
        }
        //FindObjectOfType<StageSelect>() != null
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
        if (new List<string> { "Title", "Loading" }.Contains(SceneManager.GetActiveScene().name)) return;
        if (FindAnyObjectByType<StageSelect>() != null)
        {
            if (FindAnyObjectByType<StageSelect>().stageSelectPanel.activeSelf) return;
        }
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
            maxGameTime = StageManager.instance.stageDataArr[stageId].gameTime;
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
        switch (sceneName)
        {
            case "Stage_0":
                stage0_ClearCount++;
                break;
            case "Stage_1":
                stage1_ClearCount++;
                break;
            case "Stage_2":
                stage2_ClearCount++;
                break;
        }
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
