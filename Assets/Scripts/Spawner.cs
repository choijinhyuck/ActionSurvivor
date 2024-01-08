using UnityEditor;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public StageData stage;
    public Transform RightUp;
    public EnemyData[] enemyData;

    int currentStageId;
    int wave;
    float[] timer;

    private void Start()
    {
        if (GameManager.instance.stageId == -1)
        {
            stage = null;
            wave = -1;
            timer = new float[] { };
        }
        else
        {
            stage = GameManager.instance.stage.stageDataArr[GameManager.instance.stageId];
            wave = 0;
            timer = new float[stage.waveData[0].enemyInfos.Length];
        }
        currentStageId = GameManager.instance.stageId;
    }

    private void Update()
    {
        if (GameManager.instance.stageId == -1) return;
        if (!GameManager.instance.isLive) return;
        if (GameManager.instance.gameTime > GameManager.instance.maxGameTime) return;
        if (currentStageId != GameManager.instance.stageId)
        {
            currentStageId = GameManager.instance.stageId;
            wave = 0;
            timer = new float[stage.waveData[0].enemyInfos.Length];
        }

        if (wave < stage.waveData.Length - 1 && GameManager.instance.gameTime > stage.waveData[wave + 1].startTime)
        {
            wave++;
            timer = new float[stage.waveData[wave].enemyInfos.Length];
        }

        for (int i = 0; i < stage.waveData[wave].enemyInfos.Length; i++)
        {
            timer[i] += Time.deltaTime;
            if (timer[i] > stage.waveData[wave].enemyInfos[i].spawnTime)
            {
                timer[i] = 0;

                for (int count = 0; count < stage.waveData[wave].enemyInfos[i].enemyCount; count++)
                {
                    Spawn((int)stage.waveData[wave].enemyInfos[i].enemyType);
                }
            }
        }
    }

    void Spawn(int spawnIndex)
    {
        // PoolManager의 0번 Prefab은 Enemy
        GameObject enemy = PoolManager.instance.Get(0);
        enemy.transform.parent = PoolManager.instance.transform.GetChild(0);

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
        enemy.GetComponent<Enemy>().Init(enemyData[spawnIndex]);
    }

}