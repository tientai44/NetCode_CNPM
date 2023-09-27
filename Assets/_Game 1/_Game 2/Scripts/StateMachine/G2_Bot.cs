using SkeletonEditor;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class G2_Bot : NetworkBehaviour
{
    public NetworkVariable<int> ID;
    public NetworkVariable<bool> networkDeath = new NetworkVariable<bool>(false);
    [SerializeField] Animator animator;
    [SerializeField] G2_IState currentState;
    [SerializeField] G2_PlayerController target;
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] List<G2_PlayerController> playerArounds= new List<G2_PlayerController>();
    Collider _collider;
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<Collider>();
    }

    public void OnInit()
    {
        ChangeState(new G2_PatrolState());
        _collider.enabled = true;
        networkDeath.Value = false;

    }
    public void Update()
    {
        if(networkDeath.Value == true)
        {
            if (currentState is not G2_DieState)
            {
                ChangeState(new G2_DieState());
            }
        }
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
        for(int i = 0; i < playerArounds.Count; i++)
        {
            if (playerArounds[i] == null)
            {
                playerArounds.RemoveAt(i);
            }
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
            if (Vector3.Distance(target.transform.position, transform.position) < 3f)
            {
                ChangeState(new G2_AttackState());
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
            if(target==null)
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
    public void Attack()
    {
        ChangeAnim("attack");
        StartCoroutine(IEAttack());
    }
    IEnumerator IEAttack()
    {
        yield return new WaitForSeconds(1);
        if (target != null)
        {
            if (Vector3.Distance(target.transform.position, transform.position) < 5f)
            {
                target.IncreaseHealth(-100);
            }
        }
    }
    public void ChangeAnim(string animName)
    {
        animator.Play(animName);
    }
    public void OnDeath()
    {
        StopMoving();
        ChangeAnim("die");
        _collider.enabled = false;

    }
}
