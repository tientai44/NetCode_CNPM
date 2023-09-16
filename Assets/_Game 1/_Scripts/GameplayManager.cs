using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class GameplayManager : SingletonNetwork<GameplayManager>
{
    private List<ulong> m_connectedClients = new List<ulong>();

    [SerializeField]
    private Transform[] m_shipStartingPositions;
    private int m_numberOfPlayerConnected;

    [SerializeField]
    private GameObject m_player;
    public void ServerSceneInit(ulong clientId)
    {
        // Save the clients 
        m_connectedClients.Add(clientId);

        // Check if is the last client
        //if (m_connectedClients.Count < NetworkManager.Singleton.ConnectedClients.Count)
        //    return;

        // For each client spawn and set UI
        //for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
        //{
            GameObject playerSpaceship =
                NetworkObjectSpawner.SpawnNewNetworkObjectAsPlayerObject(
                    m_player,
                    m_shipStartingPositions[m_numberOfPlayerConnected].position,
                    clientId,
                    true);

            PlayerShipController playerShipController =
                playerSpaceship.GetComponent<PlayerShipController>();
            //playerShipController.characterData = data;
            //playerShipController.gameplayManager = this;

            //m_playerShips.Add(playerShipController);
            //SetPlayerUIClientRpc(index, playerSpaceship.name);

            m_numberOfPlayerConnected++;
        //}
    }
}
