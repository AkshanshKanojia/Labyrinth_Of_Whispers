using UnityEngine;
namespace Extensions
{
    public class Singleton<t> : MonoBehaviour where t : MonoBehaviour
    {
       public static t Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as t;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public class PersistentSingleton<t> : MonoBehaviour where t : MonoBehaviour
    {
        public static t Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as t;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
