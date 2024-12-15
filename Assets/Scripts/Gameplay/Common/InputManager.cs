using Extensions;
using UnityEngine;

namespace FPS
{
    public class InputManager : Singleton<InputManager>
    {
        internal bool playerInputsEnabled = false;

        internal void SetPlayerInputs(bool enabled)
        {
            playerInputsEnabled = enabled;
        }

        internal void SetAllInputs(bool enabled)
        {
            playerInputsEnabled = enabled;
        }
    }
}
