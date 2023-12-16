using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public ItemData[] itemDataArr;

    private void Awake()
    {
        ItemManager[] scripts = GameObject.FindObjectsByType<ItemManager>(FindObjectsSortMode.None);
        if (scripts.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    
    
}
