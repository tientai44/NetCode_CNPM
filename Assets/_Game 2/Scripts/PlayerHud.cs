using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : NetworkBehaviour
{
    [FoldoutGroup("NetworkVariable"), SerializeField]
    private NetworkVariable<NetworkString> playersName = new NetworkVariable<NetworkString>();

    [FoldoutGroup("UI Component"), SerializeField]
    private G2_PlayerController player;
    [FoldoutGroup("UI Component"), SerializeField] TextMeshProUGUI nameText;
    [FoldoutGroup("UI Component"), SerializeField] TextMeshProUGUI levelText;
    [FoldoutGroup("UI Component"), SerializeField] Image hpImgFill;
    private bool overlaySet = false;
    

    private void Awake()
    {
        player = GetComponent<G2_PlayerController>();
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            playersName.Value = $"Player {OwnerClientId}";
        }
    }
    public void SetOverlay()
    {
        nameText.text = playersName.Value;
    }
    public void SetLevel(int level)
    {
        levelText.text = level.ToString();
        UIManager.Instance.UIGamePlay.loggerPlayer.AddPlayerInTop(playersName.Value, level);
    }
    public void SetHP(float currentHP,float maxHP)
    {     
        float val = currentHP / maxHP;
        hpImgFill.fillAmount =  val<0?0:val;
    }
    private void Update()
    {
        if(!overlaySet && !string.IsNullOrEmpty(playersName.Value)){
            SetOverlay();
            overlaySet = true;
        }
    }
   

}

