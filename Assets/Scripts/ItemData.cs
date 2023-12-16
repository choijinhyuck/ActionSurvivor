using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
public enum ItemType { Melee, Range, Shoes, Necklace, Scroll, Potion}

    [Header("# Main Info")]
    public ItemType itemType;
    public string itemName;
    [TextArea]
    public string itemDesc;
    [TextArea]
    public string itemEffet;
    public Sprite itemIcon;

    [Header("# Level Data")]
    // 무기면 damage, 회복제면 heal.
    public float baseAmount;

    //관통
    public bool canPierce = false;
    public int pierceCount = 0;


    [Header("# Check for Pool")]
    public GameObject projectile;
}
