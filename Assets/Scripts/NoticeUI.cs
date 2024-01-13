using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
                stageUnlockPanel.GetComponentInChildren<Text>(true).text = "<color=yellow>[���� ��]</color>����\r\n���� ���� ���Ƚ��ϴ�.";
                stageUnlockPanel.SetActive(true);
            }
        }
        else if (GameManager.instance.stageId == 1)
        {
            if (GameManager.instance.stage1_ClearCount == 1)
            {
                stageUnlockPanel.GetComponentInChildren<Text>(true).text = "<color=yellow>[����� ��]</color>����\r\n���� ���� ���Ƚ��ϴ�.";
                stageUnlockPanel.SetActive(true);
                characterUnlockPanel.GetComponentInChildren<Text>(true).text = "<color=yellow>[�߸�����]</color>��\r\n�շ��߽��ϴ�.";
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
