using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum G2_BoosterType
{
    None,
    BuffHp,
    BuffDame,
    LevelUp,
    SpeedUp
}
public class G2_Booster : NetworkBehaviour
{
    private G2_BoosterType type=G2_BoosterType.None;
    private  NetworkVariable<G2_BoosterType> nw_Type = new NetworkVariable<G2_BoosterType>(G2_BoosterType.None);
    [SerializeField] private GameObject[] models;

    private void FixedUpdate()
    {
        if(type != nw_Type.Value)
        {
            if(type != G2_BoosterType.None)
                models[(int)type-1].SetActive(false);
            type = nw_Type.Value;
            models[(int)type-1].SetActive(true);
        }
    }
    [ClientRpc]
    public void SetTypeClientRpc(G2_BoosterType _type)
    {
        models[(int)type - 1].SetActive(false);
        type = _type;
        models[(int)type - 1].SetActive(true);
    }
    
    public void SetType(G2_BoosterType _type)
    {
        nw_Type.Value = _type;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (IsServer)
            {
                G2_PlayerController player = other.GetComponent<G2_PlayerController>();
                BuffPlayer(player);
                SpawnerController.Instance.GetBackObject(NetworkObject);
            }
        }
    }

    public void BuffPlayer(G2_PlayerController player)
    {
        switch (type)
        {
            case G2_BoosterType.BuffHp:
                player.IncreaseHealth(200);
                break;
            case G2_BoosterType.BuffDame:
                player.IncreaseDamage(20);
                break;
            case G2_BoosterType.LevelUp:
                player.LevelUp();
                break;
            case G2_BoosterType.SpeedUp:
                player.SpeedUp(1);
                break;
        }
    }
   
}
