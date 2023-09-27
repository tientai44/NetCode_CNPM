using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class G2_IdleState : G2_IState
{

    public void OnEnter(G2_Bot bot)
    {
        bot.ChangeAnim("idle");
        bot.StopMoving();
    }

    public void OnExecute(G2_Bot bot)
    {
        bot.CheckTarget();
    }

    public void OnExit(G2_Bot bot)
    {
        
    }
}
