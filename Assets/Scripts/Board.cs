using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }
    public Tilemap _tilemap { get; private set; }

    public readonly RectInt _bounds = new RectInt(new Vector2Int(-5, -10), new Vector2Int(10, 20));

    void Awake()
    {
        if (Instance) Destroy(this);
        Instance = this;

        _tilemap = GetComponentInChildren<Tilemap>();
    }

    public void SetTilesOnMap(Piece piece)
    {
        for (int i = 0; i < piece._cells.Length; i++)
        {
            Vector3Int tilePos = piece._cells[i] + piece._position;
            _tilemap.SetTile(tilePos, piece._data._tile);
        }
    }

    public void SetTilesOnMap(Vector3Int pos, TileBase tile)
    {
        _tilemap.SetTile(pos, tile);
    }

    public void ClearTilesOnMap(Piece piece)
    {
        for (int i = 0; i < piece._cells.Length; i++)
        {
            Vector3Int tilePos = piece._cells[i] + piece._position;
            _tilemap.SetTile(tilePos, null);
        }
    }

    public void ClearAllTiles()
    {
        _tilemap.ClearAllTiles();
    }

    public bool HasTile(Vector3Int tilePos)
    {
        return _tilemap.HasTile(tilePos);
    }

    public void LowerTile(Vector3Int tilePosToLower)
    {
        TileBase aboveTile = _tilemap.GetTile(tilePosToLower);
        Vector3Int curCellPos = new Vector3Int(tilePosToLower.x, tilePosToLower.y - 1, 0);
        _tilemap.SetTile(curCellPos, aboveTile);
    }

    public bool CheckPositionValid(Vector3Int position)
    {
        if (_bounds.Contains((Vector2Int)position))
        {
            return true;
        }

        return false;
    }
}
