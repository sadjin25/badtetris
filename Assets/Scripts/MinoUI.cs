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
    [SerializeField] ImageHolder baseImageHolder;     // ORDER : IOTLJSZ
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
}
