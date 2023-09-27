using UnityEngine;
using UnityEngine.Tilemaps;
using Tetris.EnumTypes;

[System.Serializable]
public struct MinoData
{
    public Mino mino;
    public Tile tile;
    public Vector2Int[] cells { get; private set; }
    public Vector2Int[,] wallkicks { get; private set; }

    public void Init()
    {
        this.cells = Data.Cells[this.mino];
        this.wallkicks = Data.WallKicks[this.mino];
    }
}