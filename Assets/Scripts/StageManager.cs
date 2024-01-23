using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    public StageData[] stageDataArr;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
