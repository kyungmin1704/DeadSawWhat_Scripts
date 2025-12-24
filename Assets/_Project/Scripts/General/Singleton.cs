using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance => instance;
    [Tooltip("Dont Destroy On Load 씬에 적재할 여부")]
    public bool isDontDestroyOnLoad = false;

    protected virtual void Awake()
    {
        if (instance == null)
            instance = FindAnyObjectByType<T>();
        else
        {
            Destroy(gameObject);
            return;
        }
        
        if (isDontDestroyOnLoad) SetDDOL();
    }

    private void SetDDOL()
    {
        if (transform.root != null || transform.parent != null)
            DontDestroyOnLoad(transform.root);
        else
            DontDestroyOnLoad(gameObject);
    }
}
