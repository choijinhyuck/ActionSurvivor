using UnityEngine;

public class Hammer : MonoBehaviour
{
    public UpgradeUI upgradeUI;

    public void Hammering()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Upgrade);
    }

    public void UpgradeComplete()
    {
        upgradeUI.FinishUpgrade();
        transform.parent.gameObject.SetActive(false);
    }
}
