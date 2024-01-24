using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{
    [SerializeField] InputActionAsset actions;
    [SerializeField] Text stageName;
    [SerializeField] Text difficultyAndTime;
    [SerializeField] Text clearCount;
    [SerializeField] Image leftArrow;
    [SerializeField] Image rightArrow;
    [SerializeField] GameObject confirm;
    [SerializeField] Text stageNameConfirm;
    [SerializeField] Sprite[] controlIcons;
    [SerializeField] GameObject help;
    public GameObject stageSelectPanel;

    Vector2 lastPressedMove;
    int selectedId;
    InputSystemUIInputModule input;
    InputAction cancelAction;
    InputAction menuAction;
    GameObject lastSelected;
    bool confirmOn;

    private void Awake()
    {
        selectedId = 0;
        lastPressedMove = Vector2.zero;
        confirmOn = false;
        input = null;

        cancelAction = actions.FindActionMap("UI").FindAction("Cancel");
        menuAction = actions.FindActionMap("UI").FindAction("Menu");

        cancelAction.performed += CancelHandler;
        menuAction.performed += CancelHandler;
    }

    void CancelHandler(InputAction.CallbackContext context)
    {
        if (confirm.activeSelf)
        {
            if (!confirm.GetComponentsInChildren<Button>()[0].enabled) return;
            OnClick(false);
            return;
        }
        else if (stageSelectPanel.activeSelf)
        {
            stageSelectPanel.SetActive(false);
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
            GameManager.instance.isLive = true;
            return;
        }
    }

    private void OnDestroy()
    {
        cancelAction.performed -= CancelHandler;
        menuAction.performed -= CancelHandler;
    }

    private void Start()
    {
        if (stageSelectPanel.activeSelf) stageSelectPanel.SetActive(false);
        if (confirm.activeSelf) confirm.SetActive(false);
        if (help.activeSelf) help.SetActive(false);
    }

    private void Update()
    {
        if (help.activeSelf)
        {
            switch (ControllerManager.instance.CurrentScheme)
            {
                case ControllerManager.scheme.Keyboard:
                    help.GetComponentsInChildren<Image>()[0].sprite = controlIcons[0];
                    help.GetComponentsInChildren<Image>()[1].sprite = controlIcons[1];
                    break;
                case ControllerManager.scheme.Gamepad:
                    help.GetComponentsInChildren<Image>()[0].sprite = controlIcons[2];
                    help.GetComponentsInChildren<Image>()[1].sprite = controlIcons[3];
                    break;
            }
        }

        if (input == null)
        {
            input = (InputSystemUIInputModule)EventSystem.current.currentInputModule;
        }

        if (confirm.activeSelf)
        {
            if (lastSelected == null)
            {
                lastSelected = EventSystem.current.currentSelectedGameObject;
                return;
            }
            if (lastSelected != EventSystem.current.currentSelectedGameObject)
            {
                lastSelected = EventSystem.current.currentSelectedGameObject;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonChange);
            }
            return;
        }
        else if (confirmOn)
        {
            confirmOn = false;
            return;
        }


        if (!stageSelectPanel.activeSelf)
        {
            lastPressedMove = Vector2.zero;
            return;
        }


        if (input.submit.action.WasPerformedThisFrame())
        {
            // 확인 창 띄우기
            if (selectedId == 0
                || (selectedId == 1 && GameManager.instance.stage0_ClearCount > 0)
                || (selectedId == 2 && GameManager.instance.stage1_ClearCount > 0))
            {
                if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
                {
                    stageNameConfirm.text = "<" + StageManager.instance.stageDataArr[selectedId].stageName +
                                            ">\r\n<size=35><color=black>(으)로 떠나시겠습니까?</color></size>";
                    stageNameConfirm.transform.parent.GetComponentsInChildren<Text>(true)[1].text = "예";
                    stageNameConfirm.transform.parent.GetComponentsInChildren<Text>(true)[2].text = "아니요";
                    help.GetComponentsInChildren<Text>()[0].text = "선택";
                    help.GetComponentsInChildren<Text>()[1].text = "취소";
                }
                else
                {
                    stageNameConfirm.text = "<size=35><color=black>Leave for </color></size>\r\n"+
                                            "<" + StageManager.instance.stageDataArr[selectedId].stageNameEng +
                                            "><size=35><color=black>?</color></size>";
                    stageNameConfirm.transform.parent.GetComponentsInChildren<Text>(true)[1].text = "Yes";
                    stageNameConfirm.transform.parent.GetComponentsInChildren<Text>(true)[2].text = "No";
                    help.GetComponentsInChildren<Text>()[0].text = "Select";
                    help.GetComponentsInChildren<Text>()[1].text = "Cancel";
                }
                

                
                confirm.SetActive(true);

                AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonPress);
                EventSystem.current.SetSelectedGameObject(confirm.GetComponentsInChildren<Button>()[1].gameObject);
                confirmOn = true;
                return;
            }
        }

        if (input.move.action.ReadValue<Vector2>() != lastPressedMove)
        {
            if (input.move.action.ReadValue<Vector2>().x > 0)
            {
                if (selectedId < StageManager.instance.stageDataArr.Length - 1)
                {
                    selectedId++;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonChange);
                }
            }
            else if (input.move.action.ReadValue<Vector2>().x < 0)
            {
                if (selectedId > 0)
                {
                    selectedId--;
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonChange);
                }
            }
            lastPressedMove = input.move.action.ReadValue<Vector2>();
        }


        if (selectedId < StageManager.instance.stageDataArr.Length - 1)
        {
            if (!rightArrow.gameObject.activeSelf) rightArrow.gameObject.SetActive(true);
        }
        else
        {
            if (rightArrow.gameObject.activeSelf) rightArrow.gameObject.SetActive(false);
        }
        if (selectedId > 0)
        {
            if (!leftArrow.gameObject.activeSelf) leftArrow.gameObject.SetActive(true);
        }
        else
        {
            if (leftArrow.gameObject.activeSelf) leftArrow.gameObject.SetActive(false);
        }

        if (selectedId > 0)
        {
            switch (selectedId - 1)
            {
                case 0:
                    if (GameManager.instance.stage0_ClearCount == 0)
                    {
                        stageName.text = "<color=grey>???</color>";
                    }
                    else
                    {
                        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
                        {
                            stageName.text = StageManager.instance.stageDataArr[selectedId].stageName;
                        }
                        else
                        {
                            stageName.text = StageManager.instance.stageDataArr[selectedId].stageNameEng;
                        }
                    }
                    break;
                case 1:
                    if (GameManager.instance.stage1_ClearCount == 0)
                    {
                        stageName.text = "<color=grey>???</color>";
                    }
                    else
                    {
                        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
                        {
                            stageName.text = StageManager.instance.stageDataArr[selectedId].stageName;
                        }
                        else
                        {
                            stageName.text = StageManager.instance.stageDataArr[selectedId].stageNameEng;
                        }
                    }
                    break;
            }
        }
        else
        {
            if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
            {
                stageName.text = StageManager.instance.stageDataArr[0].stageName;
            }
            else
            {
                stageName.text = StageManager.instance.stageDataArr[0].stageNameEng;
            }
        }


        if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
        {
            string difficulty = selectedId switch
            {
                0 => "<color=grey>쉬움</color>",
                1 => "보통",
                2 => "<color=red>어려움</color>",
                _ => "알 수 없음",
            };
            string time = Mathf.FloorToInt(StageManager.instance.stageDataArr[selectedId].gameTime / 60).ToString() + "분";

            difficultyAndTime.text = difficulty + "    " + time;

            clearCount.text = selectedId switch
            {
                0 => GameManager.instance.stage0_ClearCount.ToString(),
                1 => GameManager.instance.stage1_ClearCount.ToString(),
                2 => GameManager.instance.stage2_ClearCount.ToString(),
                _ => "알 수 없음",
            } + "회 클리어";
        }
        else
        {
            string difficulty = selectedId switch
            {
                0 => "<color=grey>Easy</color>",
                1 => "Normal",
                2 => "<color=red>Hard</color>",
                _ => "Unknown",
            };
            string time = Mathf.FloorToInt(StageManager.instance.stageDataArr[selectedId].gameTime / 60).ToString() + "mins";

            difficultyAndTime.text = difficulty + "    " + time;

            clearCount.text = selectedId switch
            {
                0 => GameManager.instance.stage0_ClearCount.ToString(),
                1 => GameManager.instance.stage1_ClearCount.ToString(),
                2 => GameManager.instance.stage2_ClearCount.ToString(),
                _ => "Unknown",
            };
            if (clearCount.text == "0" || clearCount.text == "1")
            {
                clearCount.text += " time Clear";
            }
            else
            {
                clearCount.text += " times Clear";
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!stageSelectPanel.activeSelf) stageSelectPanel.SetActive(true);

            if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
            {
                GetComponentsInChildren<Text>(true)[^1].text = "모험 장소 선택";
                help.GetComponentsInChildren<Text>()[0].text = "출발";
                help.GetComponentsInChildren<Text>()[1].text = "닫기";
            }
            else
            {
                GetComponentsInChildren<Text>(true)[^1].text = "Choose Your Destination";
                help.GetComponentsInChildren<Text>()[0].text = "Leave";
                help.GetComponentsInChildren<Text>()[1].text = "Close";
            }
            
            if (!help.activeSelf) help.SetActive(true);
            GameManager.instance.isLive = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (confirm.activeSelf) confirm.SetActive(false);
        if (stageSelectPanel.activeSelf) stageSelectPanel.SetActive(false);
        GameManager.instance.isLive = true;
    }

    public void OnClick(bool yes)
    {
        if (yes)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.MenuSelect);
            confirm.GetComponentsInChildren<Button>()[0].enabled = false;
            switch (selectedId)
            {
                case 0:
                    GameManager.instance.sceneName = "Stage_0";
                    break;
                case 1:
                    GameManager.instance.sceneName = "Stage_1";
                    break;
                case 2:
                    GameManager.instance.sceneName = "Stage_2";
                    break;
            }
            GameManager.instance.FadeOut();
        }
        else
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancel);
            EventSystem.current.SetSelectedGameObject(null);

            if (SettingUI.instance.currLanguage == SettingUI.LanguageType.Korean)
            {
                help.GetComponentsInChildren<Text>()[0].text = "출발";
                help.GetComponentsInChildren<Text>()[1].text = "닫기";
            }
            else
            {
                help.GetComponentsInChildren<Text>()[0].text = "Leave";
                help.GetComponentsInChildren<Text>()[1].text = "Close";
            }

            confirm.SetActive(false);
            lastSelected = null;
        }
    }
}
