using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Magic : MonoBehaviour
{
    public Transform projectilePool;
    public bool readyMagic;
    public int leftTime;

    int magicId;
    float timer;
    ItemData magicData;

    private void Awake()
    {
        readyMagic = false;
        magicId = -1;
    }

    private void LateUpdate()
    {
        if (!readyMagic && magicId != -1)
        {
            this.timer += Time.deltaTime;
        }
        // 장비가 변하면 원거리 장비의 아이템 Id 갱신
        if (magicId != GameManager.Instance.magicItem)
        {
            magicId = GameManager.Instance.magicItem;
            if (magicId != -1)
            {
                magicData = ItemManager.Instance.itemDataArr[magicId];
                this.timer = 0;
                readyMagic = false;
            }
        }
        if (magicId == -1)
            return;

        leftTime = Mathf.FloorToInt(Mathf.Abs(magicData.coolTime - timer));

        if (timer > magicData.coolTime)
        {
            readyMagic = true;
            timer = 0f;
        }


    }

    public void Fire(Vector3 magicDir)
    {
        int prefabId = -1;
        GameObject magicPrefab = magicData.projectile;
        for (int i = 0; i < GameManager.Instance.pool.prefabs.Length; i++)
        {
            if (GameManager.Instance.pool.prefabs[i] == magicPrefab)
            {
                prefabId = i;
                break;
            }
        }

        if (prefabId == -1)
        {
            Debug.Log("Magic.Fire() 함수에서 prefabId 변수 갱신 실패 : -1");
            return;
        }

        GameObject projectile = GameManager.Instance.pool.Get(prefabId);
        projectile.transform.parent = projectilePool;
        projectile.transform.position = GameManager.Instance.player.rangeArrow.transform.GetChild(0).position;
        projectile.transform.localRotation = Quaternion.FromToRotation(Vector3.right, magicDir);
        projectile.GetComponent<Projectile>().Init(magicData.baseAmount, magicData.pierceCount, magicDir, magicData.speed);
        //if (rangeId == 6 || rangeId == 7)
        //{
        //    AudioManager.instance.PlaySfx(AudioManager.Sfx.Kunai);
        //}
        //else
        //{
        //    AudioManager.instance.PlaySfx(AudioManager.Sfx.Arrow);
        //}

        readyMagic = false;
        timer = 0f;
    }
}
