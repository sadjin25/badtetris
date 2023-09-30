using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NextUIManager : BaseUIManager
{
    [SerializeField] MinoUI[] _nextsArr;

    public override void UIManagerEnable()
    {
        GameEventManager.OnNextMinoChanged += OnNextMinoChanged;
    }

    public override void UIManagerDisable()
    {
        GameEventManager.OnNextMinoChanged -= OnNextMinoChanged;
    }

    void OnNextMinoChanged(object s, GameEventManager.OnNextMinoChangedArgs e)
    {
        for (int i = 0; i < GameManager._maxNextNum; ++i)
        {
            _nextsArr[i].SetImage(e.nextMinoDataList[i]._mino);
        }
    }
}
