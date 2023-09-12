using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Playables;

namespace SkeletonEditor
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Run,
        Attack1h1,
        ReverseWalk,
    }
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField]
        private float walkSpeed = 3.5f;

        [SerializeField]
        private float runSpeedOffset = 2.0f;

        [SerializeField]
        private float rotationSpeed = 3.5f;

        [SerializeField]
        private Vector2 defaultInitialPositionOnPlane = new Vector2(-4, 4);

        [SerializeField]
        private NetworkVariable<Vector3> networkPositionDirection = new NetworkVariable<Vector3>();

        [SerializeField]
        private NetworkVariable<Vector3> networkRotationDirection = new NetworkVariable<Vector3>();

        [SerializeField]
        private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();
        [SerializeField]
        private NetworkVariable<float> networkSpeed = new NetworkVariable<float>();

        private CharacterController characterController;

        // client caches positions
        private Vector3 oldInputPosition = Vector3.zero;
        private Vector3 oldInputRotation = Vector3.zero;
        private PlayerState oldPlayerState = PlayerState.Idle;
        private float speed;
        private Animator animator;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
        }

        void Start()
        {
            if (IsClient && IsOwner)
            {
                transform.position = new Vector3(Random.Range(defaultInitialPositionOnPlane.x, defaultInitialPositionOnPlane.y), 0,
                       Random.Range(defaultInitialPositionOnPlane.x, defaultInitialPositionOnPlane.y));
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
                if (networkPlayerState.Value == PlayerState.Idle || networkPlayerState.Value == PlayerState.Walk || networkPlayerState.Value == PlayerState.Run || networkPlayerState.Value == PlayerState.ReverseWalk)
                {
                    animator.SetFloat("speedv", networkSpeed.Value);
                }
                else
                {
                    animator.SetTrigger(networkPlayerState.Value.ToString());
                }
            }
        }

        private void ClientInput()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            speed = Mathf.Abs(v)*walkSpeed;
            UpdateCurrentSpeedServerRpc(speed);
            // left & right rotation
            Vector3 inputRotation = new Vector3(0, h, 0);

            // forward & backward direction
            Vector3 direction = transform.TransformDirection(Vector3.forward);
            float forwardInput = v;
            Vector3 inputPosition = direction * forwardInput;

            // change animation states
            if (ActivePunchActionKey() && forwardInput == 0)
            {
                UpdatePlayerStateServerRpc(PlayerState.Attack1h1);
                return;
            }
            if (forwardInput == 0)
                UpdatePlayerStateServerRpc(PlayerState.Idle);
            else if (!ActiveRunningActionKey() && forwardInput > 0 && forwardInput <= 1)
                UpdatePlayerStateServerRpc(PlayerState.Walk);
            else if (ActiveRunningActionKey() && forwardInput > 0 && forwardInput <= 1)
            {
                inputPosition = direction * runSpeedOffset;
                UpdatePlayerStateServerRpc(PlayerState.Run);
            }
            else if (forwardInput < 0)
                UpdatePlayerStateServerRpc(PlayerState.ReverseWalk);

            // let server know about position and rotation client changes
            if (oldInputPosition != inputPosition ||
                oldInputRotation != inputRotation)
            {
                oldInputPosition = inputPosition;
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

        [ServerRpc]
        public void UpdateClientPositionAndRotationServerRpc(Vector3 newPosition, Vector3 newRotation)
        {
            networkPositionDirection.Value = newPosition;
            networkRotationDirection.Value = newRotation;
        }

        [ServerRpc]
        public void UpdatePlayerStateServerRpc(PlayerState state)
        {
            networkPlayerState.Value = state;
        }
        [ServerRpc]
        public void UpdateCurrentSpeedServerRpc(float speed)
        {
            networkSpeed.Value = speed;
        }
    }
}