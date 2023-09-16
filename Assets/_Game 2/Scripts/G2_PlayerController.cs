using System.Collections.Generic;
using System.Globalization;
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

        private CharacterController characterController;

        // client caches positions
        private Vector3 oldInputPosition = Vector3.zero;
        private Vector3 oldInputRotation = Vector3.zero;
        private G2_PlayerState oldPlayerState = G2_PlayerState.Idle;
        private float speed;
        [SerializeField]
        private Animator animator;

        private bool isAttacking =false;
        private float attackRange = 10f;
        private float heightUpAttack = 3;
        private float timeDelayCheckHit = 0.5f;
        private bool isCheckHit = true;
        [SerializeField]private ParticleSystem effectZone;
        [SerializeField] private ParticleSystem effectAttack;
        private void Awake()
        {
            characterController = GetComponent<CharacterController>();

        }

        void Start()
        {
            if (IsClient && IsOwner)
            {
                transform.position = new Vector3(Random.Range(defaultInitialPositionOnPlane.x, defaultInitialPositionOnPlane.y), 0,
                       Random.Range(defaultInitialPositionOnPlane.x, defaultInitialPositionOnPlane.y));
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
            if (IsClient && IsOwner)
            {
                CheckAlive();

            }


        }
        private void FixedUpdate()
        {
            if (IsClient && IsOwner)
            {
                if (networkPlayerState.Value == G2_PlayerState.Attack)
                {
                    CheckHit();
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
            if(oldPlayerState == G2_PlayerState.Die)
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
            if (ActivePunchActionKey() && forwardInput == 0 && !isAttacking && oldPlayerState !=G2_PlayerState.Attack)
            {
                UpdatePlayerStateServerRpc(G2_PlayerState.Attack);
                isAttacking=true;
                Invoke(nameof(ResetAttack), 2f);
                Invoke(nameof(SetCheckHit), timeDelayCheckHit);
                return;
            }
            if (forwardInput == 0 && !isAttacking )
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
                oldInputPosition = inputPosition;
                oldInputRotation = inputRotation;
                UpdateClientPositionAndRotationServerRpc(inputPosition * walkSpeed, inputRotation * rotationSpeed);
            }
        }
        private static bool ActivePunchActionKey()
        {
            return Input.GetKey(KeyCode.Space);
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

        private void CheckHit()
        {
            if (isCheckHit)
            {
                return;
            }
            isCheckHit = true;
            Debug.Log("hit");
            RaycastHit hit;

            int layerMask = LayerMask.GetMask("Player");

            if (Physics.Raycast(transform.position + Vector3.up* heightUpAttack, transform.forward, out hit, attackRange, layerMask))
            {
                Debug.DrawRay(transform.position + Vector3.up* heightUpAttack, transform.forward * attackRange, Color.yellow);

                var playerHit = hit.transform.GetComponent<NetworkObject>();
                if (playerHit != null)
                {
                    UpdateHealthServerRpc(500, playerHit.OwnerClientId);
                    effectAttack.transform.position = hit.transform.position + Vector3.up *heightUpAttack;
                    effectAttack.Play();
                    effectAttack.GetComponent<NetworkObject>().Despawn();
                    Debug.Log("Attack a enemy");
                }
            }
            else
            {
                Debug.DrawRay(transform.position + Vector3.up* heightUpAttack, transform.forward * attackRange, Color.red);
            }
        }
        [ServerRpc]
        public void UpdateHealthServerRpc(int takeAwayPoint, ulong clientId)
        {
            var clientWithDamaged = NetworkManager.Singleton.ConnectedClients[clientId]
                .PlayerObject.GetComponent<G2_PlayerController>();

            if (clientWithDamaged != null && clientWithDamaged.networkPlayerHealth.Value > 0)
            {
                clientWithDamaged.networkPlayerHealth.Value -= takeAwayPoint;
            }

            // execute method on a client getting punch
            NotifyHealthChangedClientRpc(takeAwayPoint, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });
        }
        [ClientRpc]
        public void NotifyHealthChangedClientRpc(int takeAwayPoint, ClientRpcParams clientRpcParams = default)
        {
            if (IsOwner) return;

            LoggerDebug.Instance.LogInfo($"Client got punch {takeAwayPoint}");
        }
    }
}