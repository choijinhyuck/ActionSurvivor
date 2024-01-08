using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ChargeCount : MonoBehaviour
{
    public Sprite[] skillPercentages;
    public Sprite skillReady;

    WaitForFixedUpdate waitFix;

    int maxChargeCount;
    float chargeTimer;

    private void Awake()
    {
        waitFix = new WaitForFixedUpdate();
    }

    private void Start()
    {
        Init();
        chargeTimer = 0f;
    }

    void Init()
    {
        maxChargeCount = GameManager.instance.maxChargeCount;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (i < maxChargeCount)
            {
                if (!transform.GetChild(i).gameObject.activeSelf)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
                transform.GetChild(i).GetComponent<Image>().sprite = skillReady;
            }
            else
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
                transform.GetChild(i).GetComponent<Image>().sprite = skillReady;
            }
        }
    }

    private void LateUpdate()
    {
        if (maxChargeCount != GameManager.instance.maxChargeCount)
        {
            Init();
        }

        for (int i = 0; i < maxChargeCount; i++)
        {
            if (i < GameManager.instance.chargeCount)
            {
                transform.GetChild(i).GetComponent<Image>().sprite = skillReady;
            }
            if (i == GameManager.instance.chargeCount)
            {
                chargeTimer += Time.deltaTime;

                if (chargeTimer > GameManager.instance.chargeCooltime)
                {
                    chargeTimer = 0;
                    transform.GetChild(i).GetComponent<Image>().sprite = skillPercentages[skillPercentages.Length - 1];
                    StartCoroutine(ReadyCharge(transform.GetChild(i).GetComponent<RectTransform>()));
                    GameManager.instance.chargeCount++;

                }
                else
                {
                    int spriteIndex = Mathf.FloorToInt(chargeTimer * 10 / GameManager.instance.chargeCooltime);
                    transform.GetChild(i).GetComponent<Image>().sprite = skillPercentages[spriteIndex];
                }

            }
            if (i > GameManager.instance.chargeCount)
            {
                transform.GetChild(i).GetComponent<Image>().sprite = skillPercentages[0];
            }

        }
    }

    IEnumerator ReadyCharge(RectTransform readyChargeRect)
    {
        Vector3 originScale = readyChargeRect.localScale;

        readyChargeRect.localScale = originScale * 1.1f;
        yield return waitFix;
        readyChargeRect.localScale = originScale * 1.2f;
        yield return waitFix;
        readyChargeRect.localScale = originScale * 1.3f;
        yield return waitFix;
        readyChargeRect.localScale = originScale * 1.4f;
        yield return waitFix;
        readyChargeRect.localScale = originScale * 1.5f;
        yield return waitFix;
        readyChargeRect.localScale = originScale * 1.4f;
        yield return waitFix;
        readyChargeRect.localScale = originScale * 1.3f;
        yield return waitFix;
        readyChargeRect.localScale = originScale * 1.2f;
        yield return waitFix;
        readyChargeRect.localScale = originScale * 1.1f;
        yield return waitFix;
        readyChargeRect.localScale = originScale;
        yield return waitFix;
    }
}
