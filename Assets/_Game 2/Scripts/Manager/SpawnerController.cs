using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerController : NetworkBehaviour
{
    public static SpawnerController Instance;
    [FoldoutGroup("Prefab"),SerializeField]
    private GameObject objectPrefab;
    [FoldoutGroup("effect Attack"), SerializeField]
    private GameObject effectAttack;
    [FoldoutGroup("monster Prefab"), SerializeField]
    private GameObject monsterPrefab;
    [FoldoutGroup("maxObjectInstance"),SerializeField]
    private int maxObjectInstanceCount = 10;
    [FoldoutGroup("List Bot"), SerializeField]
    List<G2_Bot> bots= new List<G2_Bot>();
    int numMonster=0;
    private static float timeDuration = 15;
    private void Awake()
    {
        Instance = this;
       
        
    }
    void Start()
    {
        NetworkManager.OnServerStarted += () =>
        {
            bots.Clear();
            StartCoroutine(IESpawnMonster());
            StartCoroutine(IESpawnBootster());
        };
    }
    IEnumerator IESpawnMonster()
    {
        yield return new WaitForSeconds(timeDuration);
        if (!GameManager.Instance.hasServerStarted)
        {
            yield break;
        }
        if (numMonster < 5)
        {
            SpawnMonsters();
        }

        StartCoroutine(IESpawnMonster());
    }
    IEnumerator IESpawnBootster()
    {
        yield return new WaitForSeconds(timeDuration);
        if (!GameManager.Instance.hasServerStarted)
        {
            yield break;
        }
        SpawnObjects();
        StartCoroutine(IESpawnBootster());
    }
    public void SpawnObjects()
    {
        if (!IsServer) return;

        for (int i = 0; i < maxObjectInstanceCount; i++)
        {
            //GameObject go = Instantiate(objectPrefab,
            //    new Vector3(Random.Range(-10, 10), 10.0f, Random.Range(-10, 10)), Quaternion.identity);
            List<G2_BoosterType> lst = new List<G2_BoosterType> { G2_BoosterType.BuffHp, G2_BoosterType.BuffDame, G2_BoosterType.LevelUp, G2_BoosterType.SpeedUp };

            NetworkObject networkObject = NetworkObjectPool.Singleton.GetNetworkObject(objectPrefab, new Vector3(Random.Range(-20, 20), 10.0f, Random.Range(-20, 20)), Quaternion.identity);
            networkObject.Spawn(true);
            //networkObject.GetComponent<G2_Booster>().SetTypeClientRpc(lst[Random.Range(0, lst.Count)]);
            networkObject.GetComponent<G2_Booster>().SetType(lst[Random.Range(0, lst.Count)]);

        }
    }
    public void SpawnMonsters()
    {
        if (!IsServer) return;
        NetworkObject networkObject = NetworkObjectPool.Singleton.GetNetworkObject(monsterPrefab, new Vector3(Random.Range(-10, 10),0, Random.Range(-10, 10)), Quaternion.identity);
        networkObject.Spawn(true);
        G2_Bot bot = networkObject.GetComponent<G2_Bot>();
        bot.networkDeath.Value=false;
        if (!bots.Contains(bot))
        {
            bot.ID.Value = bots.Count;
            bots.Add(bot);
        }
        numMonster += 1;

    }
    public void ReturnMonster(NetworkObject networkObject)
    {
        if (!IsServer) return;
        NetworkObjectPool.Singleton.ReturnNetworkObject(networkObject, monsterPrefab);
        networkObject.Despawn(false);
        numMonster -= 1;
    }
    public G2_Bot GetMonster(int id)
    {
        return bots[id];
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
