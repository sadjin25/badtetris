using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;       // for unityaction
using Tetris.EnumTypes;

// TODO : event to unityaction, or vise versa - why??

public class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    [SerializeField] Vector3Int spawnPos;

    MinoData holdPieceData;
    bool isHoldUsed;

    public UnityAction<MinoData> HoldEvent = delegate { };

    List<MinoData> nextMinoDataList;         // Why List? => Add/Remove is easy.
    public static int maxNextNum { get { return 5; } }

    public event EventHandler<OnNextMinoChangedArgs> OnNextMinoChanged;
    public class OnNextMinoChangedArgs : EventArgs
    {
        public List<MinoData> nextMinoDataList;
    }

    public event EventHandler<OnActiveMinoChangedArgs> OnActiveMinoChanged;
    public class OnActiveMinoChangedArgs : EventArgs
    {
        public Piece activePiece;
    }

    public UnityAction<ScoreType> OnScoring = delegate { };

    public readonly RectInt bounds = new RectInt(new Vector2Int(-5, -10), new Vector2Int(10, 20));

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        nextMinoDataList = new List<MinoData>();
        tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();
    }

    void Start()
    {
        for (int i = 0; i < maxNextNum; ++i)
        {
            SpawnPieces();
        }

        SetActivePiece();
    }

    #region Basic Game Functions

    public void HoldPiece()
    {
        if (isHoldUsed) return;

        isHoldUsed = true;

        if (holdPieceData.Equals(default(MinoData)))        // MinoData is struct, so it couldn't get null!
        {
            holdPieceData = activePiece.data;
            activePiece.Init(spawnPos, nextMinoDataList[0]);
            nextMinoDataList.Remove(nextMinoDataList[0]);
            SpawnPieces();
        }
        else
        {
            MinoData activePieceDataCpy = activePiece.data;
            activePiece.Init(spawnPos, holdPieceData);
            holdPieceData = activePieceDataCpy;
        }

        HoldEvent?.Invoke(holdPieceData);
    }

    public void ActivateHold()
    {
        isHoldUsed = false;
    }

    public void SpawnPieces()
    {
        if (nextMinoDataList.Count >= maxNextNum) return;

        MinoData randMinoData = RandomGenerator.Instance.GetRandomMinoData();

        nextMinoDataList.Add(randMinoData);

        if (nextMinoDataList.Count >= maxNextNum)
        {
            OnNextMinoChanged?.Invoke(this, new OnNextMinoChangedArgs { nextMinoDataList = this.nextMinoDataList });
        }
    }

    public void SetActivePiece()
    {
        MinoData nextPieceData = nextMinoDataList[0];
        nextMinoDataList.Remove(nextMinoDataList[0]);
        SpawnPieces();

        activePiece.Init(spawnPos, nextPieceData);
        if (!IsValidPosition(activePiece, spawnPos))
        {
            GameOver();
        }
        Set(activePiece);

        OnActiveMinoChanged?.Invoke(this, new OnActiveMinoChangedArgs { activePiece = activePiece });
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
        int lineClearInARowCnt = 0;
        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                EachLineClear(row);
                PushLinesDown(row);
                ++lineClearInARowCnt;

                if (lineClearInARowCnt >= 4)
                {
                    OnScoring?.Invoke(ScoreType.Tetris);
                }
            }
            else
            {
                ++row;
                lineClearInARowCnt = 0;
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

    void GameOver()
    {
        tilemap.ClearAllTiles();
    }
    #endregion
}
