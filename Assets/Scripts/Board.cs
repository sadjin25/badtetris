using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    [SerializeField] MinoData[] baseMinoes;
    [SerializeField] Vector3Int spawnPos;

    readonly RectInt bounds = new RectInt(new Vector2Int(-5, -10), new Vector2Int(10, 20));
    void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();
        for (int i = 0; i < this.baseMinoes.Length; i++)
        {
            this.baseMinoes[i].Init();
        }
    }

    void Start()
    {
        SpawnPieces();
    }

    public void SpawnPieces()
    {
        int rand = Random.Range(0, this.baseMinoes.Length);
        MinoData data = this.baseMinoes[rand];
        activePiece.Init(this, spawnPos, data);
        Set(activePiece);
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePos, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePos, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePos))
            {
                return false;
            }

            if (tilemap.HasTile(tilePos))
            {
                return false;
            }
        }
        return true;
    }

    public void ClearLines()
    {
        // naive Style
        int row = bounds.yMin;
        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                EachLineClear(row);
                PushLinesDown(row);
            }
            else
            {
                ++row;
            }
        }
    }

    bool IsLineFull(int row)
    {
        for (int col = bounds.xMin; col < bounds.xMax; ++col)
        {
            Vector3Int pos = new Vector3Int(col, row, 0);
            if (!tilemap.HasTile(pos))
            {
                return false;
            }
        }
        return true;
    }

    void EachLineClear(int row)
    {
        for (int col = bounds.xMin; col < bounds.xMax; ++col)
        {
            Vector3Int pos = new Vector3Int(col, row, 0);
            tilemap.SetTile(pos, null);
        }
    }

    void PushLinesDown(int startRow)
    {
        for (int row = startRow; row < bounds.yMax; ++row)
        {
            for (int col = bounds.xMin; col < bounds.xMax; ++col)
            {
                Vector3Int nextLineCellPos = new Vector3Int(col, row + 1, 0);
                TileBase aboveTile = tilemap.GetTile(nextLineCellPos);
                Vector3Int curCellPos = new Vector3Int(col, row, 0);
                tilemap.SetTile(curCellPos, aboveTile);
            }
        }
    }
}
