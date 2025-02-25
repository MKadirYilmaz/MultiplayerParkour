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
        Destroy(gameObject);
    }

    public void StartClient()
    {
        Destroy(UI_Camera);
        NetworkManager.Singleton.StartClient();
        Destroy(gameObject);
    }

    public void StartHost()
    {
        Destroy(UI_Camera);
        NetworkManager.Singleton.StartHost();
        Destroy(gameObject);
    }
}
