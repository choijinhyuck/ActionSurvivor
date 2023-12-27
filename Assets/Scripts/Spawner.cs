using UnityEditor;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public StageData stage;
    public Transform RightUp;
    public Transform enemyParent;


    int wave;
    float[] timer;

    private void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    private void Start()
    {
        stage = GameManager.Instance.stage.stageDataArr[GameManager.Instance.stageId];
        wave = 0;
        timer = new float[stage.waveData[wave].mobArr.Length];
    }

    private void Update()
    {
        if (!GameManager.Instance.isLive) return;

        if (wave < stage.waveData.Length - 1 && GameManager.Instance.gameTime > stage.waveData[wave + 1].startTime)
        {
            wave++;
            timer = new float[stage.waveData[wave].mobArr.Length];
        }

        // Wave별 Enemy spawn timer array 정의
        //for (int i = 0; i < stage.waveData[wave].mobArr.Length; i++)
        //{
        //    timer[i] += Time.deltaTime;
        //    if (timer[i] > spawnData[stage.waveData[wave].mobArr[i]].spawnTime)
        //    {
        //        timer[i] = 0;

        //        for (int count = 0; count < spawnData[stage.waveData[wave].mobArr[i]].enemyCount; count++)
        //        {
        //            Spawn(stage.waveData[wave].mobArr[i]);
        //        }
        //    }
        //}
    }

    void Spawn(int spawnIndex)
    {
        // PoolManager의 0번 Prefab은 Enemy
        GameObject enemy = GameManager.Instance.pool.Get(0);
        enemy.transform.parent = enemyParent;

        //Respawn Edge 영역 설정
        float localX = RightUp.localPosition.x;
        float localY = RightUp.localPosition.y;
        bool isXaxis = Random.Range(0, localX + localY) < localX;
        float x;
        float y;
        if (isXaxis)
        {
            x = Random.Range(-localX, localX);
            if (Random.Range(0, 2) == 0)
            {
                y = localY;
            }
            else
            {
                y = -localY;
            }
        }
        else
        {
            y = Random.Range(-localY, localY);
            if (Random.Range(0, 2) == 0)
            {
                x = localX;
            }
            else
            {
                x = -localX;
            }
        }

        float dX = RightUp.localPosition.x - x;
        float dY = RightUp.localPosition.y - y;
        float spawnX = RightUp.position.x - dX;
        float spawnY = RightUp.position.y - dY;

        enemy.transform.position = new Vector3(spawnX, spawnY, 0f);
        enemy.GetComponent<Enemy>().Init(spawnData[spawnIndex]);
    }

}

[System.Serializable]
public class SpawnData
{
    [Header("# Spawn Info")]
    public string name;
    public int spriteType;
    public float spawnTime;
    public int health;
    public float speed;
    public int exp;
    public int enemyCount = 1;
    public float mass = 1;
    // 아래 두 배열은 같은 인덱스를 공유해야함.
    public int[] dropItemsId;
    public float[] dropProbability;

    [Header("# Collider Info")]
    public Vector2 collPos;
    public Vector2 collSize;

    [Header("# Shadow Info")]
    public float shadowScale;
    public Vector2 shadowOrigin;
    public Vector2 shadowFlip;

    [Header("# Look Where")]
    public bool lookLeft = true;

    [Header("# UI")]
    public Vector2 hpBarPos;
    public Vector2 hpBarSize;
}
