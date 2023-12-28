using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public float damageRate;
    public List<GameObject> hitList;
    
    private void OnEnable()
    {
        hitList = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (hitList.Contains(collision.gameObject)) return;
            hitList.Add(collision.gameObject);
        }
    }

}
