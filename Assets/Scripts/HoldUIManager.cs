using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldUIManager : MonoBehaviour
{
    List<Image> _baseImages;

    [SerializeField] MinoUI _holdMino;

    void OnEnable()
    {
        GameEventManager.OnHold += OnHold;
    }

    void OnDisable()
    {
        GameEventManager.OnHold -= OnHold;
    }

    void Awake()
    {
        _baseImages = new List<Image>();
    }

    void OnHold(object s, GameEventManager.OnHoldArgs e)
    {
        _holdMino.SetImage(e.holdPieceData._mino);
    }
}
