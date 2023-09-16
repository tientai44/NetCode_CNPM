using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public float speed = 10f;

    public float timmer = 0f;
    public float maxTimmer = 5f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    void Update()
    {
        if (timmer >= maxTimmer)
        {
            if (IsServer)
                GetComponent<NetworkObject>().Despawn();
        }
        else
            timmer += Time.deltaTime;
        transform.position += new Vector3(1, 0, 0) * Time.deltaTime * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "enemy")
        {
            collision.GetComponent<Enemy>().SpawnObject();
        }
    }
}
