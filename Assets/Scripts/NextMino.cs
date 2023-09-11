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

public class NextMino : MonoBehaviour
{
    [SerializeField] MinoImageSet[] baseImages;     // ORDER : IOTLJSZ
    [SerializeField] Board board;
    [SerializeField] Image currentImage;
    Mino currentPieceType;

    public void SetImage(Mino minoType)
    {
        currentPieceType = minoType;
        currentImage.sprite = baseImages[(int)currentPieceType].image.sprite;
    }
}
