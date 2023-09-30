using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] Button startHostButton;
    [SerializeField] Button startSeverButton;
    [SerializeField] Button startClientButton;
    [SerializeField] Button executePhysicsButton;
    [SerializeField] Button spawnMonsterButton;
    [SerializeField] TextMeshProUGUI playersInGameTxt;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] GameObject UI_GamePlay;
    [SerializeField] GameObject UI_MainMenu;
    public TextMeshProUGUI RoomText;
    private bool hasServerStarted;
    private static int maxConn = 10;
    private void Awake()
    {
        Instance = this;
        Cursor.visible = true;
    }

    private void Start()
    {
        startSeverButton.onClick.AddListener(OnClickStartServerBtn);
        startClientButton.onClick.AddListener(OnClickStartClientBtn);
        startHostButton.onClick.AddListener(OnClickStartHostBtn);
        executePhysicsButton.onClick.AddListener(OnClickExecutePhysics);
        spawnMonsterButton.onClick.AddListener(OnClickSpawnMonster);
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            hasServerStarted = true;
        };
    }
    private void Update()
    {
        playersInGameTxt.text = $"Players In Game: {PlayersManager.Instance.PlayersInGame}";
    }
    async void OnClickStartHostBtn()
    {
        if (RelayManager.Instance.IsRelayEnabled)
            await RelayManager.Instance.HostGame(maxConn);
        else return;
        if (NetworkManager.Singleton.StartHost())
        {
            LoggerDebug.Instance.LogInfo("Host started...");
        }
        else
        {
            LoggerDebug.Instance.LogInfo("Host could not be started...");
        }

    }
    void OnClickStartServerBtn()
    {
        
        if (NetworkManager.Singleton.StartServer())
        {
            LoggerDebug.Instance.LogInfo("Server started...");
        }
        else
        {
            LoggerDebug.Instance.LogInfo("Server could not be started...");
        }
    }
    async void OnClickStartClientBtn()
    {
        if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(inputField.text))
            await RelayManager.Instance.JoinGame(inputField.text);
        else return;
        if (NetworkManager.Singleton.StartClient())
        {
            LoggerDebug.Instance.LogInfo("Client started...");
        }
        else
        {
            LoggerDebug.Instance.LogInfo("Client could not be started...");
        }


    }
    void OnClickExecutePhysics()
    {
        if (!hasServerStarted)
        {
            LoggerDebug.Instance.LogWarning("Server has not started...");
            return;
        }
        SpawnerController.Instance.SpawnObjects();
    }
    void OnClickSpawnMonster()
    {
        if (!hasServerStarted)
        {
            LoggerDebug.Instance.LogWarning("Server has not started...");
            return;
        }
        SpawnerController.Instance.SpawnMonsters();
    }
}
