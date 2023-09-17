using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    [SerializeField] Tile tile;
    [SerializeField] Piece activePiece;

    public Tilemap tilemap { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        cells = new Vector3Int[4];
    }

    void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    void Clear()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePos = cells[i] + position;
            this.tilemap.SetTile(tilePos, null);
        }
    }

    void Copy()
    {
        for (int i = 0; i < cells.Length; ++i)
        {
            cells[i] = activePiece.cells[i];
        }
        this.position = activePiece.position;
    }

    void Drop()
    {
        Vector3Int pos = this.position;
        int cur = pos.y;
        int bottom = -Board.Instance.bounds.height / 2 - 1;
        Board.Instance.Clear(activePiece);
        for (int row = cur; row >= bottom; --row)
        {
            pos.y = row;
            if (Board.Instance.IsValidPosition(activePiece, pos))
            {
                this.position = pos;
            }
            else
            {
                break;
            }
        }
        // original active piece is deleted, draw it again
        Board.Instance.Set(activePiece);
    }

    void Set()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePos = cells[i] + position;
            this.tilemap.SetTile(tilePos, tile);
        }
    }
}
