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
        damageLevel.text = string.Format("  ��: Lv.{0} / Lv.3", GameManager.Instance.playerDamageLevel);
        speedLevel.text = string.Format("��ø: Lv.{0} / Lv.3", GameManager.Instance.playerSpeedLevel);
        healthLevel.text = string.Format("�ǰ�: Lv.{0} / Lv.4", GameManager.Instance.playerHealthLevel);
        skillLevel.text = string.Format("���: Lv.{0} / Lv.6", GameManager.Instance.playerSkillLevel);
        dashLevel.text = string.Format("���: Lv.{0} / Lv.4", GameManager.Instance.playerDashLevel);
    }

    private void LateUpdate()
    {
        className.text = GetClassName();
        damageValue.text = GameManager.Instance.playerDamage.ToString("N0");
        speedValue.text = GameManager.Instance.playerSpeed.ToString("N0");

        switch (GameManager.Instance.playerDamageLevel)
        {
            case 3:
                damageLevel.text = string.Format("  ��:   <color=red>Lv.Max</color>", GameManager.Instance.playerDamageLevel);
                break;
            case 0:
                damageLevel.text = "  ��: Lv.- / Lv.3";
                break;
            default:
                damageLevel.text = string.Format("  ��: Lv.{0} / Lv.3", GameManager.Instance.playerDamageLevel);
                break;
        }

        switch (GameManager.Instance.playerSpeedLevel)
        {
            case 3:
                speedLevel.text = string.Format("��ø:   <color=red>Lv.Max</color>", GameManager.Instance.playerSpeedLevel);
                break;
            case 0:
                speedLevel.text = "��ø: Lv.- / Lv.3";
                break;
            default:
                speedLevel.text = string.Format("��ø: Lv.{0} / Lv.3", GameManager.Instance.playerSpeedLevel);
                break;
        }

        switch (GameManager.Instance.playerHealthLevel)
        {
            case 4:
                healthLevel.text = string.Format("�ǰ�:   <color=red>Lv.Max</color>", GameManager.Instance.playerHealthLevel);
                break;
            case 0:
                healthLevel.text = "�ǰ�: Lv.- / Lv.4";
                break;
            default:
                healthLevel.text = string.Format("�ǰ�: Lv.{0} / Lv.4", GameManager.Instance.playerHealthLevel);
                break;
        }

        switch (GameManager.Instance.playerSkillLevel)
        {
            case 6:
                skillLevel.text = string.Format("���:   <color=red>Lv.Max</color>", GameManager.Instance.playerSkillLevel);
                break;
            case 0:
                skillLevel.text = "���: Lv.- / Lv.6";
                break;
            default:
                skillLevel.text = string.Format("���: Lv.{0} / Lv.6", GameManager.Instance.playerSkillLevel);
                break;
        }
        
        switch(GameManager.Instance.playerDashLevel)
        {
            case 4:
                dashLevel.text = string.Format("���:   <color=red>Lv.Max</color>", GameManager.Instance.playerDashLevel);
                break;
            case 0:
                dashLevel.text = "���: Lv.- / Lv.4";
                break;
            default:
                dashLevel.text = string.Format("���: Lv.{0} / Lv.4", GameManager.Instance.playerDashLevel);
                break;
        }
    }
      

    string GetClassName()
    {
        switch (GameManager.Instance.playerId)
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
