using UnityEngine;
using UnityEngine.UI;

public class Gold : MonoBehaviour
{
    Text text;
    string content;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void LateUpdate()
    {
        content = GameManager.instance.gold.ToString("N0");
        text.text = content;
    }
}
