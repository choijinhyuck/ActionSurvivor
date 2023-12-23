using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    public Text className;
    public Text damageValue;
    public Text speedValue;
    public Text dashValue;
    public Text skillValue;

    private void Start()
    {
        className.text = GetClassName();
        damageValue.text = GameManager.Instance.playerDamage.ToString("N0");
        speedValue.text = GameManager.Instance.playerSpeed.ToString("N0"); ;
        dashValue.text = $"LV. {GameManager.Instance.currDashLevel + 1}";
        skillValue.text = $"Lv. {GameManager.Instance.playerSkillLevel + 1}";
    }

    private void LateUpdate()
    {
        className.text = GetClassName();
        damageValue.text = GameManager.Instance.playerDamage.ToString("N0");
        speedValue.text = GameManager.Instance.playerSpeed.ToString("N0"); ;
        dashValue.text = $"LV. {GameManager.Instance.currDashLevel + 1}";
        skillValue.text = $"Lv. {GameManager.Instance.playerSkillLevel + 1}";
    }

    string GetClassName()
    {
        switch (GameManager.Instance.playerId)
        {
            case 0:
                return "况府绢";
            case 1:
                return "具父傈荤";
            case 2:
                return "气藕盖";
            default:
                Debug.Log("Incorrect playerId!");
                return "";
        }
    }
}
