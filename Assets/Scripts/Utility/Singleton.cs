using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static Vector2 CameraBounds;
        private static T _instance;
        private static object _lock = new object();
        private static bool _applicationIsQuitting;
        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    Debug.LogWarning("[Singleton] Instance " + typeof(T) +
                    " already destroyed on application quit." +
                    "Won't create again - returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T);

                            DontDestroyOnLoad(singleton);

                            Debug.Log("[Singleton] An instance of " + typeof(T) +
                                " is needed in the scene, so '" + singleton +
                                "' was created with DontDestroyOnLoad.");
                        }
                        else
                        {
                            Debug.Log("[Singleton] Using instance already created: " +
                                _instance.gameObject.name);
                        }
                    }

                    return _instance;
                }
            }
        }
        public void OnDestroy()
        {
            _applicationIsQuitting = true;
        }

        public void CoroutineWithCallback(IEnumerator<T> coroutine, Action callback)
        {
            StartCoroutine(coroutine);
        }

    }
}
