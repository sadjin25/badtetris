using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tetris.CustomStructs;

public class RandomGenerator : MonoBehaviour
{
    public static RandomGenerator Instance { get; private set; }

    const int MAX_PIECE_NUM = 7;
    [SerializeField] MinoData[] _basePieceDataBag;
    bool[] _chkPiecePoppedInBag = new bool[MAX_PIECE_NUM];
    int _usedPieceNum;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        for (int i = 0; i < _basePieceDataBag.Length; i++)
        {
            _basePieceDataBag[i].Init();
        }

        ResetPieceCheckArr();
    }

    // Return Random MinoData by following 7-bag system  
    public MinoData GetRandomMinoData()
    {
        if (_usedPieceNum >= MAX_PIECE_NUM)
        {
            ResetPieceCheckArr();
            _usedPieceNum = 0;
        }
        ++_usedPieceNum;

        int index = Random.Range(0, MAX_PIECE_NUM);
        while (_chkPiecePoppedInBag[index])
        {
            index = Random.Range(0, MAX_PIECE_NUM);
        }

        MinoData dataToReturn = _basePieceDataBag[index];
        _chkPiecePoppedInBag[index] = true;

        return dataToReturn;
    }

    void ResetPieceCheckArr()
    {
        System.Array.Fill(_chkPiecePoppedInBag, false);
    }
}
