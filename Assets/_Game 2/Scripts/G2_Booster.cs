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
            SpawnerController.Instance.GetBackObject(NetworkObject);
        }
    }
}
