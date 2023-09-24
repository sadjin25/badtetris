using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct MinoImageSet
{
    public Mino minoType;
    public Image image;
}

public class MinoUI : MonoBehaviour
{
    [SerializeField] ImageHolder baseImageHolder;     // MINO ORDER : IOTLJSZ, SCORE ORDER : TETRIS, TSPIN SINGLE, TSPIN DOUBLE, TSPIN TRIPLE, B2B 
    Image currentImage;
    Mino currentPieceType;

    void Awake()
    {
        currentImage = GetComponentInChildren<Image>();
    }

    public void SetImage(Mino minoType)
    {
        currentPieceType = minoType;
        currentImage.sprite = baseImageHolder.sprites[(int)currentPieceType];
    }

    public void SetImage(ScoreType scoreType)
    {
        currentImage.sprite = baseImageHolder.sprites[(int)scoreType];
    }

    public void Clear()
    {
        currentImage.sprite = null;
    }
}
