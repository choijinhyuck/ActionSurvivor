using UnityEngine;
using UnityEngine.UI;

public class Portrait : MonoBehaviour
{
    public Sprite[] sprites;

    Image portrait;

    private void Awake()
    {
        portrait = GetComponent<Image>();
    }
    private void Start()
    {
        GetPortrait();
    }

    private void LateUpdate()
    {
        GetPortrait();
    }

    void GetPortrait()
    {
        switch (GameManager.instance.playerId)
        {
            case 0:
                portrait.sprite = sprites[0];
                break;

            case 1:
                portrait.sprite = sprites[1];
                break;

            case 2:
                portrait.sprite = sprites[2];
                break;

            default:
                portrait.sprite = null;
                Debug.Log("Incorrect playerId from portrait GameObject");
                break;
        }
    }
}
