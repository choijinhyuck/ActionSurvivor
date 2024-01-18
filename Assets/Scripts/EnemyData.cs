using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Object/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("# Spawn Info")]
    public string enemyName;
    public RuntimeAnimatorController anim;
    public int health;
    public float speed;
    public int exp;
    public float mass = 1;

    [Header("# Drop Items (% probability)")]
    public DropItems[] dropItems;

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

[System.Serializable]
public struct DropItems
{
    public ItemData.Items itemType;
    public float probability;
}