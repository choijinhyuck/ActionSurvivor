using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class RangeWeapon : MonoBehaviour
{
    public Transform projectilePool;
    public bool readyRangeWeapon;

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
        if (rangeId != GameManager.Instance.rangeWeaponItem[GameManager.Instance.playerId])
        {
            rangeId = GameManager.Instance.rangeWeaponItem[GameManager.Instance.playerId];
            if (rangeId != -1)
            {
                rangeData = ItemManager.Instance.itemDataArr[rangeId];
                this.timer = 0;
                readyRangeWeapon = false;
            }
        }
        if (rangeId == -1)
            return;

        if (timer > rangeData.coolTime)
        {
            readyRangeWeapon = true;
            timer = 0f;
            Debug.Log("Ready");
        }

        
    }

    public void Fire(Vector3 rangeDir)
    {
        int prefabId = -1;
        GameObject rangePrefab = rangeData.projectile;
        for (int i = 0; i < GameManager.Instance.pool.prefabs.Length; i++)
        {
            if (GameManager.Instance.pool.prefabs[i] == rangePrefab)
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

        GameObject projectile = GameManager.Instance.pool.Get(prefabId);
        projectile.transform.parent = projectilePool;
        projectile.transform.position = GameManager.Instance.player.rangeArrow.transform.GetChild(0).position;
        projectile.transform.localRotation = Quaternion.FromToRotation(Vector3.right, rangeDir);
        projectile.GetComponent<Projectile>().Init(rangeData.baseAmount, rangeData.pierceCount, rangeDir, 10f);
        if (rangeId == 6 || rangeId == 7)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Kunai);
        }
        else
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Arrow);
        }

        readyRangeWeapon = false;
        timer = 0f;
    }
}
