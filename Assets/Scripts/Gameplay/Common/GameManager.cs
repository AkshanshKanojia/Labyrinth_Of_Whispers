using Extensions;
using UnityEngine;

namespace FPS
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        protected override void Awake()
        {
            //todo: remove later once game flow is finalized

            base.Awake();
            SaveManager.Initialize("testID");
        }
        internal void SetMouseVisible(bool visible)
        {
            Cursor.visible = visible;
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
