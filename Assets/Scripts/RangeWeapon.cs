using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class RangeWeapon : MonoBehaviour
{
    public bool readyRangeWeapon;
    public int leftTime;

    int rangeId;
    float timer;
    ItemData rangeData;

    private void Awake()
    {
        readyRangeWeapon = false;
        rangeId = -1;
    }

    private void LateUpdate()
    {
        if (!readyRangeWeapon && rangeId != -1)
        {
            this.timer += Time.deltaTime;
        }
        // 장비가 변하면 원거리 장비의 아이템 Id 갱신
        if (rangeId != GameManager.instance.rangeWeaponItem)
        {
            rangeId = GameManager.instance.rangeWeaponItem;
            if (rangeId != -1)
            {
                rangeData = ItemManager.Instance.itemDataArr[rangeId];
                this.timer = 0;
                readyRangeWeapon = false;
            }
        }
        if (rangeId == -1)
            return;

        leftTime = Mathf.FloorToInt(Mathf.Abs(rangeData.coolTime - timer));

        if (timer > rangeData.coolTime)
        {
            readyRangeWeapon = true;
            timer = 0f;
        }


    }

    public void Fire(Vector3 rangeDir)
    {
        int prefabId = -1;
        GameObject rangePrefab = rangeData.projectile;
        for (int i = 0; i < PoolManager.instance.prefabs.Length; i++)
        {
            if (PoolManager.instance.prefabs[i] == rangePrefab)
            {
                prefabId = i;
                break;
            }
        }

        if (prefabId == -1)
        {
            Debug.Log("rangeWeapon.Fire() 함수에서 prefabId 변수 갱신 실패 : -1");
            return;
        }

        StartCoroutine(Shoot(rangeId, prefabId, rangeDir));

        readyRangeWeapon = false;
        timer = 0f;
    }

    IEnumerator Shoot(int rangeId, int prefabId, Vector3 rangeDir)
    {
        int count;
        if (rangeId == 6)
        {
            count = 3;
            GameObject[] projectiles = new GameObject[count];
            Vector3 deltaPos = GameManager.instance.player.rangeArrow.transform.GetChild(0).position - GameManager.instance.player.transform.position;
            Vector3 arrowSpritePos;
            Vector3 arrowAngle = GameManager.instance.player.rangeArrow.transform.localEulerAngles;
            Vector3 newDir = rangeDir;
            float rotAngle = 15f;

            for (int i = 0; i < count; i++)
            {
                projectiles[i] = PoolManager.instance.Get(prefabId);
                projectiles[i].transform.parent = PoolManager.instance.transform.GetChild(1);
                arrowSpritePos = GameManager.instance.player.transform.position + deltaPos;

                switch (i)
                {
                    case 0:
                        projectiles[i].transform.position = Quaternion.AngleAxis(rotAngle, Vector3.forward)
                                                       * (arrowSpritePos - GameManager.instance.player.rangeArrow.transform.position)
                                                       + GameManager.instance.player.rangeArrow.transform.position;
                        projectiles[i].transform.localEulerAngles = arrowAngle + new Vector3(0, 0, rotAngle);
                        newDir = Quaternion.AngleAxis(rotAngle, Vector3.forward) * rangeDir;
                        break;

                    case 1:
                        projectiles[i].transform.position = arrowSpritePos;
                        projectiles[i].transform.localEulerAngles = arrowAngle;
                        newDir = rangeDir;
                        break;

                    case 2:
                        projectiles[i].transform.position = Quaternion.AngleAxis(-rotAngle, Vector3.forward)
                                                       * (arrowSpritePos - GameManager.instance.player.rangeArrow.transform.position)
                                                       + GameManager.instance.player.rangeArrow.transform.position;
                        projectiles[i].transform.localEulerAngles = arrowAngle + new Vector3(0, 0, -rotAngle);
                        newDir = Quaternion.AngleAxis(-rotAngle, Vector3.forward) * rangeDir;
                        break;
                }

                projectiles[i].GetComponent<Projectile>().Init(rangeData.baseAmount, rangeData.pierceCount, newDir, rangeData.speed, rangeId);
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Kunai);
            }
            yield break;
        }
        else if (rangeId == 7)
        {
            count = 2;
            GameObject[] projectiles = new GameObject[count];
            Vector3 deltaPos = GameManager.instance.player.rangeArrow.transform.GetChild(0).position - GameManager.instance.player.transform.position;
            Vector3 arrowSpritePos;
            Vector3 arrowAngle = GameManager.instance.player.rangeArrow.transform.localEulerAngles;
            Vector3 newDir = rangeDir;

            for (int i = 0; i < count; i++)
            {
                projectiles[i] = PoolManager.instance.Get(prefabId);
                projectiles[i].transform.parent = PoolManager.instance.transform.GetChild(1); ;
                arrowSpritePos = GameManager.instance.player.transform.position + deltaPos;

                projectiles[i].transform.position = arrowSpritePos;
                projectiles[i].transform.localEulerAngles = arrowAngle;

                projectiles[i].GetComponent<Projectile>().Init(rangeData.baseAmount, rangeData.pierceCount, newDir, rangeData.speed, rangeId);
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Kunai);
                yield return new WaitForSeconds(0.15f);
            }
            yield break;
        }
        else if (rangeId == 8)
        {
            count = 1;
            GameObject[] projectiles = new GameObject[count];
            Vector3 deltaPos = GameManager.instance.player.rangeArrow.transform.GetChild(0).position - GameManager.instance.player.transform.position;
            Vector3 arrowSpritePos;
            Vector3 arrowAngle = GameManager.instance.player.rangeArrow.transform.localEulerAngles;
            Vector3 newDir = rangeDir;

            for (int i = 0; i < count; i++)
            {
                projectiles[i] = PoolManager.instance.Get(prefabId);
                projectiles[i].transform.parent = PoolManager.instance.transform.GetChild(1); ;
                arrowSpritePos = GameManager.instance.player.transform.position + deltaPos;

                projectiles[i].transform.position = arrowSpritePos;
                projectiles[i].transform.localEulerAngles = arrowAngle;

                projectiles[i].GetComponent<Projectile>().Init(rangeData.baseAmount, rangeData.pierceCount, newDir, rangeData.speed, rangeId);
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Arrow);
                //yield return new WaitForSeconds(0.15f);
            }
            yield break;
        }
    }
}
