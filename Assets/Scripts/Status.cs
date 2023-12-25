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
        speedValue.text = GameManager.Instance.playerSpeed.ToString("N0"); ;
        damageLevel.text = string.Format("  ��: Lv.{0} / Lv.3", GameManager.Instance.playerDamageLevel);
        speedLevel.text = string.Format("��ø: Lv.{0} / Lv.3", GameManager.Instance.playerSpeedLevel);
        healthLevel.text = string.Format("�ǰ�: Lv.{0} / Lv.4", GameManager.Instance.playerHealthLevel);
        skillLevel.text = string.Format("���: Lv.{0} / Lv.6", GameManager.Instance.playerSkillLevel);
        dashLevel.text = string.Format("���: Lv.{0} / Lv.4", GameManager.Instance.playerDashLevel);
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
