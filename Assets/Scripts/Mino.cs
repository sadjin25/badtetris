using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public enum Mino
{
    I = 0,
    O = 1,
    T = 2,
    L = 3,
    J = 4,
    S = 5,
    Z = 6,
}

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