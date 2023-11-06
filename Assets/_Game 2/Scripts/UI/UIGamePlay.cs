using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePlay : BasePopUp
{
    [SerializeField] TextMeshProUGUI RoomText;
    [SerializeField] TextMeshProUGUI PlayerInGameText;
    [SerializeField] Button exitRoom;
    public LoggerPlayer loggerPlayer;
    void Awake()
    {
        exitRoom.onClick.AddListener(FunExitRoom);
    }
    private void FunExitRoom()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            GameManager.Instance.hasServerStarted = false;
        }

        NetworkManager.Singleton.Shutdown();
        UIManager.Instance.UI_MainMenu.Show();
    }
    public void SetRoomText(string m)
    {
        RoomText.text = m;
    }
    public void SetPlayerInGame(int numPlayer)
    {
        PlayerInGameText.text = $"Player In Game: {numPlayer}/{GameManager.MaxConn}";
    }
}
