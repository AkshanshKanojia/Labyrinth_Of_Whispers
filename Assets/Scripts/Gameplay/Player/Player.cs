using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FPS
{
    public class Player : MonoBehaviour
    {
        [SerializeField] internal PlayerMovementData playerMoventData;

        private PlayerInputHandler _playerInputHandler;

        private void Start()
        {
            Initialize();
            InputManager.Instance.SetPlayerInputs(true);
        }

        internal void Initialize()
        {
            _playerInputHandler = GetComponent<PlayerInputHandler>();
            _playerInputHandler.Initialize();

            InitalizeInputActions();
        }

        private void InitalizeInputActions()
        {
            _playerInputHandler.OnInputUpdated += CheckForMovement;
        }

        private void RemoveInputActions()
        {
            _playerInputHandler.OnInputUpdated -= CheckForMovement;
        }

        private void OnDestroy()
        {
            RemoveInputActions();
        }

        private void CheckForMovement(AvailablePlayerActions action, InputAction.CallbackContext ctx)
        {
            if (action == AvailablePlayerActions.Move)
            {
                switch (ctx.phase)
                {
                    case InputActionPhase.Performed:
                        MovePlayer(ctx.ReadValue<Vector2>());
                        break;

                    case InputActionPhase.Canceled:
                        StopPlayerMovement();
                        break;

                    default:
                        break;
                }
            }
        }

        private void MovePlayer(Vector2 direction)
        {
            print("Moving Player:  " + direction);
        }

        private void StopPlayerMovement()
        {
            print("stopped Player");
        }

        [Serializable]
        internal class PlayerMovementData
        {
            public float WalkingSpeed = 3f;
            public float SprintSpeed = 6f;
            public float CrouchSpeed = 1.5f;

            [Header("Look Properties")]

            public float LookSensitivity = 2f;

            public float MinLookX = -90f;
            public float MaxLookX = 90f;

            [HideInInspector] public float CurrentSpeed;

            internal void Initialize()
            {
                CurrentSpeed = WalkingSpeed;
            }
        }
    }
}
