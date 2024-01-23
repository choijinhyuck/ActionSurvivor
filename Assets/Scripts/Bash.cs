using UnityEngine;

public class Bash : MonoBehaviour
{
    public void StopBash()
    {
        GameManager.instance.player.StopBash();
    }
}
