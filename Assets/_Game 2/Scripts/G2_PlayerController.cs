﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

namespace SkeletonEditor
{
    public enum G2_PlayerState
    {
        Idle,
        Walk,
        Run,
        Attack,
        ReverseWalk,
        OnHit,
        Die
    }
    public class G2_PlayerController : NetworkBehaviour
    {
        [SerializeField]
        private float walkSpeed = 3.5f;

        [SerializeField]
        private float runSpeedOffset = 3.5f;

        [SerializeField]
        private float rotationSpeed = 3.5f;

        [SerializeField]
        private Vector2 defaultInitialPositionOnPlane = new Vector2(-4, 4);

        [SerializeField]
        private NetworkVariable<Vector3> networkPositionDirection = new NetworkVariable<Vector3>();

        [SerializeField]
        private NetworkVariable<Vector3> networkRotationDirection = new NetworkVariable<Vector3>();

        [SerializeField]
        private NetworkVariable<G2_PlayerState> networkPlayerState = new NetworkVariable<G2_PlayerState>();
        [SerializeField]
        private NetworkVariable<float> networkSpeed = new NetworkVariable<float>();
        [SerializeField]
        private NetworkVariable<float> networkPlayerHealth = new NetworkVariable<float>(1000);
        [SerializeField]
        private NetworkVariable<float> networkMaxPlayerHealth = new NetworkVariable<float>(1000);
        [SerializeField]
        private NetworkVariable<int> networkLevel = new NetworkVariable<int>(1);
        [SerializeField]
        private float oldPlayerHealth;
        private float oldMaxPlayerHealth;
        private int oldLevel;
        private PlayerHud playerHud;
        private CharacterController characterController;

        // client caches positions
        private Vector3 oldInputPosition = Vector3.zero;
        private Vector3 oldInputRotation = Vector3.zero;
        [SerializeField] private G2_PlayerState oldPlayerState = G2_PlayerState.Idle;
        private float speed;
        [SerializeField]
        private Animator animator;

        private bool isAttacking = false;
        private float attackRange = 10f;
        private float heightUpAttack = 4;
        private float timeDelayCheckHit = 0.5f;
        private bool isCheckHit = true;
        private float damage = 300;
        [SerializeField] private GameObject skillEffect;

        [SerializeField] private ParticleSystem effectZone;
        [SerializeField] private ParticleSystem effectAttack;
        [SerializeField] private ParticleSystem dieEffect;
        [SerializeField] private ParticleSystem buffHpEffect;
        [SerializeField] private ParticleSystem levelUpEffect;
        Coroutine coroutineAttack;
        Coroutine coroutineCheckHit;
        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            playerHud = GetComponent<PlayerHud>();
        }
        [ServerRpc]
        void OnInitServerRpc()
        {
            UpdatePlayerStateServerRpc(G2_PlayerState.Idle);
            networkLevel.Value = 1;
            networkPlayerHealth.Value = networkMaxPlayerHealth.Value;
            transform.position = new Vector3(Random.Range(defaultInitialPositionOnPlane.x, defaultInitialPositionOnPlane.y), 0,
                       Random.Range(defaultInitialPositionOnPlane.x, defaultInitialPositionOnPlane.y));

        }
        void Start()
        {
            if (IsClient && IsOwner)
            {
                
                G2_CameraFollow.Instance.SetTarget(transform);
                effectZone.Play();
            }
        }

        void Update()
        {
            if (IsClient && IsOwner)
            {
                ClientInput();
            }
            ClientMoveAndRotate();
            ClientVisuals();
            //CheckAlive();
        }
        private void FixedUpdate()
        {
            if (IsClient && IsOwner)
            {
                switch (networkPlayerState.Value)
                {
                    case G2_PlayerState.Attack:
                        CheckHit();
                        break;
                    default:
                        break;

                }
             
                
            }
        }
        private void ClientMoveAndRotate()
        {
            if (networkPositionDirection.Value != Vector3.zero)
            {
                characterController.SimpleMove(networkPositionDirection.Value);
            }
            if (networkRotationDirection.Value != Vector3.zero)
            {
                transform.Rotate(networkRotationDirection.Value, Space.World);
            }
        }

        private void ClientVisuals()
        {

            if (oldPlayerState != networkPlayerState.Value)
            {
                
                oldPlayerState = networkPlayerState.Value;
                animator.Play(networkPlayerState.Value.ToString());

                switch (oldPlayerState)
                {
                    case G2_PlayerState.Die:
                        DOVirtual.DelayedCall(1f, () =>
                        {
                            dieEffect.gameObject.SetActive(true);
                            dieEffect.Play();
                        });
                        break;
                    default:
                        dieEffect.gameObject.SetActive(false);
                        break;
                }

            }
            if (oldPlayerHealth != networkPlayerHealth.Value || oldMaxPlayerHealth !=networkMaxPlayerHealth.Value)
            {
                if(oldPlayerHealth < networkPlayerHealth.Value)
                {
                    buffHpEffect.Play();
                }
                oldPlayerHealth = networkPlayerHealth.Value;
                oldMaxPlayerHealth = networkMaxPlayerHealth.Value;
                playerHud.SetHP(oldPlayerHealth, oldMaxPlayerHealth);
            }
            if(oldLevel != networkLevel.Value)
            {
                if (oldLevel < networkLevel.Value)
                {
                    levelUpEffect.Play();

                }
                oldLevel = networkLevel.Value;
                playerHud.SetLevel(oldLevel);
            }
        }
        private void CheckAlive()
        {
            if (networkPlayerHealth.Value <= 0)
            {
                UpdatePlayerStateServerRpc(G2_PlayerState.Die);
                
            }
            
        }
        private void ClientInput()
        {
            if (oldPlayerState == G2_PlayerState.Die)
            {
                return;
            }
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            speed = Mathf.Abs(v) * walkSpeed;
            UpdateCurrentSpeedServerRpc(speed);
            // left & right rotation
            Vector3 inputRotation = new Vector3(0, h, 0);

            // forward & backward direction
            Vector3 direction = transform.TransformDirection(Vector3.forward);
            float forwardInput = v;
            Vector3 inputPosition = direction * forwardInput;

            // change animation states
            if (networkPlayerHealth.Value <= 0)
            {
                UpdatePlayerStateServerRpc(G2_PlayerState.Die);
                //ExitServerRpc(OwnerClientId);
                
                Invoke(nameof(OnInitServerRpc), 4);
                return;
            }
            if (ActivePunchActionKey() && forwardInput == 0 && !isAttacking && oldPlayerState != G2_PlayerState.Attack)
            {
                UpdatePlayerStateServerRpc(G2_PlayerState.Attack);
                isAttacking = true;
                Invoke(nameof(ResetAttack), 2f);
                Invoke(nameof(SetCheckHit), timeDelayCheckHit);
                return;
            }
            if (forwardInput == 0 && !isAttacking)
                UpdatePlayerStateServerRpc(G2_PlayerState.Idle);
            else if (!ActiveRunningActionKey() && forwardInput > 0 && forwardInput <= 1)
                UpdatePlayerStateServerRpc(G2_PlayerState.Walk);
            else if (ActiveRunningActionKey() && forwardInput > 0 && forwardInput <= 1)
            {
                inputPosition = direction * runSpeedOffset;
                UpdatePlayerStateServerRpc(G2_PlayerState.Run);
            }
            else if (forwardInput < 0)
                UpdatePlayerStateServerRpc(G2_PlayerState.ReverseWalk);

            // let server know about position and rotation client changes
            if (oldInputPosition != inputPosition ||
                oldInputRotation != inputRotation)
            {
                CancelInvoke(nameof(SetCheckHit));
                CancelInvoke(nameof(ResetAttack));
                isAttacking = false;
                oldInputPosition = inputPosition;
                oldInputRotation = inputRotation;
                UpdateClientPositionAndRotationServerRpc(inputPosition * walkSpeed, inputRotation * rotationSpeed);
            }
        }
        private static bool ActivePunchActionKey()
        {
            return Input.GetKey(KeyCode.Z);
        }
        private static bool ActiveRunningActionKey()
        {
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }
        public void ResetAttack()
        {
            isAttacking = false;
        }
        public void SetCheckHit()
        {
            isCheckHit = false;
        }
        [ServerRpc]
        public void UpdateClientPositionAndRotationServerRpc(Vector3 newPosition, Vector3 newRotation)
        {
            networkPositionDirection.Value = newPosition;
            networkRotationDirection.Value = newRotation;
        }

        [ServerRpc]
        public void UpdatePlayerStateServerRpc(G2_PlayerState state)
        {
            networkPlayerState.Value = state;
        }
        [ServerRpc]
        public void UpdateCurrentSpeedServerRpc(float speed)
        {
            networkSpeed.Value = speed;
        }
        public void LevelUp(int val=1)
        {
            if (oldPlayerState == G2_PlayerState.Die)
            {
                return;
            }
            networkLevel.Value += val;
        }
        private void CheckHit()
        {
            if (isCheckHit)
            {
                return;
            }
            isCheckHit = true;
            Debug.Log("hit");
            RaycastHit hit;
            SkillEffectServerRpc(transform.position + Vector3.up * heightUpAttack + transform.forward*2);
            int layerMask = LayerMask.GetMask("Player");

            if (Physics.SphereCast(transform.position + Vector3.up * heightUpAttack,1f, transform.forward, out hit, attackRange, layerMask))
            {
                Debug.DrawRay(transform.position + Vector3.up * heightUpAttack, transform.forward * attackRange, Color.yellow);

                var playerHit = hit.transform.GetComponent<NetworkObject>();
                if (playerHit != null)
                {
                    UpdateHealthServerRpc(damage, playerHit.OwnerClientId,OwnerClientId);
                    SpawnAttackServerRpc(hit.transform.position + Vector3.up * heightUpAttack);
                    Debug.Log("Attack a enemy");
                }
            }
            else
            {
                Debug.DrawRay(transform.position + Vector3.up * heightUpAttack, transform.forward * attackRange, Color.red);
            }
        }
        [ServerRpc]
        public void SkillEffectServerRpc(Vector3 pos)
        {
            NetworkObject networkObject = NetworkObjectPool.Singleton.GetNetworkObject(skillEffect, pos, transform.rotation);
            networkObject.Spawn(true);
            StartCoroutine(ReturnSkillEffect(1f,networkObject));
        }
        [ServerRpc]
        public void SpawnAttackServerRpc(Vector3 pos)
        {
            SpawnerController.Instance.SpawnAttackHitEffect(pos);

        }
        IEnumerator ReturnSkillEffect(float time,NetworkObject networkObject)
        {
            yield return new WaitForSeconds(time);
            NetworkObjectPool.Singleton.ReturnNetworkObject(networkObject, skillEffect);
            networkObject.Despawn(false);
        }
        public void EatBooster(float takeAwayPoint)
        {
            networkPlayerHealth.Value += takeAwayPoint;
        }
        public void IncreaseHealth(float takeAwayPoint)
        {
            if (oldPlayerState == G2_PlayerState.Die)
            {
                return;
            }
            networkPlayerHealth.Value += takeAwayPoint;
            networkPlayerHealth.Value = networkPlayerHealth.Value > networkMaxPlayerHealth.Value ? networkMaxPlayerHealth.Value : networkPlayerHealth.Value;
            buffHpEffect.Play();
        }
        [ServerRpc]
        public void UpdateHealthServerRpc(float takeAwayPoint, ulong clientId,ulong attackerId)
        {
            var clientWithDamaged = NetworkManager.Singleton.ConnectedClients[clientId]
                .PlayerObject.GetComponent<G2_PlayerController>();

            if (clientWithDamaged != null && clientWithDamaged.networkPlayerHealth.Value > 0)
            {
                clientWithDamaged.networkPlayerHealth.Value -= takeAwayPoint;
                if(clientWithDamaged.networkPlayerHealth.Value <= 0)
                {
                    var attacker = NetworkManager.Singleton.ConnectedClients[attackerId]
                .PlayerObject.GetComponent<G2_PlayerController>();
                    attacker.LevelUp();
                }
            }

            // execute method on a client getting punch
            NotifyHealthChangedClientRpc(takeAwayPoint,attackerId, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });
        }
        [ClientRpc]
        public void NotifyHealthChangedClientRpc(float takeAwayPoint,ulong attackerId, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner) return;

            LoggerDebug.Instance.LogInfo($"Client got punch {takeAwayPoint} by Player {attackerId}");
        }
        [ServerRpc]
        public void ExitServerRpc(ulong clientId)
        {
            var client = NetworkManager.Singleton.ConnectedClients[clientId]
                .PlayerObject.GetComponent<G2_PlayerController>();
            client.ExitSever();
        }
        
        public void ExitSever()
        {
            Debug.Log("Exit");
            StartCoroutine(IEExitServer());
        }
        public IEnumerator IEExitServer()
        {
            yield return new WaitForSeconds(4);
            PlayersManager.Instance.DisconnectPlayer(GetComponent<NetworkObject>());

        }
        
    }
}