namespace Tetris.EnumTypes
{
    [System.Serializable]
    public enum Mino
    {
        I = 0,
        O = 1,
        T = 2,
        L = 3,
        J = 4,
        S = 5,
        Z = 6,
    }

    [System.Serializable]
    public enum ScoreType
    {
        Tetris = 0,
        TSpinSingle = 1,
        TSpinDouble = 2,
        TSpinTriple = 3,
        B2B = 4,
    }
}

