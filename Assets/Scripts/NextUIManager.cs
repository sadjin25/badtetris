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

    void Start()
    {
        Board.Instance.OnNextMinoChanged += OnNextMinoChanged;
    }

    void OnNextMinoChanged(object s, Board.OnNextMinoChangedArgs e)
    {
        for (int i = 0; i < Board.maxNextNum; ++i)
        {
            nextsArr[i].SetImage(e.nextMinoDataList[i].mino);
        }
    }
}
