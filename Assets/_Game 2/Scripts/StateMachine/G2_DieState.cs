using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class G2_DieState : G2_IState
{
    

    public void OnEnter(G2_Bot bot)
    {
        bot.OnDeath();
    }

    public void OnExecute(G2_Bot bot)
    {
    }

    public void OnExit(G2_Bot bot)
    {
    }
}
