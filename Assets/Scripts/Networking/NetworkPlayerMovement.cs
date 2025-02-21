using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class NetworkPlayerMovement : NetworkBehaviour
{
    [SerializeField] private float speed = 10f;

    private InputSystem inputs;
    private Vector2 movementInput;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputs = new InputSystem();
        inputs.Enable();

        inputs.Player.Move.performed += context => movementInput = context.ReadValue<Vector2>().normalized;
        inputs.Player.Move.canceled += context => movementInput = Vector2.zero;

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        if(!IsOwner)
            return;
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);
        Vector3 moveVector = moveDirection * speed * Time.deltaTime;

        transform.position += moveVector;
    }
}
