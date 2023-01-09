using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    public static T Instance;

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            string typename = typeof(T).Name;
            Debug.LogWarning($"More that one instance of {typename} found.\nDestroying this gameObject", gameObject);
            Destroy(gameObject);
        }
        else
        {
            Instance = this as T;
        }
    }
}
