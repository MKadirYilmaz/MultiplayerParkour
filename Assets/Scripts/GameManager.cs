using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public enum GameMode
    {
        Singleplayer,
        Multiplayer
    }
    [SerializeField] public GameMode gameMode = GameMode.Singleplayer;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject networkUI;
    void Start()
    {
        switch(gameMode)
        {
            case GameMode.Singleplayer:
            Instantiate(playerPrefab);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            break;
            case GameMode.Multiplayer:
            Instantiate(networkUI);
            break;
        }
    }
}
