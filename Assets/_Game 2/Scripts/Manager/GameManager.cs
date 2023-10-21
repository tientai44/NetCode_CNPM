using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool hasServerStarted;

    private static int maxConn = 10;
    public static int MaxConn { get { return maxConn; } }

    private void Awake()
    {
        Instance = this;
       
    }
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            hasServerStarted = true;
        };
    }

}
