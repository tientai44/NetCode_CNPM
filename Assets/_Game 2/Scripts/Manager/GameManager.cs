using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool hasServerStarted;

    private static int maxConn = 10;
    public static int MaxConn { get { return maxConn; } }

    private void Awake()
    {
        Instance = this;

    }
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            hasServerStarted = true;
        };
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
    }
    private void HandleClientDisconnected(ulong clientId)
    {
        UIManager.Instance.Notify($"Player disconnected with client ID : {clientId}");
        Debug.Log("Player Disconnected with client ID: " + clientId);
        if (!hasServerStarted)
        {
            UIManager.Instance.Notify($"The owner of the room has disbanded this room");
            NetworkManager.Singleton.Shutdown();
            UIManager.Instance.UI_MainMenu.Show();
        }
    }
}
