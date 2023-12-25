using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    public Text className;
    public Text damageValue;
    public Text speedValue;
    public Text damageLevel;
    public Text speedLevel;
    public Text healthLevel;
    public Text skillLevel;
    public Text dashLevel;

    private void Start()
    {
        className.text = GetClassName();
        damageValue.text = GameManager.Instance.playerDamage.ToString("N0");
        speedValue.text = GameManager.Instance.playerSpeed.ToString("N0"); ;
        damageLevel.text = string.Format("  Èû: Lv.{0} / Lv.3", GameManager.Instance.playerDamageLevel);
        speedLevel.text = string.Format("¹ÎÃ¸: Lv.{0} / Lv.3", GameManager.Instance.playerSpeedLevel);
        healthLevel.text = string.Format("°Ç°­: Lv.{0} / Lv.4", GameManager.Instance.playerHealthLevel);
        skillLevel.text = string.Format("±â¼ú: Lv.{0} / Lv.6", GameManager.Instance.playerSkillLevel);
        dashLevel.text = string.Format("´ë½Ã: Lv.{0} / Lv.4", GameManager.Instance.playerDashLevel);
    }

    private void LateUpdate()
    {
        className.text = GetClassName();
        damageValue.text = GameManager.Instance.playerDamage.ToString("N0");
        speedValue.text = GameManager.Instance.playerSpeed.ToString("N0"); ;
        damageLevel.text = string.Format("  Èû: Lv.{0} / Lv.3", GameManager.Instance.playerDamageLevel);
        speedLevel.text = string.Format("¹ÎÃ¸: Lv.{0} / Lv.3", GameManager.Instance.playerSpeedLevel);
        healthLevel.text = string.Format("°Ç°­: Lv.{0} / Lv.4", GameManager.Instance.playerHealthLevel);
        skillLevel.text = string.Format("±â¼ú: Lv.{0} / Lv.6", GameManager.Instance.playerSkillLevel);
        dashLevel.text = string.Format("´ë½Ã: Lv.{0} / Lv.4", GameManager.Instance.playerDashLevel);
    }

    string GetClassName()
    {
        switch (GameManager.Instance.playerId)
        {
            case 0:
                return "¿ö¸®¾î";
            case 1:
                return "¾ß¸¸Àü»ç";
            case 2:
                return "ÆøÅº¸Ç";
            default:
                Debug.Log("Incorrect playerId!");
                return "";
        }
    }
}
