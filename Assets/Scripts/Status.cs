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
        damageValue.text = GameManager.instance.playerDamage.ToString("N0");
        speedValue.text = GameManager.instance.playerSpeed.ToString("N0"); ;
        damageLevel.text = string.Format("  ��: Lv.{0} / Lv.3", GameManager.instance.playerDamageLevel);
        speedLevel.text = string.Format("��ø: Lv.{0} / Lv.3", GameManager.instance.playerSpeedLevel);
        healthLevel.text = string.Format("�ǰ�: Lv.{0} / Lv.4", GameManager.instance.playerHealthLevel);
        skillLevel.text = string.Format("���: Lv.{0} / Lv.6", GameManager.instance.playerSkillLevel);
        dashLevel.text = string.Format("���: Lv.{0} / Lv.4", GameManager.instance.playerDashLevel);
    }

    private void LateUpdate()
    {
        className.text = GetClassName();
        damageValue.text = GameManager.instance.playerDamage.ToString("N0");
        speedValue.text = GameManager.instance.playerSpeed.ToString("N0");

        switch (GameManager.instance.playerDamageLevel)
        {
            case 3:
                damageLevel.text = string.Format("  ��:   <color=red>Lv.Max</color>", GameManager.instance.playerDamageLevel);
                break;
            case 0:
                damageLevel.text = "  ��: Lv.- / Lv.3";
                break;
            default:
                damageLevel.text = string.Format("  ��: Lv.{0} / Lv.3", GameManager.instance.playerDamageLevel);
                break;
        }

        switch (GameManager.instance.playerSpeedLevel)
        {
            case 3:
                speedLevel.text = string.Format("��ø:   <color=red>Lv.Max</color>", GameManager.instance.playerSpeedLevel);
                break;
            case 0:
                speedLevel.text = "��ø: Lv.- / Lv.3";
                break;
            default:
                speedLevel.text = string.Format("��ø: Lv.{0} / Lv.3", GameManager.instance.playerSpeedLevel);
                break;
        }

        switch (GameManager.instance.playerHealthLevel)
        {
            case 4:
                healthLevel.text = string.Format("�ǰ�:   <color=red>Lv.Max</color>", GameManager.instance.playerHealthLevel);
                break;
            case 0:
                healthLevel.text = "�ǰ�: Lv.- / Lv.4";
                break;
            default:
                healthLevel.text = string.Format("�ǰ�: Lv.{0} / Lv.4", GameManager.instance.playerHealthLevel);
                break;
        }

        switch (GameManager.instance.playerSkillLevel)
        {
            case 6:
                skillLevel.text = string.Format("���:   <color=red>Lv.Max</color>", GameManager.instance.playerSkillLevel);
                break;
            case 0:
                skillLevel.text = "���: Lv.- / Lv.6";
                break;
            default:
                skillLevel.text = string.Format("���: Lv.{0} / Lv.6", GameManager.instance.playerSkillLevel);
                break;
        }
        
        switch(GameManager.instance.playerDashLevel)
        {
            case 4:
                dashLevel.text = string.Format("���:   <color=red>Lv.Max</color>", GameManager.instance.playerDashLevel);
                break;
            case 0:
                dashLevel.text = "���: Lv.- / Lv.4";
                break;
            default:
                dashLevel.text = string.Format("���: Lv.{0} / Lv.4", GameManager.instance.playerDashLevel);
                break;
        }
    }
      

    string GetClassName()
    {
        switch (GameManager.instance.playerId)
        {
            case 0:
                return "������";
            case 1:
                return "�߸�����";
            case 2:
                return "��ź��";
            default:
                Debug.Log("Incorrect playerId!");
                return "";
        }
    }
}
