using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldUIManager : MonoBehaviour
{
    public static HoldUIManager Instance { get; private set; }

    List<Image> _baseImages;

    [SerializeField] MinoUI _holdMino;

    void Awake()
    {
        if (Instance) Destroy(this);
        Instance = this;

        _baseImages = new List<Image>();
    }

    public void HoldUIInit()
    {
        GameEventManager.HoldEvent += OnHold;
    }

    void OnHold(object s, GameEventManager.OnHoldArgs e)
    {
        _holdMino.SetImage(e.holdPieceData._mino);
    }
}
