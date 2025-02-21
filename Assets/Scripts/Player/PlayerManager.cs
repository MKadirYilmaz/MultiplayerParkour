using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public InputSystem input {get; private set;}
    void Awake()
    {
        input = new InputSystem();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }
}
