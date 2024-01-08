using UnityEngine;
using UnityEngine.UI;

public class PlayerBars : MonoBehaviour
{
    public Slider dodgeBar;
    public Slider chargeBar;

    [Header("# Skill Images")]
    public GameObject[] empty;
    public GameObject[] warrior;
    public GameObject[] barbarian;
    public GameObject[] bombGuy;
    public GameObject[] locked;

    Player player;
    RectTransform dodgeBarRect;
    RectTransform chargeRect;


    float dodgeTimer;
    float[] dodgeBarPosY;
    float[] chargePosY;

    private void Awake()
    {
        dodgeTimer = 0f;
        dodgeBarPosY = new float[] { 30f, 25f, 25f };
        chargePosY = new float[] { -1.5f, -3.5f, -3.5f };

    }

    void Start()
    {
        dodgeBarRect = dodgeBar.GetComponent<RectTransform>();
        chargeRect = chargeBar.transform.parent.GetComponent<RectTransform>();

        player = GameManager.instance.player;
        dodgeBarRect.anchoredPosition = new Vector2(dodgeBarRect.anchoredPosition.x, dodgeBarPosY[GameManager.instance.playerId]);
        chargeRect.anchoredPosition = new Vector2(chargeRect.anchoredPosition.x, chargePosY[GameManager.instance.playerId]);

        Init();
    }

    void FixedUpdate()
    {
        transform.position = Camera.main.WorldToScreenPoint(player.transform.position);
    }

    private void LateUpdate()
    {
        UpdateDodgeBar();
        UpdateChargeBar();

    }

    void UpdateDodgeBar()
    {
        if (!GameManager.instance.isLive)
        {
            if (dodgeBar.gameObject.activeSelf)
            {
                dodgeBar.gameObject.SetActive(false);
            }
            return;
        }

        dodgeBarRect.anchoredPosition = new Vector2(dodgeBarRect.anchoredPosition.x, dodgeBarPosY[GameManager.instance.playerId]);
        if (!player.readyDodge)
        {
            if (GameManager.instance.health < .1f)
            {
                if (dodgeBar.gameObject.activeSelf)
                {
                    dodgeBar.gameObject.SetActive(false);
                }
            }
            else
            {
                if (!dodgeBar.gameObject.activeSelf)
                {
                    dodgeBar.gameObject.SetActive(true);
                }
            }

            dodgeTimer += Time.deltaTime;
            dodgeBar.value = dodgeTimer / GameManager.instance.dodgeTime;

            if (dodgeTimer > GameManager.instance.dodgeTime)
            {
                dodgeTimer = 0f;
                player.readyDodge = true;
            }
        }
        else
        {
            if (dodgeBar.gameObject.activeSelf)
            {
                dodgeBar.gameObject.SetActive(false);
            }
        }
    }

    void UpdateChargeBar()
    {
        if (!GameManager.instance.isLive)
        {
            if (chargeRect.gameObject.activeSelf)
            {
                chargeRect.gameObject.SetActive(false);
            }
            return;
        }

        chargeRect.anchoredPosition = new Vector2(chargeRect.anchoredPosition.x, chargePosY[GameManager.instance.playerId]);
        if (player.isCharging)
        {
            if (!chargeRect.gameObject.activeSelf)
            {
                chargeRect.gameObject.SetActive(true);
            }

            // ****** Past UI script *******
            //if (player.chargeCount == GameManager.Instance.maxChargibleCount || player.chargeCount == GameManager.Instance.chargeCount)
            //{
            //    chargeBar.value = 1f;
            //}
            //else
            //{
            //    chargeBar.value = player.chargeTimer / GameManager.Instance.chargeTime;
            //}
            //chargeRect.GetChild(1).GetComponent<Text>().text =string.Format("<color=#FFFF00>{0:F0}</color><size=2> </size>/<size=2> </size>{1:F0}",
            //    player.chargeCount, GameManager.Instance.maxChargibleCount);

            if (player.chargeCount == GameManager.instance.maxChargibleCount || player.chargeCount == GameManager.instance.chargeCount)
            {
                chargeBar.value = 1f;
            }
            else
            {
                chargeBar.value = player.chargeTimer / GameManager.instance.chargeTime;
            }

            for (int i = 0; i < 3; i++)
            {
                if (i < GameManager.instance.maxChargibleCount)
                {
                    if (player.chargeCount > i)
                    {
                        switch (GameManager.instance.playerId)
                        {
                            case 0:
                                warrior[i].SetActive(true);
                                break;

                            case 1:
                                barbarian[i].SetActive(true);
                                break;

                            case 2:
                                bombGuy[i].SetActive(true);
                                break;
                        }
                    }
                    else
                    {
                        if (!empty[i].activeSelf) empty[i].SetActive(true);
                    }
                }
                else
                {
                    if (!locked[i].activeSelf) locked[i].SetActive(true);
                }
            }


        }
        else
        {
            if (chargeRect.gameObject.activeSelf)
            {
                chargeRect.gameObject.SetActive(false);
            }

            for (int i = 0; i < 3; i++)
            {
                switch (GameManager.instance.playerId)
                {
                    case 0:
                        warrior[i].SetActive(false);
                        break;

                    case 1:
                        barbarian[i].SetActive(false);
                        break;

                    case 2:
                        bombGuy[i].SetActive(false);
                        break;
                }
                empty[i].SetActive(false);
                locked[i].SetActive(false);
            }
        }
    }

    void Init()
    {
        for (int i = 0; i < 3; i++)
        {
            warrior[i].SetActive(false);
            barbarian[i].SetActive(false);
            bombGuy[i].SetActive(false);
            empty[i].SetActive(false);
            locked[i].SetActive(false);
        }
    }
}
