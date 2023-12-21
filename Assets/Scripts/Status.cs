using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    public Text damageValue;
    public Text speedValue;
    public Text dashValue;
    public Text skillValue;

    private void Start()
    {
        damageValue.text = GameManager.Instance.playerDamage.ToString("N0");
        speedValue.text = GameManager.Instance.playerSpeed.ToString("N0"); ;
        dashValue.text = $"LV. {GameManager.Instance.currDashLevel + 1}";
        skillValue.text = $"Lv. {GameManager.Instance.playerSkillLevel + 1}";
    }

    private void LateUpdate()
    {
        damageValue.text = GameManager.Instance.playerDamage.ToString("N0");
        speedValue.text = GameManager.Instance.playerSpeed.ToString("N0"); ;
        dashValue.text = $"LV. {GameManager.Instance.currDashLevel + 1}";
        skillValue.text = $"Lv. {GameManager.Instance.playerSkillLevel + 1}";
    }
}
