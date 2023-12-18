using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inven : MonoBehaviour
{
    void LateUpdate()
    {
        string itemList = "Inventory: [";

        foreach (int itemId in GameManager.Instance.inventoryItemsId)
        {
            itemList += ItemManager.Instance.itemDataArr[itemId].itemName + ", ";
        }


        GetComponent<Text>().text = itemList + "]";
    }
}
