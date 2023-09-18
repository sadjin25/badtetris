using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGenerator : MonoBehaviour
{
    public static RandomGenerator Instance { get; private set; }

    const int MAX_PIECE_NUM = 7;
    [SerializeField] MinoData[] basePieceDataBag;
    bool[] chkPiecePoppedInBag = new bool[MAX_PIECE_NUM];
    int usedPieceNum;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        for (int i = 0; i < basePieceDataBag.Length; i++)
        {
            basePieceDataBag[i].Init();
        }

        ResetPieceCheckArr();
    }

    // Return Random MinoData by following 7-bag system  
    public MinoData GetRandomMino()
    {
        if (usedPieceNum >= MAX_PIECE_NUM)
        {
            ResetPieceCheckArr();
            usedPieceNum = 0;
        }
        ++usedPieceNum;

        int index = Random.Range(0, MAX_PIECE_NUM);
        while (chkPiecePoppedInBag[index])
        {
            index = Random.Range(0, MAX_PIECE_NUM);
        }

        MinoData dataToReturn = basePieceDataBag[index];
        chkPiecePoppedInBag[index] = true;

        return dataToReturn;
    }

    void ResetPieceCheckArr()
    {
        System.Array.Fill(chkPiecePoppedInBag, false);
    }
}
