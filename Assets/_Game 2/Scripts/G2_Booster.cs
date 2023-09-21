using SkeletonEditor;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class G2_Booster : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            G2_PlayerController player = other.GetComponent<G2_PlayerController>();
            player.networkPlayerHealth.Value += 200;
            SpawnerController.Instance.GetBackObject(NetworkObject);

        }
    }

   
}
