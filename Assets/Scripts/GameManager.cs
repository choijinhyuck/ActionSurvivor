using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
    public int chargeCount; // How many skills you can use. Player ��ũ��Ʈ�� chargeCount�ʹ� ����.
    public int maxChargibleCount; // �� ���� �󸶳� ���� ����?
    public float chargeTime;
    public float chargeCooltime;

    [Header("# Item Info")]
    public ItemManager item;

    [Header("# Equipment ")]
    // -1 �̸� ������
    public int[] mainWeaponItem;
    public int[] rangeWeaponItem;
    public int[] magicItem;
    public int[] shoesItem;

    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    public LevelUp uiLevelUp;
    public Result uiResult;

    GameObject enemyCleaner;


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
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

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



    public void GetExp()
    {
        if (!isLive) return;

        exp++;

        if (exp == nextExp[Mathf.Min(level, nextExp.Length - 1)])
        {
            level++;
            exp = 0;
            uiLevelUp.Show();
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
