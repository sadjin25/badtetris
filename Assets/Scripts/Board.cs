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
        Debug.Log(data.mino.ToString());
        activePiece.Init(this, spawnPos, data);
        Set(activePiece);
    }

    void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePos, piece.data.tile);
            Debug.Log("DRAW" + i.ToString());
        }
    }

}
