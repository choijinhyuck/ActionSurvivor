using System.Collections;
using System.Collections.Generic;
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
        content = GameManager.Instance.gold.ToString("N0");
        text.text = content;
    }
}
