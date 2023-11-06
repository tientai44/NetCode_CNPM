using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : BasePopUp
{
    [SerializeField] Button startHostButton;
    [SerializeField] Button startSeverButton;
    [SerializeField] Button startClientButton;
    [SerializeField] Button selectCharacterButton;
    [SerializeField] TMP_InputField inputField;
    private void Awake()
    {
        startSeverButton.onClick.AddListener(OnClickStartServerBtn);
        startClientButton.onClick.AddListener(OnClickStartClientBtn);
        startHostButton.onClick.AddListener(OnClickStartHostBtn);
        selectCharacterButton.onClick.AddListener(SelectCharacterButton);
    }
    async void OnClickStartHostBtn()
    {
        if (GameManager.Instance.hasServerStarted)
        {
            UIManager.Instance.Notify("Server Is Started");
            return;
        }

        if (RelayManager.Instance.IsRelayEnabled)
        {
            UIManager.Instance.UI_Loading.Show();
            await RelayManager.Instance.HostGame(GameManager.MaxConn);
        }
        else
        {
            UIManager.Instance.Notify("Relay Is Not Ready");
            return;
        }

        if (NetworkManager.Singleton.StartHost())
        {
            LoggerDebug.Instance.LogInfo("Host started...");
            UIManager.Instance.UI_MainMenu.Hide();
        }
        else
        {
            UIManager.Instance.Notify("Host could not be started...");

            LoggerDebug.Instance.LogInfo("Host could not be started...");
        }
        UIManager.Instance.UI_Loading.Hide();
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
            UIManager.Instance.UI_Loading.Show();
            try
            {
                await RelayManager.Instance.JoinGame(inputField.text);
            }
            catch
            {
                UIManager.Instance.Notify("Room not exist");
                UIManager.Instance.UI_Loading.Hide();
                return;
            }
        }
        else
        {
            UIManager.Instance.Notify("Enter the Room ID");
            return;
        }
        if (NetworkManager.Singleton.StartClient())
        {
            LoggerDebug.Instance.LogInfo("Client started...");
            UIManager.Instance.UI_MainMenu.Hide();

        }
        else
        {
            LoggerDebug.Instance.LogInfo("Client could not be started...");
        }
        UIManager.Instance.UI_Loading.Hide();

    }
    private void SelectCharacterButton()
    {
        UIManager.Instance.UIChooseModel.Show();
    }

   
}
