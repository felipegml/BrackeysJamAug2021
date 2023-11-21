using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class ManagerBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T instance = CreateSingleton();

        public static T Get() => instance;

        private static T CreateSingleton()
        {
            if (instance == null)
            {
                Debug.Log(typeof(T).Name);
                var ownerObject = new GameObject(typeof(T).Name);
                instance = ownerObject.AddComponent<T>();
                DontDestroyOnLoad(ownerObject);
            }

            return instance;
        }
    }
}
