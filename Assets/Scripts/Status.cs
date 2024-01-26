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
        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
        {
            damageLevel.text = string.Format("    Èû: Lv.{0} / Lv.3", GameManager.instance.playerDamageLevel);
            speedLevel.text = string.Format("  ¹ÎÃ¸: Lv.{0} / Lv.3", GameManager.instance.playerSpeedLevel);
            healthLevel.text = string.Format("  °Ç°­: Lv.{0} / Lv.4", GameManager.instance.playerHealthLevel);
            skillLevel.text = string.Format("  ±â¼ú: Lv.{0} / Lv.6", GameManager.instance.playerSkillLevel);
            dashLevel.text = string.Format("  ´ë½Ã: Lv.{0} / Lv.4", GameManager.instance.playerDashLevel);
        }
        else
        {
            damageLevel.text = string.Format("Strength: Lv.{0} / Lv.3", GameManager.instance.playerDamageLevel);
            speedLevel.text = string.Format(" Agility: Lv.{0} / Lv.3", GameManager.instance.playerSpeedLevel);
            healthLevel.text = string.Format("  Health: Lv.{0} / Lv.4", GameManager.instance.playerHealthLevel);
            skillLevel.text = string.Format("   Skill: Lv.{0} / Lv.6", GameManager.instance.playerSkillLevel);
            dashLevel.text = string.Format("    Dash: Lv.{0} / Lv.4", GameManager.instance.playerDashLevel);
        }
        InitLanguage();
    }

    private void LateUpdate()
    {
        InitLanguage();

        className.text = GetClassName();
        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
        {
            damageValue.text = string.Format("°ø°Ý·Â: {0}", GameManager.instance.playerDamage.ToString("N0"));
            speedValue.text = string.Format("ÀÌµ¿¼Óµµ: {0}", GameManager.instance.playerSpeed.ToString("N0"));
        }
        else
        {
            damageValue.text = string.Format("Attack Damage: {0}", GameManager.instance.playerDamage.ToString("N0"));
            speedValue.text = string.Format("Movement Speed: {0}", GameManager.instance.playerSpeed.ToString("N0"));
        }
        

        switch (GameManager.instance.playerDamageLevel)
        {
            case 3:
                damageLevel.text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean
                                 ? string.Format("    Èû:   <color=red>Lv.Max</color>", GameManager.instance.playerDamageLevel)
                                 : string.Format("Strength:   <color=red>Lv.Max</color>", GameManager.instance.playerDamageLevel);
                break;
            case 0:
                damageLevel.text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ?
                                    "    Èû: Lv.- / Lv.3" : "Strength: Lv.- / Lv.3";
                break;
            default:
                damageLevel.text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? 
                                    string.Format("    Èû: Lv.{0} / Lv.3", GameManager.instance.playerDamageLevel) :
                                    string.Format("Strength: Lv.{0} / Lv.3", GameManager.instance.playerDamageLevel);
                break;
        }

        switch (GameManager.instance.playerSpeedLevel)
        {
            case 3:
                speedLevel.text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? 
                                    string.Format("  ¹ÎÃ¸:   <color=red>Lv.Max</color>", GameManager.instance.playerSpeedLevel) :
                                    string.Format(" Agility:   <color=red>Lv.Max</color>", GameManager.instance.playerSpeedLevel);
                break;
            case 0:
                speedLevel.text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? 
                                    "  ¹ÎÃ¸: Lv.- / Lv.3" : " Agility: Lv.- / Lv.3";
                break;
            default:
                speedLevel.text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? 
                                    string.Format("  ¹ÎÃ¸: Lv.{0} / Lv.3", GameManager.instance.playerSpeedLevel) :
                                    string.Format(" Agility: Lv.{0} / Lv.3", GameManager.instance.playerSpeedLevel);
                break;
        }

        switch (GameManager.instance.playerHealthLevel)
        {
            case 4:
                healthLevel.text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? 
                                    string.Format("  °Ç°­:   <color=red>Lv.Max</color>", GameManager.instance.playerHealthLevel) :
                                    string.Format("  Health:   <color=red>Lv.Max</color>", GameManager.instance.playerHealthLevel);
                break;
            case 0:
                healthLevel.text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? 
                                    "  °Ç°­: Lv.- / Lv.4" : "  Health: Lv.- / Lv.4";
                break;
            default:
                healthLevel.text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? 
                                    string.Format("  °Ç°­: Lv.{0} / Lv.4", GameManager.instance.playerHealthLevel) :
                                    string.Format("  Health: Lv.{0} / Lv.4", GameManager.instance.playerHealthLevel);
                break;
        }

        switch (GameManager.instance.playerSkillLevel)
        {
            case 6:
                skillLevel.text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? 
                                    string.Format("  ±â¼ú:   <color=red>Lv.Max</color>", GameManager.instance.playerSkillLevel) :
                                    string.Format("   Skill:   <color=red>Lv.Max</color>", GameManager.instance.playerSkillLevel);
                break;
            case 0:
                skillLevel.text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? 
                                    "  ±â¼ú: Lv.- / Lv.6" : "   Skill: Lv.- / Lv.6";
                break;
            default:
                skillLevel.text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? 
                                    string.Format("  ±â¼ú: Lv.{0} / Lv.6", GameManager.instance.playerSkillLevel) :
                                    string.Format("   Skill: Lv.{0} / Lv.6", GameManager.instance.playerSkillLevel);
                break;
        }

        switch (GameManager.instance.playerDashLevel)
        {
            case 4:
                dashLevel.text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? 
                                    string.Format("  ´ë½Ã:   <color=red>Lv.Max</color>", GameManager.instance.playerDashLevel) :
                                    string.Format("    Dash:   <color=red>Lv.Max</color>", GameManager.instance.playerDashLevel);
                break;
            case 0:
                dashLevel.text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ?
                                    "  ´ë½Ã: Lv.- / Lv.4" : "    Dash: Lv.- / Lv.4";
                break;
            default:
                dashLevel.text = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? 
                                    string.Format("  ´ë½Ã: Lv.{0} / Lv.4", GameManager.instance.playerDashLevel) :
                                    string.Format("    Dash: Lv.{0} / Lv.4", GameManager.instance.playerDashLevel);
                break;
        }
    }


    string GetClassName()
    {
        switch (GameManager.instance.playerId)
        {
            case 0:
                return SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "¿ö¸®¾î" : "Warrior";
            case 1:
                return SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "¾ß¸¸Àü»ç" : "Barbarian";
            case 2:
                return SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? "ÆøÅº¸Ç" : "BombGuy";
            default:
                Debug.Log("Incorrect playerId!");
                return "";
        }
    }

    void InitLanguage()
    {
        Dictionary<string, string[]> nameDic = new();
        nameDic["Status Title"] = new string[] { "´É·Â »óÅÂ", "Ability Status" };

        var texts = transform.parent.GetComponentsInChildren<Text>(true);
        int textId = SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean ? 0 : 1;
        foreach (var text in texts)
        {
            if (nameDic.ContainsKey(text.name))
            {
                text.text = nameDic[text.name][textId];
            }
        }
    }
}
