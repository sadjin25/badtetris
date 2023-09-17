using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    [SerializeField] InputReader inputReader;

    //WARNING : if some action function(rotate, harddrop.. etc) is used, then CLEAR input vars.
    //          Clearing always be performed in CheckInputActions()!
    Vector2 moveInput;
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

    void Update()
    {
        Board.Instance.Clear(this);
        lockTime += Time.deltaTime;
        stepTime += Time.deltaTime;

        CheckInputActions();

        if (stepTime >= stepDelay)
        {
            ResetStepTime();
            Step();
        }
        Board.Instance.Set(this);
    }

    void CheckInputActions()
    {
        if (holdInput)
        {
            holdInput = false;
            Hold();
        }

        if (rotateLInput)
        {
            rotateLInput = false;
            Rotate(-1);
        }
        else if (rotateRInput)
        {
            rotateRInput = false;
            Rotate(1);
        }

        if (moveInput.x < -0.2f)
        {
            MoveWithDAS(Vector2Int.left);
        }
        else if (moveInput.x > 0.2f)
        {
            MoveWithDAS(Vector2Int.right);
        }
        else
        {
            ResetDASDelay();
        }

        if (softDropInput)
        {
            SoftDrop();
        }

        else if (hardDropInput)
        {
            hardDropInput = false;
            HardDrop();
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

    void SoftDrop()
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

    void MoveWithDAS(Vector2Int moveVec)
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

    void ResetDASDelay()
    {
        dasChkTime = 0f;
        arrChkTime = 0f;
        isFirstMoveTriggered = false;
    }

    void Rotate(int rotateDir)
    {
        ResetSoftDropDelay();

        int originalRot = this.rotateIndex;
        rotateIndex = Wrap(rotateIndex + rotateDir, 0, 4);

        ApplyRotation(rotateDir);

        if (!WallKickTest(rotateIndex, rotateDir))
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

    bool WallKickTest(int rotateIndex, int rotateDir)
    {
        int wallkickIndex = GetWallKickIndex(rotateIndex, rotateDir);
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

    int GetWallKickIndex(int rotateIndex, int rotateDir)
    {
        int wallkickIndex = rotateIndex * 2;

        if (rotateDir < 0)
        {
            wallkickIndex--;
        }
        return Wrap(wallkickIndex, 0, this.data.wallkicks.GetLength(0));
    }

    void HardDrop()
    {
        ResetSoftDropDelay();

        while (Move(Vector2Int.down))
        {
            continue;
        }
        Lock();
    }

    void Hold()
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
    }
    #endregion
}
