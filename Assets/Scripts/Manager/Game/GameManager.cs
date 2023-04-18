using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { Init(); return _instance; } }



    AreaManager _area = new AreaManager();


    public static AreaManager Area { get { return Instance._area; } }



    static void Init()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<GameManager>();
            if (_instance == null)
            {
                var go = new GameObject(name: "@GameManagers");
                _instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go);
            }
        }
    }


    private void Awake()
    {
        Init();
        Area.Init();
    }

    void Start()
    {

    }

    void Update()
    {

    }

}
