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

    [Header("# Item info")]
    // ����� damage, ȸ������ heal.
    public float baseAmount;
    public float coolTime;
    //����
    public int pierceCount = 0;
    //��ô ���ǵ�
    public float speed = 0;


    [Header("# Check for Pool")]
    public GameObject projectile;
}
