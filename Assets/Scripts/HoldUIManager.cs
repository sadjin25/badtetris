using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldUIManager : MonoBehaviour
{
    public static HoldUIManager Instance { get; private set; }

    List<Image> baseImages;

    [SerializeField] MinoUI holdMino;

    void Awake()
    {
        if (Instance) Destroy(this);
        Instance = this;

        baseImages = new List<Image>();
    }

    public void HoldUIInit()
    {
        GameManager.HoldEvent += OnHold;
    }

    void OnHold(object s, GameManager.OnHoldArgs e)
    {
        holdMino.SetImage(e.holdPieceData.mino);
    }
}
