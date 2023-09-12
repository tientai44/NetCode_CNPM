using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Button startHostButton;
    [SerializeField] Button startSeverButton;
    [SerializeField] Button startClientButton;
    [SerializeField] TextMeshProUGUI playersInGameTxt;

    private void Awake()
    {
        Cursor.visible = true;
    }

    private void Start()
    {
        startSeverButton.onClick.AddListener(OnClickStartClientBtn);
        startClientButton.onClick.AddListener(OnClickStartClientBtn);
        startHostButton.onClick.AddListener(OnClickStartHostBtn);
    }
    private void Update()
    {
        playersInGameTxt.text = $"Players In Game: {PlayersManager.Instance.PlayersInGame}";
    }
    void OnClickStartHostBtn()
    {
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
    void OnClickStartClientBtn()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            LoggerDebug.Instance.LogInfo("Client started...");
        }
        else
        {
            LoggerDebug.Instance.LogInfo("Client could not be started...");
        }
    }
}
