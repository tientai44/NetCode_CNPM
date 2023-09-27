using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayersManager : NetworkBehaviour
{
    public static PlayersManager Instance;
    NetworkVariable<int> playersInGame = new NetworkVariable<int>();

    public int PlayersInGame
    {
        get
        {
            return playersInGame.Value;
        }
    }
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if (IsServer)
            {
                LoggerDebug.Instance.LogInfo($"{id} just connected...");
                playersInGame.Value++;
            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (IsServer)
            {
                LoggerDebug.Instance.LogInfo($"{id} just disconnected...");

                playersInGame.Value--;
            }
        
        };
    }
    public void DisconnectPlayer(NetworkObject player)
    {
        // Note: If a client invokes this method, it will throw an exception.
        if (IsServer)
        {
            NetworkManager.DisconnectClient(player.OwnerClientId);
            LoggerDebug.Instance.LogInfo($"{player.OwnerClientId} just disconnected...");

            playersInGame.Value--;
        }
    }
}
