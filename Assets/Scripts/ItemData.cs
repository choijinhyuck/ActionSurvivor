using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Melee, Range, Shoes, Necklace, Magic, Potion, Gold }
    public enum Items
    {
        BronzeSword, SilverSword, GoldenSword, BronzeHammer, SilverHammer, GoldenHammer, Kunai, Shuriken, Arrow, MiniPotion, NormalPotion, BigPotion,
        Gold5, Gold19, Gold49, FireMagic, IceMagic, LightningMagic, BronzeShoes, SilverShoes, GoldenShoes, KunaiPlus, ShurikenPlus, ArrowPlus,
        RevivalNecklace, SkillNecklace, HealthNecklace
    }


    [Header("# Main Info")]
    public ItemType itemType;
    public string itemName;
    [TextArea]
    public string itemDesc;
    [TextArea]
    public string itemEffect;
    public Sprite itemIcon;

    [Header("# Item info")]
    // 무기면 damage, 회복제면 heal.
    public float baseAmount;
    public float coolTime;
    //관통
    public int pierceCount = 0;
    //투척 스피드
    public float speed = 0;

    [Header("# Shop")]
    public int priceToBuy;

    [Header("# Check for Pool")]
    public GameObject projectile;
    public GameObject dropItem;
}

