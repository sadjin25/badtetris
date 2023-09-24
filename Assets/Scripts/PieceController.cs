using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    [SerializeField] Piece piece;

    [SerializeField] InputReader inputReader;

    //WARNING : if some action function(rotate, harddrop.. etc) is used, then CLEAR input vars.
    //          Clearing always be performed in Input checking update func!
    Vector2 moveInput;
    Vector2 bfrMoveInput;
    bool hardDropInput;
    bool softDropInput;
    bool rotateLInput;
    bool rotateRInput;
    bool holdInput;

    void OnEnable()
    {
        inputReader.HardDropEvent += OnHardDrop;
        inputReader.SoftDropEvent += OnSoftDrop;
        inputReader.SoftDropCancelEvent += OnSoftDropCancel;
        inputReader.MoveEvent += OnMove;
        inputReader.RotateLEvent += OnRotateL;
        inputReader.RotateREvent += OnRotateR;
        inputReader.HoldEvent += OnHold;
    }

    void OnDisable()
    {
        inputReader.HardDropEvent -= OnHardDrop;
        inputReader.SoftDropEvent -= OnSoftDrop;
        inputReader.SoftDropCancelEvent -= OnSoftDropCancel;
        inputReader.MoveEvent -= OnMove;
        inputReader.RotateLEvent -= OnRotateL;
        inputReader.RotateREvent -= OnRotateR;
        inputReader.HoldEvent -= OnHold;
    }

    public void InputUpdate()
    {
        if (holdInput)
        {
            holdInput = false;
            piece.Hold();
        }

        if (rotateLInput)
        {
            rotateLInput = false;
            piece.Rotate(-1);
        }
        else if (rotateRInput)
        {
            rotateRInput = false;
            piece.Rotate(1);
        }

        if (moveInput.x < -0.2f)
        {
            if (bfrMoveInput != moveInput)
            {
                piece.ResetAllDASDelay();
            }
            bfrMoveInput = moveInput;
            piece.MoveWithDAS(Vector2Int.left);
        }
        else if (moveInput.x > 0.2f)
        {
            if (bfrMoveInput != moveInput)
            {
                piece.ResetAllDASDelay();
            }
            bfrMoveInput = moveInput;
            piece.MoveWithDAS(Vector2Int.right);
        }
        else
        {
            bfrMoveInput = Vector2Int.zero;
            piece.ResetAllDASDelay();
        }

        if (softDropInput)
        {
            piece.SoftDrop();
        }

        else if (hardDropInput)
        {
            hardDropInput = false;
            piece.HardDrop();
        }
    }

    #region Event Listeners
    void OnMove(Vector2 input)
    {
        moveInput = input;
    }

    void OnHardDrop() => hardDropInput = true;

    void OnSoftDrop()
    {
        softDropInput = true;

    }

    void OnSoftDropCancel()
    {
        softDropInput = false;
    }

    void OnRotateL() => rotateLInput = true;

    void OnRotateR() => rotateRInput = true;

    void OnHold() => holdInput = true;
    #endregion
}
