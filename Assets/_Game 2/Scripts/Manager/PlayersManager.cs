using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayersManager : NetworkBehaviour
{
    public static PlayersManager Instance;
    [SerializeField]
    NetworkVariable<int> playersInGame = new NetworkVariable<int>();

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
    void Start()
    {
        NetworkManager.OnServerStarted += () =>
        {
            if (IsServer)
            {
                playersInGame.Value = 0;
            }
        };
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if (IsServer)
            {
                LoggerDebug.Instance.LogInfo($"{id} just connected...");
                playersInGame.Value++;
            }
            UIManager.Instance.UIGamePlay.SetPlayerInGame(PlayersInGame);
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (IsServer)
            {
                LoggerDebug.Instance.LogInfo($"{id} just disconnected...");
                playersInGame.Value--;
            }
            UIManager.Instance.UIGamePlay.SetPlayerInGame(PlayersInGame);
        };
    }
}
