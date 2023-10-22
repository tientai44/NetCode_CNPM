using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum G2_BoosterType
{
    BuffHp,
    BuffDame,
    LevelUp,
    SpeedUp
}
public class G2_Booster : NetworkBehaviour
{
    private G2_BoosterType type;

    [ClientRpc]
    public void SetTypeClientRpc(G2_BoosterType _type)
    {
        type = _type;
        switch (_type)
        {
            case G2_BoosterType.BuffHp:
                GetComponentInChildren<MeshRenderer>().material.color = Color.green;
                break;
            case G2_BoosterType.BuffDame:
                GetComponentInChildren<MeshRenderer>().material.color = Color.red;
                break;
            case G2_BoosterType.LevelUp:
                GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
                break;
            case G2_BoosterType.SpeedUp:
                GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
                break;
        }
        
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
