using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }

    void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
    }

    public void SetTilesOnMap(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePos, piece.data.tile);
        }
    }

    public void SetTilesOnMap(Vector3Int pos, TileBase tile)
    {
        tilemap.SetTile(pos, tile);
    }

    public void ClearTilesOnMap(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePos, null);
        }
    }

    public void ClearAllTiles()
    {
        tilemap.ClearAllTiles();
    }

    public bool HasTile(Vector3Int tilePos)
    {
        return tilemap.HasTile(tilePos);
    }

    public void LowerTile(Vector3Int tilePosToLower)
    {
        TileBase aboveTile = tilemap.GetTile(tilePosToLower);
        Vector3Int curCellPos = new Vector3Int(tilePosToLower.x, tilePosToLower.y - 1, 0);
        tilemap.SetTile(curCellPos, aboveTile);
    }

}
