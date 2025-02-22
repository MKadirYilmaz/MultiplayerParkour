using UnityEngine;
using Unity.Netcode;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private Camera attachedCamera;
    public InputSystem input {get; private set;}
    void Awake()
    {
        input = new InputSystem();
    }

    public override void OnNetworkSpawn()
    {
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
