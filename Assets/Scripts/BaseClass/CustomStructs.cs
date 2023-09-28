using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Tetris.EnumTypes;
using TMPro;

namespace Tetris.CustomStructs
{
    [System.Serializable]
    public struct MinoData
    {
        public Mino _mino;
        public Tile _tile;
        public Vector2Int[] _cells { get; private set; }
        public Vector2Int[,] _wallkicks { get; private set; }

        public void Init()
        {
            _cells = Data.Cells[_mino];
            _wallkicks = Data.WallKicks[_mino];
        }
    }

    [System.Serializable]
    public struct MinoUISet
    {
        public Mino _minoType;
        public Image _image;
        public TextMeshProUGUI _text;
    }
}