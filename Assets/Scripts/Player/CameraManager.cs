using Unity.Netcode;
using UnityEngine;

public class CameraManager : NetworkBehaviour
{
    [SerializeField] private Transform cameraTarget;

    private Quaternion targetRotation;

    void Start()
    {
        targetRotation = transform.rotation;
    }
    void Update()
    {
        transform.position = cameraTarget.position;

        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);
    }

    public void SetTargetRotation(Quaternion rot) => targetRotation = rot;
}
