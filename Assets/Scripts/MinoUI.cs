using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Tetris.EnumTypes;
using Tetris.CustomStructs;

public class MinoUI : MonoBehaviour
{
    // MINO ORDER  : IOTLJSZ
    // SCORE ORDER : TETRIS, TSPIN SINGLE, TSPIN DOUBLE, TSPIN TRIPLE, B2B 
    [SerializeField] ImageHolder _baseImageHolder;
    MinoUISet _minoUISet;

    void Awake()
    {
        _minoUISet._image = GetComponentInChildren<Image>();
        _minoUISet._text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetImage(Mino minoType)
    {
        _minoUISet._minoType = minoType;
        _minoUISet._image.sprite = _baseImageHolder.sprites[(int)minoType];
    }

    public void SetImage(ScoreType scoreType)
    {
        _minoUISet._image.sprite = _baseImageHolder.sprites[(int)scoreType];
    }

    public void ClearImage()
    {
        _minoUISet._image.sprite = null;
    }

    public void UpdateText(int value)
    {
        _minoUISet._text.text = value.ToString();
    }
}
