using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface G2_IState
{
    void OnEnter(G2_Bot bot);

    void OnExecute(G2_Bot bot);

    void OnExit(G2_Bot bot);

}
