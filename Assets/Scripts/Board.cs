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
            Debug.Log("TILE" + i + " POS " + tilePos);

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
}
