using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public StageData[] stageDataArr;

    private void Awake()
    {
        StageManager[] scripts = GameObject.FindObjectsByType<StageManager>(FindObjectsSortMode.None);
        if (scripts.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
