using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bash : MonoBehaviour
{
    public void StopBash()
    {
        GameManager.Instance.player.StopBash();
    }
}
