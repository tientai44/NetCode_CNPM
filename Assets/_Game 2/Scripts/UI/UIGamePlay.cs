using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGamePlay : BasePopUp
{
    [SerializeField] TextMeshProUGUI RoomText;
    [SerializeField] TextMeshProUGUI PlayerInGameText;
    
    public void SetRoomText(string m)
    {
        RoomText.text = m;
    }
    public void SetPlayerInGame(int numPlayer)
    {
        PlayerInGameText.text = $"Player In Game: {numPlayer}/{GameManager.MaxConn}";
    }
}
