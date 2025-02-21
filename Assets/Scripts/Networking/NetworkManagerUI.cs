using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] GameObject UI_Camera;

    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button hostButton;

    private void Awake()
    {
        serverButton.onClick.AddListener(() => {
            Destroy(UI_Camera);
            NetworkManager.Singleton.StartServer();
            Destroy(gameObject);
        });
        clientButton.onClick.AddListener(() => {
            Destroy(UI_Camera);
            NetworkManager.Singleton.StartClient();
            Destroy(gameObject);
        });
        hostButton.onClick.AddListener(() => {
            Destroy(UI_Camera);
            NetworkManager.Singleton.StartHost();
            Destroy(gameObject);
        });
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
