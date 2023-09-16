using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CharacterContainer
{
    public Image imageContainer;                    // The image of the character container
    public TextMeshProUGUI nameContainer;           // Character name container
    public GameObject border;                       // The border of the character container when not ready
    public GameObject borderReady;                  // The border of the character container when ready
    public GameObject borderClient;                 // Client border of the character container
    public Image playerIcon;                        // The background icon of the player (p1, p2)
    public GameObject waitingText;                  // The waiting text on the container were no client connected
    public GameObject backgroundShip;               // The background of the ship when not ready
    public Image backgroundShipImage;               // The image of the ship when not ready
    public GameObject backgroundShipReady;          // The background of the ship when ready
    public Image backgroundShipReadyImage;          // The image of the ship when ready
    public GameObject backgroundClientShipReady;    // Client background of the ship when ready
    public Image backgroundClientShipReadyImage;    // Client image of the ship when ready
}

public class CharacterSelectionManager : SingletonNetwork<CharacterSelectionManager>
{
    [SerializeField]
    CharacterContainer[] m_charactersContainers;

    [SerializeField]
    GameObject m_playerPrefab;

    public List<int> m_playerStates;

    [SerializeField]
    GameObject m_readyButton;

    [SerializeField]
    SceneName m_nextScene = SceneName.Gameplay;
    public void SetCharacterUI(int playerId, int characterSelected)
    {
        Debug.Log(playerId);
        m_charactersContainers[playerId].imageContainer.gameObject.SetActive(true);


        m_charactersContainers[playerId].nameContainer.text = playerId.ToString();
    }

    public int GetPlayerId(ulong clientId)
    {
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        Debug.Log("Số lượng người chơi trong phòng: " + playerCount);
        Debug.Log(clientId);
        //return 0;
        for (int i = 0; i < m_playerStates.Count; i++)
        {
            if (m_playerStates[i] == int.Parse(clientId.ToString()))
                return i;
        }

        //! This should never happen
        Debug.LogError("This should never happen");
        return -1;
    }

    public void SetPlayebleChar(int playerId, int characterSelected, bool isClientOwner)
    {
        SetCharacterUI(playerId, characterSelected);
        //m_charactersContainers[playerId].playerIcon.gameObject.SetActive(true);
        //if (isClientOwner)
        //{
        //    m_charactersContainers[playerId].borderClient.SetActive(true);
        //    m_charactersContainers[playerId].border.SetActive(false);
        //    m_charactersContainers[playerId].borderReady.SetActive(false);
        //    //m_charactersContainers[playerId].playerIcon.color = m_clientColor;
        //}
        //else
        //{
        //    m_charactersContainers[playerId].border.SetActive(true);
        //    m_charactersContainers[playerId].borderReady.SetActive(false);
        //    m_charactersContainers[playerId].borderClient.SetActive(false);
        //    //m_charactersContainers[playerId].playerIcon.color = m_playerColor;
        //}

        //m_charactersContainers[playerId].backgroundShip.SetActive(true);
        //m_charactersContainers[playerId].waitingText.SetActive(false);
    }

    public void SetPlayerReadyUIButtons(bool isReady, int characterSelected)
    {
        if (isReady )
        {
            m_readyButton.SetActive(false);
        }
        else if (!isReady )
        {
            m_readyButton.SetActive(true);
        }
    }

    public void PlayerReady(ulong clientId, int playerId, int characterSelected)
    {
        LoadingSceneManager.Instance.LoadScene(m_nextScene);
    }

    public void ServerSceneInit(ulong clientId)
    {
        GameObject go =
            NetworkObjectSpawner.SpawnNewNetworkObjectChangeOwnershipToClient(
                m_playerPrefab,
                transform.position,
                clientId,
                true);

        //for (int i = 0; i < m_playerStates.Length; i++)
        //{
        //    if (m_playerStates[i].playerState == ConnectionState.disconnected)
        //    {
        //        m_playerStates[i].playerState = ConnectionState.connected;
        //        m_playerStates[i].playerObject = go.GetComponent<PlayerCharSelection>();
        //        m_playerStates[i].playerName = go.name;
        //        m_playerStates[i].clientId = clientId;

        //        // Force the exit
        //        break;
        //    }
        //}

        // Sync states to clients
        //for (int i = 0; i < m_playerStates.Length; i++)
        //{
        //    if (m_playerStates[i].playerObject != null)
        //        PlayerConnectsClientRpc(
        //            m_playerStates[i].clientId,
        //            i,
        //            m_playerStates[i].playerState,
        //            m_playerStates[i].playerObject.GetComponent<NetworkObject>());
        //}

    }
    [ClientRpc]
    void PlayerConnectsClientRpc(
    ulong clientId,
    int stateIndex,
    ConnectionState state,
    NetworkObjectReference player)
    {
        if (IsServer)
            return;

        //if (state != ConnectionState.disconnected)
        //{
        //    m_playerStates[stateIndex].playerState = state;
        //    m_playerStates[stateIndex].clientId = clientId;

        //    if (player.TryGet(out NetworkObject playerObject))
        //        m_playerStates[stateIndex].playerObject =
        //            playerObject.GetComponent<PlayerCharSelection>();
        //}
    }
}
