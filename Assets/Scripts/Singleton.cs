using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    [Header("Singleton")]
    [SerializeField] private bool _persistBetweenScenes;

    private static T _instance;
    public static T Instance => _instance;

    public static bool HasInstance => _instance != null;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;

        if (_persistBetweenScenes)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
