using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tetris.EnumTypes;
using System;

public class Piece : MonoBehaviour
{
    public Vector3Int position { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public MinoData data { get; private set; }
    public int rotateIndex { get; private set; }

    //--------------------Move Variables----------------
    [SerializeField] float stepDelay = 1f;
    [SerializeField] float lockDelay = .5f;
    [SerializeField] float softDropDelay = .15f;
    [SerializeField] float das = .3f;
    [SerializeField] float arr = 0f;

    float stepTime;
    float lockTime;
    float softDropTime;
    float dasChkTime;
    float arrChkTime;
    bool isFirstMoveTriggered;
    //-----------------------INPUTS-------------------------
    [SerializeField] PieceController pieceController;
    bool isHardDropping;
    bool isSoftDropping;
    RotateType rotateDir;
    MoveLRType moveDir;

    void Update()
    {
        Board.Instance.Clear(this);
        lockTime += Time.deltaTime;
        stepTime += Time.deltaTime;

        pieceController.InputUpdate();

        if (stepTime >= stepDelay)
        {
            ResetStepTime();
            Step();
        }

        Board.Instance.Set(this);
    }

    #region User Controls
    bool Move(Vector2Int moveVec)
    {
        Vector3Int newPos = this.position + (Vector3Int)moveVec;

        bool valid = Board.Instance.IsValidPosition(this, newPos);
        if (valid)
        {
            position = newPos;
            ResetLockTime();
        }
        return valid;
    }

    public void SoftDrop()
    {
        softDropTime += Time.deltaTime;
        if (softDropTime >= softDropDelay)
        {
            softDropTime = 0f;
            Move(Vector2Int.down);
        }
    }

    void ResetSoftDropDelay()
    {
        softDropTime = 0f;
    }

    public void MoveWithDAS(Vector2Int moveVec)
    {
        if (!isFirstMoveTriggered)
        {
            Move(moveVec);
            isFirstMoveTriggered = true;
        }

        dasChkTime += Time.deltaTime;
        if (dasChkTime >= das)
        {
            dasChkTime = das;
            arrChkTime += Time.deltaTime;
            if (arrChkTime >= arr)
            {
                arrChkTime = 0f;
                Move(moveVec);
            }
        }
    }

    public void ResetAllDASDelay()
    {
        dasChkTime = 0f;
        arrChkTime = 0f;
        isFirstMoveTriggered = false;
    }

    public void Rotate(int rotateDir)
    {
        ResetSoftDropDelay();

        int originalRot = this.rotateIndex;
        rotateIndex = Wrap(rotateIndex + rotateDir, 0, 4);

        ApplyRotation(rotateDir);

        if (!WallKickTest(originalRot, rotateIndex, rotateDir))
        {
            this.rotateIndex = originalRot;
            ApplyRotation(-rotateDir);
        }
    }

    void ApplyRotation(int rotateDir)
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cell = this.cells[i];
            int x = (int)cell.x, y = (int)cell.y;
            switch (this.data.mino)
            {
                case Mino.O:
                    break;
                case Mino.I:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * rotateDir) + (cell.y * Data.RotationMatrix[1] * rotateDir));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * rotateDir) + (cell.y * Data.RotationMatrix[3] * rotateDir));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * rotateDir) + (cell.y * Data.RotationMatrix[1] * rotateDir));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * rotateDir) + (cell.y * Data.RotationMatrix[3] * rotateDir));
                    break;
            }
            this.cells[i] = new Vector3Int(x, y, 0);
        }
    }

    void ResetRotationIndex()
    {
        rotateIndex = 0;
    }

    bool WallKickTest(int beforeRotateIndex, int afterRotateIndex, int rotateDir)
    {
        if (Move(Vector2Int.zero)) return true;

        int wallkickIndex = GetWallKickIndex(beforeRotateIndex, afterRotateIndex, rotateDir);
        for (int i = 0; i < this.data.wallkicks.GetLength(1); i++)
        {
            Vector2Int translation = this.data.wallkicks[wallkickIndex, i];
            if (Move(translation))
            {
                return true;
            }
        }
        return false;
    }

    int GetWallKickIndex(int beforeRotateIndex, int afterRotateIndex, int rotateDir)
    {
        int wallkickIndex = 0;
        if (rotateDir > 0)
        {
            wallkickIndex = beforeRotateIndex * 2;
        }
        else
        {
            wallkickIndex = afterRotateIndex * 2 + 1;
        }

        return wallkickIndex;
    }

    public void HardDrop()
    {
        ResetSoftDropDelay();

        while (Move(Vector2Int.down))
        {
            continue;
        }
        Lock();
    }

    public void Hold()
    {
        Board.Instance.HoldPiece();
    }

    #endregion

    #region Game Mechanic
    public void Init(Vector3Int position, MinoData data)
    {
        this.position = position;
        this.data = data;
        if (this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }

    int Wrap(int target, int min, int max)
    {
        //[min, max)
        if (target < min)
        {
            return max - 1;
        }
        if (target >= max)
        {
            return min;
        }
        return target;
    }

    void ResetLockTime()
    {
        lockTime = 0f;
    }

    void ResetStepTime()
    {
        stepTime = 0f;
    }

    void Step()
    {
        if (!Move(Vector2Int.down))
        {
            if (lockTime >= lockDelay)
            {
                ResetLockTime();
                Lock();
            }
        }
    }

    void Lock()
    {
        Board.Instance.Set(this);
        Board.Instance.ClearLines();
        Board.Instance.SetActivePiece();
        Board.Instance.ActivateHold();
        ResetRotationIndex();
    }
    #endregion
}
