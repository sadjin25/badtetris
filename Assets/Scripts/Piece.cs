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

    void Update()
    {
        board.Clear(this);
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Rotate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            Rotate(1);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2Int.right);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Vector2Int.down);
        }
        board.Set(this);
    }

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

    bool Move(Vector2Int moveVec)
    {
        Vector3Int newPos = this.position + (Vector3Int)moveVec;

        bool valid = board.IsValidPosition(this, newPos);
        if (valid)
        {
            position = newPos;
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
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * rotateDir) + (cell.y * Data.RotationMatrix[1] * rotateDir));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * rotateDir) + (cell.y * Data.RotationMatrix[3] * rotateDir));
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

}
