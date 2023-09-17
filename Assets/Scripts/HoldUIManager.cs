using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldUIManager : MonoBehaviour
{
    public static HoldUIManager Instance { get; private set; }

    [SerializeField] Board board;
    List<Image> baseImages;

    [SerializeField] MinoUI holdMino;

    void Awake()
    {
        if (Instance) Destroy(this);
        Instance = this;

        baseImages = new List<Image>();
    }

    void Start()
    {
        board.HoldEvent += OnHold;
    }

    void OnHold(MinoData holdMinoData)
    {
        holdMino.SetImage(holdMinoData.mino);
    }
}