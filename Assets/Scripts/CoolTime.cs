using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoolTime : MonoBehaviour
{
    public GameObject range;
    public GameObject magic;
    public RangeWeapon rangeWeapon;
    public Magic magicScript;

    bool prevRangeReady;
    bool prevMagicReady;
    int prevRangeId;
    int prevMagicId;

    private void OnEnable()
    {
        prevRangeReady = false;
        prevMagicReady = false;
        prevRangeId = -1;
        prevMagicId = -1;
    }

    private void LateUpdate()
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == 0)
            {
                // 투척 무기 처리
                if (GameManager.instance.rangeWeaponItem == -1)
                {
                    if (range.activeSelf)
                    {
                        range.SetActive(false);
                    }
                    prevRangeReady = false;
                    prevRangeId = -1;
                    continue;
                }
                if (!range.activeSelf)
                {
                    range.SetActive(true);
                }
                if (prevRangeId != GameManager.instance.rangeWeaponItem)
                {
                    prevRangeId = GameManager.instance.rangeWeaponItem;
                    range.GetComponent<Image>().sprite = ItemManager.Instance.itemDataArr[prevRangeId].itemIcon;
                }

                if (rangeWeapon.readyRangeWeapon)
                {
                    if (prevRangeReady) continue;
                    if (range.transform.GetChild(0).gameObject.activeSelf) range.transform.GetChild(0).gameObject.SetActive(false);
                    range.GetComponent<Image>().color = Color.white;
                    // 아이콘 애니메이션
                    StartCoroutine(ReadyCharge(range.GetComponent<RectTransform>()));
                    prevRangeReady = true;
                }
                else
                {

                    if (!range.transform.GetChild(0).gameObject.activeSelf) range.transform.GetChild(0).gameObject.SetActive(true);
                    range.transform.GetChild(0).gameObject.GetComponent<Text>().text = rangeWeapon.leftTime.ToString() + "초";


                    range.GetComponent<Image>().color = new Color(1f, 1f, 1f, .3f);
                    if (prevRangeReady)
                    {
                        prevRangeReady = false;
                    }
                }
            }
            else if (i == 1)
            {
                // 마법 처리
                if (GameManager.instance.magicItem == -1)
                {
                    if (magic.activeSelf)
                    {
                        magic.SetActive(false);
                    }
                    prevMagicReady = false;
                    prevMagicId = -1;
                    continue;
                }
                if (!magic.activeSelf)
                {
                    magic.SetActive(true);
                }
                if (prevMagicId != GameManager.instance.magicItem)
                {
                    prevMagicId = GameManager.instance.magicItem;
                    magic.GetComponent<Image>().sprite = ItemManager.Instance.itemDataArr[prevMagicId].itemIcon;
                }

                if (magicScript.readyMagic)
                {
                    if (prevMagicReady) continue;
                    if (magic.transform.GetChild(0).gameObject.activeSelf) magic.transform.GetChild(0).gameObject.SetActive(false);
                    magic.GetComponent<Image>().color = Color.white;
                    // 아이콘 애니메이션
                    StartCoroutine(ReadyCharge(magic.GetComponent<RectTransform>()));
                    prevMagicReady = true;
                }
                else
                {

                    if (!magic.transform.GetChild(0).gameObject.activeSelf) magic.transform.GetChild(0).gameObject.SetActive(true);
                    magic.transform.GetChild(0).gameObject.GetComponent<Text>().text = magicScript.leftTime.ToString() + "초";


                    magic.GetComponent<Image>().color = new Color(1f, 1f, 1f, .3f);
                    if (prevMagicReady)
                    {
                        prevMagicReady = false;
                    }
                }
            }
        }
        

        //
    }

    IEnumerator ReadyCharge(RectTransform readyChargeRect)
    {
        Vector3 originScale = readyChargeRect.localScale;
        WaitForFixedUpdate waitFix = new WaitForFixedUpdate();

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
