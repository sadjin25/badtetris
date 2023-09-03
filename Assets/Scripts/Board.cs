using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    [SerializeField] MinoData[] minoes;

    void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();

        for (int i = 0; i < this.minoes.Length; i++)
        {
            this.minoes[i].Init();
        }
    }

    void Start()
    {
        SpawnPieces();
    }

    public void SpawnPieces()
    {
        int rand = Random.Range(0, this.minoes.Length);
        MinoData data = this.minoes[rand];
    }

    void Set()
    {
        
    }

}
