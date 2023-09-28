using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tetris.EnumTypes;
using Tetris.CustomStructs;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] Board _board;
    public readonly RectInt _bounds = new RectInt(new Vector2Int(-5, -10), new Vector2Int(10, 20));

    public Piece _activePiece { get; private set; }
    [SerializeField] Vector3Int _spawnPos;

    MinoData _holdPieceData;
    bool _isHoldUsed;

    List<MinoData> _nextMinoDataList;         // Why List? => Add/Remove is easy.
    public static int _maxNextNum { get { return 5; } }

    void Awake()
    {
        if (Instance) Destroy(this);
        Instance = this;

        _activePiece = GetComponentInChildren<Piece>();
        _nextMinoDataList = new List<MinoData>();
    }

    void Start()
    {
        GameStart();
    }

    void GameStart()
    {
        for (int i = 0; i < _maxNextNum; ++i)
        {
            SpawnPieces();
        }

        SetActivePiece();
    }

    public void Set(Piece piece)
    {
        _board.SetTilesOnMap(piece);
    }

    public void Clear(Piece piece)
    {
        _board.ClearTilesOnMap(piece);
    }

    #region Basic Game Functions

    public void HoldPiece()
    {
        if (_isHoldUsed) return;

        _isHoldUsed = true;

        if (_holdPieceData.Equals(default(MinoData)))        // MinoData is struct, so it couldn't get null!
        {
            _holdPieceData = _activePiece._data;
            _activePiece.Init(_spawnPos, _nextMinoDataList[0]);
            _nextMinoDataList.Remove(_nextMinoDataList[0]);
            SpawnPieces();
        }
        else
        {
            MinoData activePieceDataCpy = _activePiece._data;
            _activePiece.Init(_spawnPos, _holdPieceData);
            _holdPieceData = activePieceDataCpy;
        }

        GameEventManager.Instance.InvokeHoldEvent(_holdPieceData);
    }

    public void ActivateHold()
    {
        _isHoldUsed = false;
    }

    public void SpawnPieces()
    {
        if (_nextMinoDataList.Count >= _maxNextNum) return;

        MinoData randMinoData = RandomGenerator.Instance.GetRandomMinoData();

        _nextMinoDataList.Add(randMinoData);

        if (_nextMinoDataList.Count >= _maxNextNum)
        {
            GameEventManager.Instance.InvokeNextMinoChangedEvent(_nextMinoDataList);
        }
    }

    public void SetActivePiece()
    {
        MinoData nextPieceData = _nextMinoDataList[0];
        _nextMinoDataList.Remove(_nextMinoDataList[0]);
        SpawnPieces();

        _activePiece.Init(_spawnPos, nextPieceData);
        if (!IsValidPosition(_activePiece, _spawnPos))
        {
            GameOver();
        }
        Set(_activePiece);
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        for (int i = 0; i < piece._cells.Length; i++)
        {
            Vector3Int tilePos = piece._cells[i] + position;

            if (!_bounds.Contains((Vector2Int)tilePos))
            {
                return false;
            }

            if (_board.HasTile(tilePos))
            {
                return false;
            }
        }
        return true;
    }

    public void ClearLines()
    {
        // naive Style
        int row = _bounds.yMin;
        int lineClearInARowCnt = 0;
        while (row < _bounds.yMax)
        {
            if (IsLineFull(row))
            {
                EachLineClear(row);
                PushLinesDown(row);
                ++lineClearInARowCnt;

                if (lineClearInARowCnt >= 4)
                {
                    GameEventManager.Instance.InvokeScoringEvent(ScoreType.Tetris);
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
        for (int col = _bounds.xMin; col < _bounds.xMax; ++col)
        {
            Vector3Int pos = new Vector3Int(col, row, 0);
            if (!_board.HasTile(pos))
            {
                return false;
            }
        }
        return true;
    }

    void EachLineClear(int row)
    {
        for (int col = _bounds.xMin; col < _bounds.xMax; ++col)
        {
            Vector3Int pos = new Vector3Int(col, row, 0);
            _board.SetTilesOnMap(pos, null);
        }
    }

    void PushLinesDown(int startRow)
    {
        for (int row = startRow; row < _bounds.yMax; ++row)
        {
            for (int col = _bounds.xMin; col < _bounds.xMax; ++col)
            {
                Vector3Int nextLineCellPos = new Vector3Int(col, row + 1, 0);
                _board.LowerTile(nextLineCellPos);
            }
        }
    }

    void GameOver()
    {
        _board.ClearAllTiles();
    }
    #endregion
}
