using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] GameObject UI_Camera;

    public void StartServer()
    {
        Destroy(UI_Camera);
        NetworkManager.Singleton.StartServer();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Destroy(gameObject);
    }

    public void StartClient()
    {
        Destroy(UI_Camera);
        NetworkManager.Singleton.StartClient();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Destroy(gameObject);
    }

    public void StartHost()
    {
        Destroy(UI_Camera);
        NetworkManager.Singleton.StartHost();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
