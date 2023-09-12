using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public Vector3Int position { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public MinoData data { get; private set; }
    public int rotateIndex { get; private set; }

    //--------------------DOWNSTEP TIMES----------------
    [SerializeField] float stepDelay = 1f;
    [SerializeField] float lockDelay = .5f;

    float stepTime;
    float lockTime;
    //-----------------------INPUTS-------------------------
    [SerializeField] InputReader inputReader;

    //WARNING : if some action function(rotate, harddrop.. etc) is used, then CLEAR input vars.
    //          Clearing always be performed in CheckInputActions()!
    Vector2 moveInput;
    bool hardDropInput;
    bool softDropInput;
    bool rotateLInput;
    bool rotateRInput;

    void OnEnable()
    {
        inputReader.HardDropEvent += OnHardDrop;
        inputReader.SoftDropEvent += OnSoftDrop;
        inputReader.SoftDropCancelEvent += OnSoftDropCancel;
        inputReader.MoveEvent += OnMove;
        inputReader.RotateLEvent += OnRotateL;
        inputReader.RotateREvent += OnRotateR;
    }

    void OnDisable()
    {
        inputReader.HardDropEvent -= OnHardDrop;
        inputReader.SoftDropEvent -= OnSoftDrop;
        inputReader.SoftDropCancelEvent -= OnSoftDropCancel;
        inputReader.MoveEvent -= OnMove;
        inputReader.RotateLEvent -= OnRotateL;
        inputReader.RotateREvent -= OnRotateR;
    }

    void Update()
    {
        board.Clear(this);
        lockTime += Time.deltaTime;
        stepTime += Time.deltaTime;

        CheckInputActions();

        if (stepTime >= stepDelay)
        {
            ResetStepTime();
            Step();
        }
        board.Set(this);
    }

    void CheckInputActions()
    {
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

        // TODO : When Move, use DAS/ARR!
        if (moveInput.x < 0.2f)
        {
            moveInput = new Vector2();
            Move(Vector2Int.left);
        }
        else if (moveInput.x > 0.2f)
        {
            moveInput = new Vector2();
            Move(Vector2Int.right);
        }

        else if (softDropInput)
        {
            softDropInput = false;
            Move(Vector2Int.down);
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

    void OnSoftDrop() => softDropInput = true;

    void OnSoftDropCancel() => softDropInput = false;

    void OnRotateL() => rotateLInput = true;

    void OnRotateR() => rotateRInput = true;
    #endregion

    #region Move/Rotate
    bool Move(Vector2Int moveVec)
    {
        Vector3Int newPos = this.position + (Vector3Int)moveVec;

        bool valid = board.IsValidPosition(this, newPos);
        if (valid)
        {
            position = newPos;
            ResetLockTime();
        }
        return valid;
    }


    void Rotate(int rotateDir)
    {
        int originalRot = this.rotateIndex;
        rotateIndex = Wrap(rotateIndex + rotateDir, 0, 4);

        ApplyRotation(rotateDir);

        if (!WallKicks(rotateIndex, rotateDir))
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

    bool WallKicks(int rotateIndex, int rotateDir)
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

    #endregion

    #region Game Mechanic
    public void Init(Board board, Vector3Int position, MinoData data)
    {
        this.board = board;
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
        board.Set(this);
        board.ClearLines();
        board.SetActivePiece();
    }

    void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }
        Lock();
    }
    #endregion
}
