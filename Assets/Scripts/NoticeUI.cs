using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NoticeUI : MonoBehaviour
{
    [SerializeField] GameObject stageUnlockPanel;
    [SerializeField] GameObject characterUnlockPanel;

    private void Awake()
    {
        if (GameManager.instance.stageId == 0)
        {
            if (GameManager.instance.stage0_ClearCount == 1)
            {
                if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
                {
                    stageUnlockPanel.GetComponentInChildren<Text>(true).text = "<color=yellow>[깊은 숲]</color>으로\r\n가는 길이 열렸습니다.";
                }
                else
                {
                    stageUnlockPanel.GetComponentInChildren<Text>(true).text = "The path to\r\n<color=yellow>[Deep Forest]</color> is open.";
                }
                stageUnlockPanel.SetActive(true);
            }
        }
        else if (GameManager.instance.stageId == 1)
        {
            if (GameManager.instance.stage1_ClearCount == 1)
            {
                if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
                {
                    stageUnlockPanel.GetComponentInChildren<Text>(true).text = "<color=yellow>[고블린의 숲]</color>으로\r\n가는 길이 열렸습니다.";
                    characterUnlockPanel.GetComponentInChildren<Text>(true).text = "<color=yellow>[야만전사]</color>가\r\n합류했습니다.";
                }
                else
                {
                    stageUnlockPanel.GetComponentInChildren<Text>(true).text = "The path to\r\n<color=yellow>[Goblin Forest]</color> is open.";
                    characterUnlockPanel.GetComponentInChildren<Text>(true).text = "<color=yellow>[Barbarian]</color>\r\nhas joined.";
                }
                stageUnlockPanel.SetActive(true);
                characterUnlockPanel.SetActive(true);
            }
        }
    }

    private void Start()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Notice);
        StartCoroutine(DestroyCoroutine());
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSecondsRealtime(5f);
        GetComponentInChildren<Animator>().SetTrigger("Close");
        yield return new WaitForSecondsRealtime(2f);
        Destroy(gameObject);
    }
}
