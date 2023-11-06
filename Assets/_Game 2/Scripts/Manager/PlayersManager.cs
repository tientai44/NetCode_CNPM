using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayersManager : NetworkBehaviour
{
    public static PlayersManager Instance;
    [SerializeField]
    NetworkVariable<int> playersInGame = new NetworkVariable<int>();
    int currentPlayerInGame;
    public int PlayersInGame
    {
        get
        {
            return playersInGame.Value;
        }
        set
        {
            playersInGame.Value = value;
        }
    }
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if(currentPlayerInGame != PlayersInGame)
        {
            UIManager.Instance.UIGamePlay.SetPlayerInGame(PlayersInGame);
            currentPlayerInGame = PlayersInGame;
        }   
    }
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if (IsServer)
            {
                LoggerDebug.Instance.LogInfo($"{id} just connected...");
                playersInGame.Value= NetworkManager.Singleton.ConnectedClients.Count;
            }
            UIManager.Instance.UIGamePlay.SetPlayerInGame(PlayersInGame);
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (IsServer)
            {
                LoggerDebug.Instance.LogInfo($"{id} just disconnected...");
                playersInGame.Value= NetworkManager.Singleton.ConnectedClients.Count-1;
            }
            UIManager.Instance.UIGamePlay.SetPlayerInGame(PlayersInGame);
        };
    }
}
