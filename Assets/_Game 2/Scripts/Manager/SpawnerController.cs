using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerController : NetworkBehaviour
{
    public static SpawnerController Instance;
    [SerializeField]
    private GameObject objectPrefab;
    [SerializeField]
    private GameObject effectAttack;

    [SerializeField]
    private int maxObjectInstanceCount = 3;


    private void Awake()
    {
        Instance = this;

    }
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            //NetworkObjectPool.Instance.InitializePool();
        };
    }

    public void SpawnObjects()
    {
        if (!IsServer) return;

        for (int i = 0; i < maxObjectInstanceCount; i++)
        {
            GameObject go = Instantiate(objectPrefab,
                new Vector3(Random.Range(-10, 10), 10.0f, Random.Range(-10, 10)), Quaternion.identity);
            //GameObject go = NetworkObjectPool.Instance.GetNetworkObject(objectPrefab).gameObject;
            //go.transform.position = new Vector3(Random.Range(-10, 10), 10.0f, Random.Range(-10, 10));
            go.GetComponent<NetworkObject>().Spawn();


        }
    }
    public void SpawnAttack(Vector3 pos)
    {
        if (!IsServer) return;

        GameObject go = Instantiate(effectAttack,pos, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();
    }
}
