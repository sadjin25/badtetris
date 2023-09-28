using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NextUIManager : MonoBehaviour
{
    public static NextUIManager Instance { get; private set; }

    [SerializeField] MinoUI[] nextsArr;

    void Awake()
    {
        if (Instance) Destroy(this);
        Instance = this;
    }

    public void NextUIInit()
    {
        GameManager.Instance.OnNextMinoChanged += OnNextMinoChanged;
    }

    void OnNextMinoChanged(object s, GameManager.OnNextMinoChangedArgs e)
    {
        for (int i = 0; i < GameManager.maxNextNum; ++i)
        {
            nextsArr[i].SetImage(e.nextMinoDataList[i].mino);
        }
    }
}
