using Extensions;
using System;

namespace FPS
{
    public class InputManager : Singleton<InputManager>
    {
        internal bool playerInputsEnabled = false;

        internal Action<bool> playerInputsUpdated;

        internal void SetPlayerInputs(bool enabled)
        {
            playerInputsEnabled = enabled;
            playerInputsUpdated?.Invoke(enabled);
        }

        internal void SetAllInputs(bool enabled)
        {
            SetPlayerInputs(enabled);
        }
    }
}
