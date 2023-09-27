using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class G2_DieState : G2_IState
{
    float timer;

    public void OnEnter(G2_Bot bot)
    {
        timer = 3;
        bot.OnDeath();
    }

    public void OnExecute(G2_Bot bot)
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            SpawnerController.Instance.ReturnMonster(bot.GetComponent<NetworkObject>());
        }
    }

    public void OnExit(G2_Bot bot)
    {
    }
}
