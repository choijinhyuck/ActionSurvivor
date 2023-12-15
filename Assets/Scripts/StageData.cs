using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Scriptable Object/StageData")]
public class StageData : ScriptableObject
{
    [Header("# Stage Info")]
    public int stageNumber;
    public string stageName;
    public int gameTime;
    public int flyEnemy;

    public WaveData[] waveData;

    public int boss;    
}

[System.Serializable]
public class WaveData
{
    [Header("# Wave")]
    public int startTime;
    public int[] mobArr;
}