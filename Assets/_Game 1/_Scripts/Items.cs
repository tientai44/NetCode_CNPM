using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Items : NetworkBehaviour
{
    public int score = 10;
    public enum item
    {
        coin,

    }
    public item items;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player")
        {
            return;
        }
        else
        {
            collision.GetComponent<PlayerController>().score += score;
            if (IsServer)
            {
                gameObject.GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}
