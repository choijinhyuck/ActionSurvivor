using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    public GameObject[] prefabs;

    List<GameObject>[] pools;

    bool isVictory;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);

        pools = new List<GameObject>[prefabs.Length];

        for (int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<GameObject>();
        }

        isVictory = false;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (!isVictory && GameManager.instance.stageId != -1 && GameManager.instance.gameTime > GameManager.instance.maxGameTime && EnemyCount() == 0)
        {
            isVictory = true;
            if (SceneManager.GetActiveScene().name == "Stage_2")
            {
                GameManager.instance.Boss();
            }
            else
            {
                GameManager.instance.GameVictory();
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새로운 씬 로드 시 잔여 pool 모두 제거
        for (int i = 0; i < pools.Length; i++)
        {
            foreach (var prefab in pools[i])
            {
                Destroy(prefab);
            }
            pools[i] = new List<GameObject>();
        }

        isVictory = false;
    }

    public GameObject Get(int index)
    {
        GameObject select = null;

        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        if (!select)
        {
            select = Instantiate(prefabs[index], transform);
            pools[index].Add(select);
        }

        return select;
    }

    public int EnemyCount()
    {
        int count = 0;
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            if (transform.GetChild(0).GetChild(i).gameObject.activeSelf) count++;
        }
        return count;
    }
}
