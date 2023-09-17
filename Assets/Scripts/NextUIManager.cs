using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NextUIManager : MonoBehaviour
{
    public static NextUIManager Instance { get; private set; }

    [SerializeField] Board board;
    List<Image> baseImages;

    [SerializeField] MinoUI[] nextsArr;

    void Awake()
    {
        if (Instance) Destroy(this);
        Instance = this;

        baseImages = new List<Image>();
    }

    void Start()
    {
        board.OnNextMinoChanged += OnNextMinoChanged;
    }

    void OnNextMinoChanged(object s, Board.OnNextMinoChangedArgs e)
    {
        for (int i = 0; i < Board.maxNextNum; ++i)
        {
            nextsArr[i].SetImage(e.nextMinoes[i].data.mino);
        }
    }
}
