using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class G2_AttackState : G2_IState
{
    float timer = 3;
   
    public void OnEnter(G2_Bot bot)
    {
        timer = 3;
        bot.StopMoving();
        bot.Attack();
    }

    public void OnExecute(G2_Bot bot)
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            bot.ChangeState(new G2_IdleState());
        }
    }

    public void OnExit(G2_Bot bot)
    {
    }
}
