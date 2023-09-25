using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G2_PatrolState : G2_IState
{
    public void OnEnter(G2_Bot bot)
    {
        bot.ChangeAnim("walk");
    }

    public void OnExecute(G2_Bot bot)
    {
        bot.Follow();
    }

    public void OnExit(G2_Bot bot)
    {
        
    }
}
