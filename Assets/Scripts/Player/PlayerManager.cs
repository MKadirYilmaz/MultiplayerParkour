using UnityEngine;
using Unity.Netcode;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private Camera attachedCamera;
    [SerializeField] public GameManager gameManager;
    public InputSystem input {get; private set;}
    void Awake()
    {
        input = new InputSystem();
    }

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>().GetComponent<GameManager>();
    }

    public override void OnNetworkSpawn()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if(!IsOwner)
        {
            attachedCamera.enabled = false;
        }
        else
        {
            attachedCamera.enabled = true;
        }
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
