using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : MonoBehaviour
{
    public static ControllerManager instance;
    public enum scheme { Keyboard, Gamepad, Undefined }
    public scheme CurrentScheme;
    public bool isSwitched;

    PlayerInput playerInput;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        playerInput = GetComponent<PlayerInput>();
        CurrentScheme = scheme.Undefined;
    }

    private void Start()
    {
        CurrentScheme = playerInput.currentControlScheme == "Keyboard&Mouse" ? scheme.Keyboard : scheme.Gamepad;
    }

    void OnControlsChanged()
    {
        if (playerInput != null)
        {
            CurrentScheme = playerInput.currentControlScheme == "Keyboard&Mouse" ? scheme.Keyboard : scheme.Gamepad;
        }
    }
}
