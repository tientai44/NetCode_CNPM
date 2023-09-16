using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    public GameObject[] objectPrefabs; // Prefab của đối tượng bạn muốn tạo
    public float speed = 10f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    void Update()
    {
        transform.position += Vector3.left * Time.deltaTime * speed;
    }
    public void SpawnObject()
    {
        // Tạo một đối tượng trên mạng sử dụng SpawnManager

        NetworkObject spawnedObject = Instantiate(objectPrefabs[Random.Range(0, objectPrefabs.Length)], transform.position, Quaternion.identity).GetComponent<NetworkObject>();
        spawnedObject.Spawn();

        GetComponent<NetworkObject>().Despawn();
        // Đăng ký đối tượng với NetworkManager để nó có thể được đồng bộ hóa
    }
}
