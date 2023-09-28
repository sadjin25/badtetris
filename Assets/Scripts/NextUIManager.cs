using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NextUIManager : MonoBehaviour
{
    public static NextUIManager Instance { get; private set; }

    [SerializeField] MinoUI[] _nextsArr;

    void OnEnable()
    {
        GameEventManager.OnNextMinoChanged += OnNextMinoChanged;
    }

    void OnDisable()
    {
        GameEventManager.OnNextMinoChanged -= OnNextMinoChanged;
    }

    void Awake()
    {
        if (Instance) Destroy(this);
        Instance = this;
    }

    public void NextUIInit()
    {
        GameEventManager.OnNextMinoChanged += OnNextMinoChanged;
    }

    void OnNextMinoChanged(object s, GameEventManager.OnNextMinoChangedArgs e)
    {
        for (int i = 0; i < GameManager._maxNextNum; ++i)
        {
            _nextsArr[i].SetImage(e.nextMinoDataList[i]._mino);
        }
    }
}
