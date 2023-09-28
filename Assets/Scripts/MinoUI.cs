using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Tetris.EnumTypes;

[System.Serializable]
public struct MinoImageSet
{
    public Mino _minoType;
    public Image _image;
}

public class MinoUI : MonoBehaviour
{
    [SerializeField] ImageHolder _baseImageHolder;     // MINO ORDER : IOTLJSZ, SCORE ORDER : TETRIS, TSPIN SINGLE, TSPIN DOUBLE, TSPIN TRIPLE, B2B 
    Image _currentImage;
    Mino _currentPieceType;

    void Awake()
    {
        _currentImage = GetComponentInChildren<Image>();
    }

    public void SetImage(Mino minoType)
    {
        _currentPieceType = minoType;
        _currentImage.sprite = _baseImageHolder.sprites[(int)_currentPieceType];
    }

    public void SetImage(ScoreType scoreType)
    {
        _currentImage.sprite = _baseImageHolder.sprites[(int)scoreType];
    }

    public void Clear()
    {
        _currentImage.sprite = null;
    }
}
