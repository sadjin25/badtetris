using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Tetris.EnumTypes;
using Tetris.CustomStructs;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance { get; private set; }

    public static event EventHandler<OnHoldArgs> OnHold;
    public class OnHoldArgs : EventArgs
    {
        public MinoData holdPieceData;
    }

    public static event EventHandler<OnNextMinoChangedArgs> OnNextMinoChanged;
    public class OnNextMinoChangedArgs : EventArgs
    {
        public List<MinoData> nextMinoDataList;
    }

    public static event EventHandler<OnScoringArgs> OnScoring;
    public class OnScoringArgs : EventArgs
    {
        public ScoreType scoreType;
    }

    void Awake()
    {
        if (Instance) Destroy(this);
        Instance = this;
    }

    public void InvokeHoldEvent(MinoData holdPieceData)
    {
        OnHold?.Invoke(this, new OnHoldArgs { holdPieceData = holdPieceData });
    }

    public void InvokeNextMinoChangedEvent(List<MinoData> nextMinoDataList)
    {
        OnNextMinoChanged?.Invoke(this, new OnNextMinoChangedArgs { nextMinoDataList = nextMinoDataList });
    }

    public void InvokeScoringEvent(ScoreType scoreType)
    {
        OnScoring?.Invoke(this, new OnScoringArgs { scoreType = scoreType });
    }
}
