using UnityEngine;

namespace FG
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T: MonoBehaviour
    {
        private static T Instance;
        public static T instance
        { 
            get
            {
                return Instance;
            }
        }

        protected virtual void Awake()
        {
            if (Instance == null)
                Instance = this as T;
            else
                Destroy(gameObject);
        }
    }
}
