using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    public ItemData[] itemDataArr;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }



}
