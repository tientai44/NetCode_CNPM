using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class PlayerCharSelection : NetworkBehaviour
{
    public int CharSelected => m_charSelected.Value;

    private const int k_noCharacterSelectedValue = -1;

    [SerializeField]
    private NetworkVariable<int> m_charSelected =
        new NetworkVariable<int>(k_noCharacterSelectedValue);

    [SerializeField]
    private NetworkVariable<int> m_playerId =
        new NetworkVariable<int>(k_noCharacterSelectedValue);

    [SerializeField]
    private AudioClip _changedCharacterClip;

    private void Start()
    {
        if (IsServer)
        {
            m_playerId.Value = CharacterSelectionManager.Instance.GetPlayerId(OwnerClientId);
        }
        else if (!IsOwner)
        {
            CharacterSelectionManager.Instance.SetPlayebleChar(
            m_playerId.Value,
                m_charSelected.Value,
                IsOwner);
        }

        // Assign the name of the object base on the player id on every instance
        gameObject.name = $"Player{m_playerId.Value + 1}";
    }

    private void OnEnable()
    {
        m_playerId.OnValueChanged += OnPlayerIdSet;
        m_charSelected.OnValueChanged += OnCharacterChanged;
        OnButtonPress.a_OnButtonPress += OnUIButtonPress;
    }

    private void OnUIButtonPress(ButtonActions buttonAction)
    {
        if (!IsOwner)
            return;

        switch (buttonAction)
        {
            case ButtonActions.lobby_ready:
                CharacterSelectionManager.Instance.SetPlayerReadyUIButtons(
                    true,
                    m_charSelected.Value);

                ReadyServerRpc();
                break;

            case ButtonActions.lobby_not_ready:
                //CharacterSelectionManager.Instance.SetPlayerReadyUIButtons(
                //    false,
                //    m_charSelected.Value);

                //NotReadyServerRpc();
                break;
        }
    }

    [ServerRpc]
    private void ReadyServerRpc()
    {
        CharacterSelectionManager.Instance.PlayerReady(
            OwnerClientId,
            m_playerId.Value,
            m_charSelected.Value);
    }
    private void OnPlayerIdSet(int oldValue, int newValue)
    {
        CharacterSelectionManager.Instance.SetPlayebleChar(newValue, newValue, IsOwner);

        if (IsServer)
            m_charSelected.Value = newValue;
    }
    private void OnCharacterChanged(int oldValue, int newValue)
    {
        // If I am not the owner, update the character selection UI
        if (!IsOwner)
            CharacterSelectionManager.Instance.SetCharacterUI(m_playerId.Value, newValue);
    }
}
