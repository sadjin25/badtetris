using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    [SerializeField] Tile _tile;
    [SerializeField] Piece _activePiece;

    public Tilemap _tilemap { get; private set; }
    public Vector3Int[] _cells { get; private set; }
    public Vector3Int _position { get; private set; }

    void Awake()
    {
        _tilemap = GetComponentInChildren<Tilemap>();
        _cells = new Vector3Int[4];
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
        for (int i = 0; i < _cells.Length; i++)
        {
            Vector3Int tilePos = _cells[i] + _position;
            _tilemap.SetTile(tilePos, null);
        }
    }

    void Copy()
    {
        for (int i = 0; i < _cells.Length; ++i)
        {
            _cells[i] = _activePiece._cells[i];
        }
        _position = _activePiece._position;
    }

    void Drop()
    {
        Vector3Int pos = _position;
        int cur = pos.y;
        int bottom = -Board.Instance._bounds.height / 2 - 1;
        GameManager.Instance.Clear(_activePiece);
        for (int row = cur; row >= bottom; --row)
        {
            pos.y = row;
            if (GameManager.Instance.IsValidPosition(_activePiece, pos))
            {
                _position = pos;
            }
            else
            {
                break;
            }
        }
        // original active piece is deleted, draw it again
        GameManager.Instance.Set(_activePiece);
    }

    void Set()
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            Vector3Int tilePos = _cells[i] + _position;
            _tilemap.SetTile(tilePos, _tile);
        }
    }
}
