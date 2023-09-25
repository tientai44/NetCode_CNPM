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
    private GameObject monsterPrefab;
    [SerializeField]
    private int maxObjectInstanceCount = 10;


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
            //GameObject go = Instantiate(objectPrefab,
            //    new Vector3(Random.Range(-10, 10), 10.0f, Random.Range(-10, 10)), Quaternion.identity);
            List<G2_BoosterType> lst = new List<G2_BoosterType> { G2_BoosterType.BuffHp, G2_BoosterType.BuffDame, G2_BoosterType.LevelUp, G2_BoosterType.SpeedUp };

            NetworkObject networkObject = NetworkObjectPool.Singleton.GetNetworkObject(objectPrefab, new Vector3(Random.Range(-100, 100), 10.0f, Random.Range(-100, 100)), Quaternion.identity);
            networkObject.Spawn(true);
            networkObject.GetComponent<G2_Booster>().SetType(lst[Random.Range(0, lst.Count)]);

        }
    }
    public void SpawnMonsters()
    {
        if (!IsServer) return;




        NetworkObject networkObject = NetworkObjectPool.Singleton.GetNetworkObject(monsterPrefab, new Vector3(Random.Range(-10, 10),0, Random.Range(-10, 10)), Quaternion.identity);
        networkObject.Spawn(true);

    }
    public void GetBackObject(NetworkObject networkObject)
    {
        if (!IsServer) return;
        NetworkObjectPool.Singleton.ReturnNetworkObject(networkObject, objectPrefab);
        networkObject.Despawn(false);


    }
    public void SpawnAttackHitEffect(Vector3 pos)
    {
        if (!IsServer) return;

        NetworkObject networkObject = NetworkObjectPool.Singleton.GetNetworkObject(effectAttack, pos, Quaternion.identity);
        networkObject.Spawn(true);
        networkObject.GetComponent<ParticleSystem>().Play();
        StartCoroutine(IEReturnAttackHitEffect(networkObject, 1f));
    }
    public IEnumerator IEReturnAttackHitEffect(NetworkObject networkObject, float time)
    {
        yield return new WaitForSeconds(time);
        NetworkObjectPool.Singleton.ReturnNetworkObject(networkObject, effectAttack);
        networkObject.Despawn(false);
    }
}
