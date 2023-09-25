using SkeletonEditor;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class G2_Bot : NetworkBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] G2_IState currentState;
    [SerializeField] G2_PlayerController target;
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] List<G2_PlayerController> playerArounds= new List<G2_PlayerController>();
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        ChangeState(new G2_PatrolState());
    }
    public void Update()
    {
        if(currentState != null)
        {
            currentState.OnExecute(this);
        }
    }
    public void AddTarget(G2_PlayerController player)
    {
        if (playerArounds.Contains(player))
        {
            return;
        }
        playerArounds.Add(player);
    }
    public void RemoveTarget(G2_PlayerController player)
    {
        playerArounds.Remove(player);
    }
    public void Follow()
    {
        if (!playerArounds.Contains(target))
        {
            target = null;
        }
        if (target == null)
        {
            if (playerArounds.Count > 0)
            {
                target = playerArounds[Random.Range(0, playerArounds.Count)];
            }
        }
        if(target != null)
        {
            if (Vector3.Distance(navMeshAgent.destination, target.transform.position)>0.1f)
            {
                navMeshAgent.destination = target.transform.position;
            }
        }
        else
        {
            ChangeState(new G2_IdleState());
        }
    }
    public void CheckTarget()
    {
        if (playerArounds.Count > 0)
        {
            target = playerArounds[Random.Range(0, playerArounds.Count)];
            ChangeState(new G2_PatrolState());

        }
    }
    public void StopMoving()
    {
        navMeshAgent.destination = transform.position;
    }
    public void ChangeState(G2_IState newState)
    {
        if(currentState != null)
        {
            currentState.OnExit(this);
        }
        currentState = newState;
        currentState.OnEnter(this);
    }
    public void ChangeAnim(string animName)
    {
        animator.Play(animName);
    }
}
