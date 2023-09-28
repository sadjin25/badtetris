using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Tetris.EnumTypes;
using Tetris.CustomStructs;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance { get; private set; }

    public static event EventHandler<OnHoldArgs> HoldEvent;
    public class OnHoldArgs : EventArgs
    {
        public MinoData holdPieceData;
    }

    public static event EventHandler<OnNextMinoChangedArgs> OnNextMinoChanged;
    public class OnNextMinoChangedArgs : EventArgs
    {
        public List<MinoData> nextMinoDataList;
    }

    public static event EventHandler<OnActiveMinoChangedArgs> OnActiveMinoChanged;
    public class OnActiveMinoChangedArgs : EventArgs
    {
        public Piece activePiece;
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
        HoldEvent?.Invoke(this, new OnHoldArgs { holdPieceData = holdPieceData });
    }

    public void InvokeNextMinoChangedEvent(List<MinoData> nextMinoDataList)
    {
        OnNextMinoChanged?.Invoke(this, new OnNextMinoChangedArgs { nextMinoDataList = nextMinoDataList });
    }

    public void InvokeScoringEvent(ScoreType scoreType)
    {
        OnScoring?.Invoke(this, new OnScoringArgs { scoreType = scoreType });
    }

    public void InvokeActiveMinoChangedEvent(Piece activePiece)
    {
        OnActiveMinoChanged?.Invoke(this, new OnActiveMinoChangedArgs { activePiece = activePiece });
    }
}
