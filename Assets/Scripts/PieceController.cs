using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    [SerializeField] Piece _piece;

    [SerializeField] InputReader _inputReader;

    //WARNING : if some action function(rotate, harddrop.. etc) is used, then CLEAR input vars.
    //          Clearing always be performed in Input checking update func!
    Vector2 _moveInput;
    Vector2 _bfrMoveInput;
    bool _hardDropInput;
    bool _softDropInput;
    bool _rotateLInput;
    bool _rotateRInput;
    bool _holdInput;

    void OnEnable()
    {
        _inputReader.HardDropEvent += OnHardDrop;
        _inputReader.SoftDropEvent += OnSoftDrop;
        _inputReader.SoftDropCancelEvent += OnSoftDropCancel;
        _inputReader.MoveEvent += OnMove;
        _inputReader.RotateLEvent += OnRotateL;
        _inputReader.RotateREvent += OnRotateR;
        _inputReader.HoldEvent += OnHold;
    }

    void OnDisable()
    {
        _inputReader.HardDropEvent -= OnHardDrop;
        _inputReader.SoftDropEvent -= OnSoftDrop;
        _inputReader.SoftDropCancelEvent -= OnSoftDropCancel;
        _inputReader.MoveEvent -= OnMove;
        _inputReader.RotateLEvent -= OnRotateL;
        _inputReader.RotateREvent -= OnRotateR;
        _inputReader.HoldEvent -= OnHold;
    }

    public void InputUpdate()
    {
        if (_holdInput)
        {
            _holdInput = false;
            _piece.Hold();
        }

        if (_rotateLInput)
        {
            _rotateLInput = false;
            _piece.Rotate(-1);
        }
        else if (_rotateRInput)
        {
            _rotateRInput = false;
            _piece.Rotate(1);
        }

        if (_moveInput.x < -0.2f)
        {
            if (_bfrMoveInput != _moveInput)
            {
                _piece.ResetAllDASDelay();
            }
            _bfrMoveInput = _moveInput;
            _piece.MoveWithDAS(Vector2Int.left);
        }
        else if (_moveInput.x > 0.2f)
        {
            if (_bfrMoveInput != _moveInput)
            {
                _piece.ResetAllDASDelay();
            }
            _bfrMoveInput = _moveInput;
            _piece.MoveWithDAS(Vector2Int.right);
        }
        else
        {
            _bfrMoveInput = Vector2Int.zero;
            _piece.ResetAllDASDelay();
        }

        if (_softDropInput)
        {
            _piece.SoftDrop();
        }

        else if (_hardDropInput)
        {
            _hardDropInput = false;
            _piece.HardDrop();
        }
    }

    #region Event Listeners
    void OnMove(Vector2 input)
    {
        _moveInput = input;
    }

    void OnHardDrop() => _hardDropInput = true;

    void OnSoftDrop()
    {
        _softDropInput = true;

    }

    void OnSoftDropCancel()
    {
        _softDropInput = false;
    }

    void OnRotateL() => _rotateLInput = true;

    void OnRotateR() => _rotateRInput = true;

    void OnHold() => _holdInput = true;
    #endregion
}
