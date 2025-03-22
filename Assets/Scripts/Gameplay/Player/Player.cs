using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FPS
{
    public class Player : MonoBehaviour
    {
        [SerializeField] internal PlayerMovementData playerMoventData;

        [Header("Camera Properties")]
        [SerializeField] private Transform _playerCameraTransform;
        [SerializeField] private Transform _playerBodyVisual;
        [SerializeField] private Transform _playerStandingCameraPoint, _playerCrouchingCameraPoint;
        [SerializeField] private Transform _playerBodyStandingPoint, _playerBodyCrouchingPoint;
        [SerializeField] private float _cameraTransitionTime = 1.5f;

        internal Tween cameraTransitionTween, playerBodyCrouchTween;
        internal Transform CameraTransform => _playerCameraTransform;

        [Header("Player Components")]
        [SerializeField] private GameObject _standingColliderObj;
        [SerializeField] private GameObject _crouchedColliderObj;

        private PlayerInputHandler _playerInputHandler;
        private Rigidbody _playerRB;
        private PlayerInventory _playerInventory;
        private Animator _playerAnimator;

        private Vector2 _mouseLookAngle;
        private Vector2 _currentMovementInput;

        internal bool movementEnabled = true;
        internal bool canUpdateAnimationParameters = true;
        internal bool lookEnabled = true;

        #region Constants and animation variables
        private const string _walkAnimationId = "IsWalking";
        private const string _crouchAnimationId = "IsCrouching";

        private Vector2 _animationUpdateMovementVector;
        private bool _animationUpdateCrouchState;
        #endregion

        #region Initialization and Default Methods

        private void Start()
        {
            //todo: change initalization calls via level/game manager once testing is done
            Initialize();
            SetAllInputsEnabled(true);
        }

        internal void Initialize()
        {
            InitializeComponents();

            InitalizeInputActions();

            //enable standing state by default
            SetCrouchState(false);
        }

        private void InitializeComponents()
        {
            _playerInputHandler = GetComponent<PlayerInputHandler>();
            _playerRB = GetComponent<Rigidbody>();
            _playerInventory = PlayerInventory.Instance;
            _playerAnimator = GetComponentInChildren<Animator>();

            _playerInputHandler.Initialize();
            playerMoventData.Initialize();

            InputManager.Instance.playerInputsUpdated += PlayerInputUpdated;
        }

        private void InitalizeInputActions()
        {
            _playerInputHandler.OnInputUpdated += CheckForInput;
        }

        private void RemoveAllActions()
        {
            InputManager.Instance.playerInputsUpdated -= PlayerInputUpdated;
            _playerInputHandler.OnInputUpdated -= CheckForInput;
        }

        private void Update()
        {
            UpdateAnimationParameters();
        }

        private void FixedUpdate()
        {
            CheckForPlayerMovement();
        }

        private void OnDestroy()
        {
            RemoveAllActions();
        }

        #endregion

        #region Camera Transition and Look Methods

        internal void SetLookEnabled(bool enabled)
        {
            lookEnabled = enabled;
        }
        private void SetCameraCrouchPos(bool crouched)
        {
            cameraTransitionTween?.Kill();
            playerBodyCrouchTween?.Kill();

            Vector3 targetPos = crouched ? _playerCrouchingCameraPoint.localPosition : _playerStandingCameraPoint.localPosition;
            Vector3 targetBodyPos = crouched ? _playerBodyCrouchingPoint.localPosition : _playerBodyStandingPoint.localPosition;

            cameraTransitionTween = _playerCameraTransform.DOLocalMove(targetPos, _cameraTransitionTime).SetEase(Ease.OutQuad);
            playerBodyCrouchTween = _playerBodyVisual.DOLocalMove(targetBodyPos, _cameraTransitionTime).SetEase(Ease.OutQuad);
        }

        private void UpdateMouseLookAngle(Vector2 value)
        {
            if (!lookEnabled)
            {
                return;
            }

            value *= 0.05f * playerMoventData.LookSensitivity;//note: 0.05f is a multiplier to reduce the sensitivity of the mouse movement, can be exposed if needed but would be constant in most cases

            _mouseLookAngle.x += value.x;
            _mouseLookAngle.y += value.y * (playerMoventData.useInvertedY ? 1 : -1);

            _mouseLookAngle.y = Mathf.Clamp(_mouseLookAngle.y, playerMoventData.MinLookX, playerMoventData.MaxLookX);

            transform.eulerAngles = new Vector3(0, _mouseLookAngle.x, 0);
            _playerCameraTransform.localEulerAngles = new Vector3(_mouseLookAngle.y, 0, 0);
        }
        #endregion

        #region Animation Methods

        internal void SetAnimationUpdateChecks(bool canUpdate)
        {
            canUpdateAnimationParameters = canUpdate;
        }
        private void UpdateAnimationParameters()
        {
            if (_playerAnimator && canUpdateAnimationParameters)
            {
                //added parameters check to avoid extra calling of animatior methodsz
                if (_animationUpdateMovementVector != _currentMovementInput)
                {
                    _playerAnimator.SetBool(_walkAnimationId, _currentMovementInput != Vector2.zero);
                    _animationUpdateMovementVector = _currentMovementInput;
                }

                if (_animationUpdateCrouchState != playerMoventData.isCrouching)
                {
                    _animationUpdateCrouchState = playerMoventData.isCrouching;
                    _playerAnimator.SetBool(_crouchAnimationId, playerMoventData.isCrouching);
                }
            }
        }
        #endregion

        #region Input Handling
        internal void SetMoveAndLook(bool enabled)
        {
            SetPlayerMovementEnabled(enabled);
            SetLookEnabled(enabled);
        }

        internal void SetAllInputsEnabled(bool enabled)
        {
            InputManager.Instance.SetPlayerInputs(enabled);
        }

        private void PlayerInputUpdated(bool enabled)
        {
            if (!enabled)
            {
                StopPlayerMovement();
            }
        }

        private void CheckForInput(AvailablePlayerActions action, InputAction.CallbackContext ctx)
        {
            if (!InputManager.Instance.playerInputsEnabled)
            {
                return;
            }

            CheckForPlayerMovement(action, ctx);
            CheckForMouseLook(action, ctx);
            CheckForActionKeys(action, ctx);
        }

        private void CheckForMouseLook(AvailablePlayerActions action, InputAction.CallbackContext ctx)
        {
            if (action == AvailablePlayerActions.Look)
            {
                UpdateMouseLookAngle(ctx.ReadValue<Vector2>());
            }
        }


        private void CheckForActionKeys(AvailablePlayerActions action, InputAction.CallbackContext ctx)
        {
            if (action == AvailablePlayerActions.Crouch)
            {
                switch (ctx.phase)
                {
                    case InputActionPhase.Performed:
                        if (playerMoventData.isSprinting)
                        {
                            return;//sprint overrides crouch
                        }

                        playerMoventData.ToggleCrouchSpeed();
                        SetCameraCrouchPos(playerMoventData.isCrouching);
                        break;

                    default:
                        break;
                }
            }

            //sprint action
            if (action == AvailablePlayerActions.Sprint)
            {
                switch (ctx.phase)
                {
                    case InputActionPhase.Started:
                        SetCameraCrouchPos(false);
                        playerMoventData.SetSprinting(true);
                        break;
                    case InputActionPhase.Canceled:
                        playerMoventData.SetSprinting(false);
                        break;
                    default:
                        break;
                }
            }

            //inventory action
            if (action == AvailablePlayerActions.Inventory)
            {
                switch (ctx.phase)
                {
                    case InputActionPhase.Performed:
                        _playerInventory.ToggleInventory();
                        SetMoveAndLook(!_playerInventory.isInventoryOpen);
                        break;
                    default:
                        break;
                }
            }
        }


        #endregion

        #region Player Movement

        internal void SetPlayerMovementEnabled(bool enabled)
        {
            movementEnabled = enabled;
        }

        internal void SetCrouchState(bool crouched)
        {
            if (playerMoventData.isSprinting)
            {
                crouched = false;//sprint overrides crouched state
            }

            playerMoventData.SetCrouchState(crouched);
            SetCameraCrouchPos(crouched);
            SetCrouchedCollider(crouched);
        }

        private void SetCrouchedCollider(bool crouched)
        {
            _standingColliderObj.SetActive(!crouched);
            _crouchedColliderObj.SetActive(crouched);
        }

        private void CheckForPlayerMovement(AvailablePlayerActions action, InputAction.CallbackContext ctx)
        {
            if (action == AvailablePlayerActions.Move)
            {
                switch (ctx.phase)
                {
                    case InputActionPhase.Performed:
                        UpdatePlayerMovementDirection(ctx.ReadValue<Vector2>());
                        break;

                    case InputActionPhase.Canceled:
                        StopPlayerMovement();
                        break;

                    default:
                        break;
                }
            }
        }

        private void UpdatePlayerMovementDirection(Vector2 direction)
        {
            _currentMovementInput = direction;
        }

        private void CheckForPlayerMovement()
        {
            if (_currentMovementInput == Vector2.zero || !movementEnabled)
            {
                return;
            }

            Vector3 moveDirection = transform.right * _currentMovementInput.x + transform.forward * _currentMovementInput.y;
            moveDirection.y = 0;
            _playerRB.MovePosition(transform.position + playerMoventData.CurrentSpeed * Time.fixedDeltaTime * moveDirection.normalized);
        }

        private void StopPlayerMovement()
        {
            _currentMovementInput = Vector2.zero;
        }

        #endregion

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
            public bool useInvertedY = false;

            [HideInInspector] public float CurrentSpeed;

            internal bool isCrouching;
            internal bool isSprinting;
            internal bool useSmoothSpeedBlending;
            internal float smoothSpeedBlendTime = 1f;
            internal Tween speedBlendTween;

            internal void Initialize()
            {
                CurrentSpeed = WalkingSpeed;
            }

            internal void ToggleCrouchSpeed()
            {
                isCrouching = !isCrouching;
                if (isSprinting)
                {
                    isCrouching = false;//sprint override crouch
                }
                UpdateCurrentSpeed();
            }

            internal void SetCrouchState(bool crouch)
            {
                isCrouching = crouch;
                UpdateCurrentSpeed();
            }

            private void UpdateCurrentSpeed()
            {
                float targetSpeed = isCrouching ? CrouchSpeed : WalkingSpeed;

                UpdateCurrentSpeed(isSprinting ? SprintSpeed : targetSpeed);
            }

            private void UpdateCurrentSpeed(float targetValue)
            {
                float tweenTime = useSmoothSpeedBlending ? smoothSpeedBlendTime : 0;

                speedBlendTween?.Kill();
                speedBlendTween = DOTween.To(() => CurrentSpeed, x => CurrentSpeed = x, targetValue, tweenTime).
                    SetEase(Ease.InOutQuad);
            }

            internal void SetSprinting(bool sprint)
            {
                isSprinting = sprint;
                if (isSprinting)
                {
                    isCrouching = false;
                }

                UpdateCurrentSpeed();
            }
        }
    }
}
