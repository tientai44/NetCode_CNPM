using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerHud : NetworkBehaviour
{
    [SerializeField]
    private NetworkVariable<NetworkString> playersName = new NetworkVariable<NetworkString>();

    private bool overlaySet = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            playersName.Value = $"Player {OwnerClientId}";
        }
    }
    public void SetOverlay()
    {
        var localPlayerOverlay = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        localPlayerOverlay.text = playersName.Value;
    }
    private void Update()
    {
        if(!overlaySet && !string.IsNullOrEmpty(playersName.Value)){
            SetOverlay();
            overlaySet = true;
        }
    }
   

}

