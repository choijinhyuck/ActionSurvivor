using UnityEngine;

public class Result : MonoBehaviour
{
    public GameObject[] titles;

    private void Awake()
    {
        Result[] scripts = GameObject.FindObjectsByType<Result>(FindObjectsSortMode.None);
        if (scripts.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    public void Lose()
    {
        titles[0].SetActive(true);
    }
    public void Win()
    {
        titles[1].SetActive(true);
    }
}
