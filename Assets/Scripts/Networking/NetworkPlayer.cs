using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class NetworkPlayer : NetworkBehaviour
{
    NetworkVariable<int> increament = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputSystem input = new InputSystem();
        input.Enable();

        input.Player.Interact.performed += context => 
        {
            increament.Value += 1;   
            Debug.Log(increament.Value);
        };
    }

    public override void OnNetworkSpawn()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
