using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] Button executePhysicsButton;
    [SerializeField] Button spawnMonsterButton;
    public UI_MainMenu UI_MainMenu;
    public UI_Loading UI_Loading;
    public UI_ChooseModel UIChooseModel;
    [SerializeField] UINotify UI_Notify;
    public UIGamePlay UIGamePlay;
    private void Awake()
    {
        Instance = this;
        Cursor.visible = true;
    }

    private void Start()
    {

        executePhysicsButton.onClick.AddListener(OnClickExecutePhysics);
        spawnMonsterButton.onClick.AddListener(OnClickSpawnMonster);
      
    }

 
    void OnClickExecutePhysics()
    {
        if (!GameManager.Instance.hasServerStarted)
        {
            LoggerDebug.Instance.LogWarning("Server has not started...");
            return;
        }
        SpawnerController.Instance.SpawnObjects();
    }
    void OnClickSpawnMonster()
    {
        if (!GameManager.Instance.hasServerStarted)
        {
            LoggerDebug.Instance.LogWarning("Server has not started...");
            return;
        }
        SpawnerController.Instance.SpawnMonsters();
    }
    public void Notify(string message)
    {
        UI_Notify.Notify(message);
    }
}
