using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//#if UNITY_EDITOR
//[ExecuteAlways]
//#endif

public class Singleton<T> : MonoBehaviour where T : Component
{
    [SerializeField] private bool _dontDestryOnLoad = false;
    [SerializeField] private bool _overrideInstance = false;

    protected static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectsOfType<T>().FirstOrDefault();
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            if (_overrideInstance)
            {
                if (Application.isPlaying == false)
                    DestroyImmediate(_instance.gameObject);
                else
                    Destroy(_instance.gameObject);

                _instance = this as T;
                Init();
            }
            else
            {
                if (Application.isPlaying == false)
                    DestroyImmediate(gameObject);
                else
                    Destroy(gameObject);
            }
        }
        else
        {
            _instance = this as T;

            if (_dontDestryOnLoad)
                DontDestroyOnLoad(gameObject);

            Init();
        }
    }

    protected virtual void Init()
    {
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this && !_overrideInstance)
        {
            _instance = null;
        }
    }
}