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
    [SerializeField] GameObject UI_Loading;
    [SerializeField] UINotify UI_Notify;
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
        playersInGameTxt.text = $"Players In Game: {PlayersManager.Instance.PlayersInGame}/{ maxConn}";



    }
    async void OnClickStartHostBtn()
    {
        if (hasServerStarted)
        {
            return;
        }

        if (RelayManager.Instance.IsRelayEnabled)
        {
            UI_Loading.SetActive(true);
            await RelayManager.Instance.HostGame(maxConn);
        }
        else {
            Notify("Relay Is Not Ready");
            return; 
        }

        if (NetworkManager.Singleton.StartHost())
        {
            LoggerDebug.Instance.LogInfo("Host started...");
            UI_MainMenu.SetActive(false);
        }
        else
        {
            Notify("Host could not be started...");

            LoggerDebug.Instance.LogInfo("Host could not be started...");
        }
        UI_Loading.SetActive(false);
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
        {
            UI_Loading.SetActive(true);
            try
            {
                await RelayManager.Instance.JoinGame(inputField.text);
            }
            catch
            {
                Notify("Room not exist");
                UI_Loading.SetActive(false);
                return;
            }
        }
        else
        {
            Notify("Enter the Room ID");
            return;
        }
        if (NetworkManager.Singleton.StartClient())
        {
            LoggerDebug.Instance.LogInfo("Client started...");
            UI_MainMenu.SetActive(false);
        }
        else
        {
            LoggerDebug.Instance.LogInfo("Client could not be started...");
        }
        UI_Loading.SetActive(false);

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
    public void Notify(string message)
    {
        UI_Notify.Notify(message);
    }
}
