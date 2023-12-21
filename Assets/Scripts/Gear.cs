using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate;

    //public void Init(ItemData data)
    //{
    //    //name = "Gear " + data.itemId;
    //    transform.parent = GameManager.Instance.player.transform;
    //    transform.localPosition = Vector3.zero;

    //    type = data.itemType;
    //    //rate = data.damage[0];
    //    ApplyGear();
    //}

    //public void LevelUp(float rate)
    //{
    //    this.rate = rate;
    //    ApplyGear();
    //}

    //void ApplyGear()
    //{
    //    switch (type)
    //    {
    //        ////case ItemData.ItemType.Gloves:
    //        //    RateUp();
    //        //    break;
    //        case ItemData.ItemType.Shoes:
    //            SpeedUp();
    //            break;
    //    }
    //}

    void RateUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        foreach (Weapon weapon in weapons)
        {
            switch (weapon.id)
            {
                case 0:
                    float speed = 150 * Character.WeaponSpeed;
                    weapon.speed = speed + (speed * rate);
                    break;
                default:
                    speed = 0.5f * Character.WeaponRate;
                    weapon.speed = speed * (1f - rate);
                    break;
            }
        }
    }

    //void SpeedUp()
    //{
    //    float speed = 3 * Character.Speed;
    //    GameManager.Instance.player.speed = speed + speed * rate;
    //}
}
