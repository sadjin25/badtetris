using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tetris.EnumTypes;
using Tetris.CustomStructs;

public class Piece : MonoBehaviour
{
    public Vector3Int _position { get; private set; }
    public Vector3Int[] _cells { get; private set; }
    public MinoData _data { get; private set; }
    public int _rotateIndex { get; private set; }

    //--------------------Move Variables----------------
    [SerializeField] float _stepDelay = 1f;
    [SerializeField] float _lockDelay = .5f;
    [SerializeField] float _softDropDelay = .025f;
    [SerializeField] float _das = .3f;
    [SerializeField] float _arr = 0f;

    float _stepTime;
    float _lockTime;
    float _softDropTime;
    float _dasChkTime;
    float _arrChkTime;
    bool _isFirstMoveTriggered;
    //-----------------------INPUTS-------------------------
    [SerializeField] PieceController _pieceController;
    bool _isHardDropping;
    bool _isSoftDropping;
    RotateType _rotateDir;
    MoveLRType _moveDir;

    void Update()
    {
        GameManager.Instance.Clear(this);
        _lockTime += Time.deltaTime;
        _stepTime += Time.deltaTime;

        _pieceController.InputUpdate();

        if (_stepTime >= _stepDelay)
        {
            ResetStepTime();
            Step();
        }

        GameManager.Instance.Set(this);
    }

    #region User Controls
    bool Move(Vector2Int moveVec)
    {
        Vector3Int newPos = _position + (Vector3Int)moveVec;

        bool valid = GameManager.Instance.IsValidPosition(this, newPos);
        if (valid)
        {
            _position = newPos;
            ResetLockTime();
            GameManager.Instance.DeactivateTSpinReward();
        }
        return valid;
    }

    public void SoftDrop()
    {
        _softDropTime += Time.deltaTime;
        if (_softDropTime >= _softDropDelay)
        {
            _softDropTime = 0f;
            Move(Vector2Int.down);
        }
    }

    void ResetSoftDropDelay()
    {
        _softDropTime = 0f;
    }

    public void MoveWithDAS(Vector2Int moveVec)
    {
        if (!_isFirstMoveTriggered)
        {
            Move(moveVec);
            _isFirstMoveTriggered = true;
        }

        _dasChkTime += Time.deltaTime;
        if (_dasChkTime >= _das)
        {
            _dasChkTime = _das;
            _arrChkTime += Time.deltaTime;
            if (_arrChkTime >= _arr)
            {
                _arrChkTime = 0f;
                Move(moveVec);
            }
        }
    }

    public void ResetAllDASDelay()
    {
        _dasChkTime = 0f;
        _arrChkTime = 0f;
        _isFirstMoveTriggered = false;
    }

    public void Rotate(int rotateDir)
    {
        ResetSoftDropDelay();

        int originalRot = _rotateIndex;
        _rotateIndex = Wrap(_rotateIndex + rotateDir, 0, 4);

        ApplyRotation(rotateDir);

        if (!WallKickTest(originalRot, _rotateIndex, rotateDir))
        {
            _rotateIndex = originalRot;
            ApplyRotation(-rotateDir);
        }
        else
        {
            GameManager.Instance.ActivateTSpinReward();
        }
    }

    void ApplyRotation(int rotateDir)
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            Vector3 cell = _cells[i];
            int x = (int)cell.x, y = (int)cell.y;
            switch (_data._mino)
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
            _cells[i] = new Vector3Int(x, y, 0);
        }
    }

    void ResetRotationIndex()
    {
        _rotateIndex = 0;
    }

    bool WallKickTest(int beforeRotateIndex, int afterRotateIndex, int rotateDir)
    {
        if (Move(Vector2Int.zero)) return true;

        int wallkickIndex = GetWallKickIndex(beforeRotateIndex, afterRotateIndex, rotateDir);
        for (int i = 0; i < _data._wallkicks.GetLength(1); i++)
        {
            Vector2Int translation = _data._wallkicks[wallkickIndex, i];
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
        GameManager.Instance.HoldPiece();
    }

    #endregion

    #region Game Mechanic
    public void Init(Vector3Int position, MinoData data)
    {
        _position = position;
        _data = data;
        if (_cells == null)
        {
            _cells = new Vector3Int[_data._cells.Length];
        }

        for (int i = 0; i < data._cells.Length; i++)
        {
            _cells[i] = (Vector3Int)_data._cells[i];
        }
    }

    // TODO : Remove this func from Piece.cs
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
        _lockTime = 0f;
    }

    void ResetStepTime()
    {
        _stepTime = 0f;
    }

    void Step()
    {
        if (!Move(Vector2Int.down))
        {
            if (_lockTime >= _lockDelay)
            {
                ResetLockTime();
                Lock();
            }
        }
    }

    void Lock()
    {
        GameManager.Instance.Set(this);
        GameManager.Instance.ClearLines();
        GameManager.Instance.SetActivePiece();
        GameManager.Instance.ActivateHold();
        ResetRotationIndex();
    }

    public bool CheckTMinoDiagBlockOccupied()
    {
        if (_data._mino != Mino.T) return false;

        int[] dx = { 1, 1, -1, -1 };
        int[] dy = { 1, -1, -1, 1 };
        int occupiedDiagCnt = 0;

        for (int i = 0; i < 4; ++i)
        {
            Vector3Int posToChk = _position;
            posToChk.x += dx[i];
            posToChk.y += dy[i];

            if (Board.Instance.CheckPositionValid(posToChk))
            {
                // This is considered to be occupied
                ++occupiedDiagCnt;
                continue;
            }
            else if (Board.Instance.HasTile(posToChk))
            {
                ++occupiedDiagCnt;
            }
        }
        return occupiedDiagCnt >= 3;
    }
    #endregion
}
