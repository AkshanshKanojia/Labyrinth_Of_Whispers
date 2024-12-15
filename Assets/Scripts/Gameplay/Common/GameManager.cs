using Extensions;
using UnityEngine;

namespace FPS
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        internal void SetMouseVisible(bool visible)
        {
            Cursor.visible = visible;
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
