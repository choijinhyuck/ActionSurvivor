using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static void Save()
    {
        Prefs("lastPlayerId", GameManager.instance.playerId);
        Prefs("maxInventory", GameManager.instance.maxInventory);
        Prefs("gold", GameManager.instance.gold);
        
        for (int i = 0; i < GameManager.instance.inventoryItemsId.Length; i++)
        {
            string key = "inventory" + i.ToString();
            Prefs(key, GameManager.instance.inventoryItemsId[i]);
        }
        for (int i = 0; i < GameManager.instance.storedItemsId.Length; i++)
        {
            string key = "storage" + i.ToString();
            Prefs(key, GameManager.instance.storedItemsId[i]);
        }

        for (int i = 0; i < GameManager.instance.mainWeaponItem.Length; i++)
        {
            string key_1 = "mainWeaponItem" + i.ToString();
            string key_2 = "necklaceItem" + i.ToString();
            string key_3 = "shoesItem" + i.ToString();
            Prefs(key_1, GameManager.instance.mainWeaponItem[i]);
            Prefs(key_2, GameManager.instance.necklaceItem[i]);
            Prefs(key_3, GameManager.instance.shoesItem[i]);
        }

        Prefs("rangeWeaponItem", GameManager.instance.rangeWeaponItem);
        Prefs("magicItem", GameManager.instance.magicItem);

        Prefs("stage0_ClearCount", GameManager.instance.stage0_ClearCount);
        Prefs("stage1_ClearCount", GameManager.instance.stage1_ClearCount);
        Prefs("stage2_ClearCount", GameManager.instance.stage2_ClearCount);

        Prefs("newCharacterUnlock", GameManager.instance.newCharacterUnlock);

        PlayerPrefs.Save();
    }

    public static void ResetSave()
    {
        if (!PlayerPrefs.HasKey("maxInventory"))
        {
            Debug.Log("Try to delete the SaveData doesn't exist");
            return;
        }

        List<string> keys = new List<string>()
        {
            "lastplayerId",
            "maxInventory",
            "gold",
            "rangeWeaponItem",
            "magicitem"
        };

        for (int i = 0; i < GameManager.instance.inventoryItemsId.Length; i++)
        {
            string key = "inventory" + i.ToString();
            keys.Add(key);
        }
        for (int i = 0; i < GameManager.instance.storedItemsId.Length; i++)
        {
            string key = "storage" + i.ToString();
            keys.Add(key);
        }

        for (int i = 0; i < GameManager.instance.mainWeaponItem.Length; i++)
        {
            string key_1 = "mainWeaponItem" + i.ToString();
            string key_2 = "necklaceItem" + i.ToString();
            string key_3 = "shoesItem" + i.ToString();
            keys.Add(key_1);
            keys.Add(key_2);
            keys.Add(key_3);
        }

        keys.Add("stage0_ClearCount");
        keys.Add("stage1_ClearCount");
        keys.Add("stage2_ClearCount");
        keys.Add("newCharacterUnlock");


        foreach (string key in keys)
        {
            PlayerPrefs.DeleteKey(key);
        }

        PlayerPrefs.Save();
    }

    public static void Load()
    {
        GameManager.instance.playerId = GetInt("lastPlayerId");
        GameManager.instance.maxInventory = GetInt("maxInventory");
        GameManager.instance.gold = GetInt("gold");

        for (int i = 0; i < GameManager.instance.inventoryItemsId.Length; i++)
        {
            string key = "inventory" + i.ToString();
            GameManager.instance.inventoryItemsId[i] = GetInt(key);
        }
        for (int i = 0; i < GameManager.instance.storedItemsId.Length; i++)
        {
            string key = "storage" + i.ToString();
            GameManager.instance.storedItemsId[i] = GetInt(key);
        }

        for (int i = 0; i < GameManager.instance.mainWeaponItem.Length; i++)
        {
            string key_1 = "mainWeaponItem" + i.ToString();
            string key_2 = "necklaceItem" + i.ToString();
            string key_3 = "shoesItem" + i.ToString();
            GameManager.instance.mainWeaponItem[i] = GetInt(key_1);
            GameManager.instance.necklaceItem[i] = GetInt(key_2);
            GameManager.instance.shoesItem[i] = GetInt(key_3);
        }

        GameManager.instance.rangeWeaponItem = GetInt("rangeWeaponItem");
        GameManager.instance.magicItem = GetInt("magicItem");

        GameManager.instance.stage0_ClearCount = GetInt("stage0_ClearCount");
        GameManager.instance.stage1_ClearCount = GetInt("stage1_ClearCount");
        GameManager.instance.stage2_ClearCount = GetInt("stage2_ClearCount");

        GameManager.instance.newCharacterUnlock = GetInt("newCharacterUnlock");

        PlayerPrefs.Save();
    }

    static void Prefs(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }
    static void Prefs(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }
    
    static int GetInt(string key)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            Debug.Log($"Couldn't find [{key}]");
            return -100;
        }
        return PlayerPrefs.GetInt(key);
    }

}
