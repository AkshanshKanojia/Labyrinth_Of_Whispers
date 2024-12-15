using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FPS
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _playerInputSheet;

        private InputManager _inputManager;

        internal Action<AvailablePlayerActions, InputAction.CallbackContext> OnInputUpdated;

        internal void Initialize()
        {
            _inputManager = InputManager.Instance;

            InitializeInputActions();

            _playerInputSheet.Enable();
        }

        private void InitializeInputActions()
        {
            //assign all relevant events
            foreach (var action in _playerInputSheet.actionMaps[0].actions)
            {
                action.started += ctx => ProcessAction(ctx, GetActionType(action));

                action.performed += ctx => ProcessAction(ctx, GetActionType(action));

                action.canceled += ctx => ProcessAction(ctx, GetActionType(action));
            }
        }

        private AvailablePlayerActions GetActionType(InputAction action)
        {
            return action.name switch
            {
                "Move" => AvailablePlayerActions.Move,
                "Look" => AvailablePlayerActions.Look,
                "Sprint" => AvailablePlayerActions.Sprint,
                "Interact" => AvailablePlayerActions.Interact,
                "Flashlight" => AvailablePlayerActions.Flashlight,
                "Crouch" => AvailablePlayerActions.Crouch,
                "LMB" => AvailablePlayerActions.LMB,
                "RMB" => AvailablePlayerActions.RMB,
                "Inventory" => AvailablePlayerActions.Inventory,
                _ => AvailablePlayerActions.Move,
            };
        }

        private void ProcessAction(InputAction.CallbackContext callback, AvailablePlayerActions playerActions)
        {
            //handle all related event and detection based on type of input passed like toggle crouch hold sprint etc

            if (!_inputManager.playerInputsEnabled)
            {
                return;
            }

            OnInputUpdated?.Invoke(playerActions, callback); // Trigger action at start
        }

    }

    internal enum AvailablePlayerActions
    {
        Move,
        Look,
        Sprint,
        Interact,
        Flashlight,
        Crouch,
        LMB,
        RMB,
        Inventory,
    }
}
