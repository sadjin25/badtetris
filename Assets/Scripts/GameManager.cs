using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;       // for unityaction
using Tetris.EnumTypes;
using System;

// TODO : event to unityaction, or vise versa - why??

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] Board board;
    public readonly RectInt bounds = new RectInt(new Vector2Int(-5, -10), new Vector2Int(10, 20));

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

    void Awake()
    {
        if (Instance) Destroy(this);
        Instance = this;

        this.activePiece = GetComponentInChildren<Piece>();
        nextMinoDataList = new List<MinoData>();
    }

    void Start()
    {
        NextUIManager.Instance.NextUIInit();
        HoldUIManager.Instance.HoldUIInit();
        ScoreUIManager.Instance.ScoreUIInit();
        GameStart();
    }

    void GameStart()
    {
        for (int i = 0; i < maxNextNum; ++i)
        {
            SpawnPieces();
        }

        SetActivePiece();
    }

    public void Set(Piece piece)
    {
        board.SetTilesOnMap(piece);
    }

    public void Clear(Piece piece)
    {
        board.ClearTilesOnMap(piece);
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

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePos))
            {
                return false;
            }

            if (board.HasTile(tilePos))
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
            if (!board.HasTile(pos))
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
            board.SetTilesOnMap(pos, null);
        }
    }

    void PushLinesDown(int startRow)
    {
        for (int row = startRow; row < bounds.yMax; ++row)
        {
            for (int col = bounds.xMin; col < bounds.xMax; ++col)
            {
                Vector3Int nextLineCellPos = new Vector3Int(col, row + 1, 0);
                board.LowerTile(nextLineCellPos);
            }
        }
    }

    void GameOver()
    {
        board.ClearAllTiles();
    }
    #endregion
}
