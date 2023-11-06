using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LoggerPlayer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI areaText;

    [SerializeField]
    private bool enableLogger = false;

    [SerializeField]
    private int maxLines = 5;
    private List<TopPlayerInGame> topPlayer;
    private List<string> txtText;
    void Start()
    {
        topPlayer = new List<TopPlayerInGame>();
        txtText = new List<string>();
        for (int i = 0; i < maxLines; i++)
        {
            string infor = "Player" + " " + "0";
            txtText.Add(infor);
        }
    }

    private void Update()
    {
        UpdateTopPlayer();
    }

    public void AddPlayerInTop(string InforPlayer, int point)
    {
        TopPlayerInGame topPlayerInGame = new TopPlayerInGame();
        topPlayerInGame.AddTop(InforPlayer, point);
        if(topPlayer.Count < 5)
        {
            topPlayer.Add(topPlayerInGame);
            topPlayer = topPlayer.OrderByDescending(x => x.point).ToList();
        }
        else
        {
            foreach(var item in topPlayer)
            {
                if(item.point < topPlayerInGame.point)
                {
                    item.AddTop(topPlayerInGame.namePlayer, topPlayerInGame.point);
                    break;
                }
            }
        }
    }

    private void UpdateTopPlayer()
    {
        if (topPlayer.Count < 0) return;
        areaText.text = "";
        for (int i = 0;i < topPlayer.Count;i++)
        {
            string infor = topPlayer[i].namePlayer + " " + topPlayer[i].point.ToString();
            txtText[i] = infor;
            areaText.text += txtText[i];
        }
    }
}

[Serializable]
public class TopPlayerInGame
{
    public string namePlayer = "";
    public int point = 0;

    public void AddTop(string name, int value)
    {
        namePlayer = name;
        point = value;
    }
}
