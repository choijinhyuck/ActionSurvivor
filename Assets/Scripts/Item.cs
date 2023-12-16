using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;

    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;

    private void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];
        textName.text = data.itemName;
    }

    private void OnEnable()
    {
        textLevel.text = "Lv." + (level + 1);

        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                //textDesc.text = string.Format(data.itemDesc, data.damage[level] * 100, data.count[level]);
                break;
            //case ItemData.ItemType.Gloves:
            case ItemData.ItemType.Shoes:
                //textDesc.text = string.Format(data.itemDesc, data.damage[level] * 100);
                break;
            default:
                textDesc.text = string.Format(data.itemDesc);
                break;
        }
        
    }

    public void OnClick()
    {
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if (level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                }
                else
                {
                    float nextDamage = data.baseAmount;
                    int nextCount = 0;

                    //nextDamage += data.baseAmount * data.damage[level];
                    //nextCount += data.count[level];

                    weapon.LevelUp(nextDamage, nextCount);
                }
                level++;
                break;
            //case ItemData.ItemType.Gloves:
            case ItemData.ItemType.Shoes:
                if (level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    //float nextRate = data.damage[level];
                    //gear.LevelUp(/*nextRate*/);

                }
                level++;
                break;
            case ItemData.ItemType.Potion:
                //GameManager.Instance.health = GameManager.Instance.maxHealth;
                break;
        }

        //if (level == data.damage.Length)
        //{
        //    GetComponent<Button>().interactable = false;
        //}
    }
}
