using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Scriptable Object/StageData")]
public class StageData : ScriptableObject
{
    public enum EnemyType { Snail, Bat, Chicken, Rock, Mushroom, Rhino, BlueBird }

    [Header("# Stage Info")]
    public int stageNumber;
    public string stageName;
    public int gameTime;

    public WaveData[] waveData;

    //public int boss;
}

[Serializable]
public class WaveData
{
    [Header("# Wave")]
    public int startTime;
    public EnemyInfo[] enemyInfos;
}

[Serializable]
public struct EnemyInfo
{
    public StageData.EnemyType enemyType;
    public int enemyCount;
    public float spawnTime;
}