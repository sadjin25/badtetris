using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ScoreType
{
    Tetris = 0,
    TSpinSingle = 1,
    TSpinDouble = 2,
    TSpinTriple = 3,
    B2B = 4,
}

public class ScoreUIManager : MonoBehaviour
{
    public static ScoreUIManager Instance { get; private set; }

    [SerializeField] MinoUI scoreUI;

    void Awake()
    {
        if (Instance) Destroy(this);
        Instance = this;
    }

    void Start()
    {
        Board.Instance.OnScoring += OnScoring;
    }

    void OnScoring(ScoreType scoreType)
    {
        StartCoroutine(ShowingScoreCoroutine(scoreType));
    }
    IEnumerator ShowingScoreCoroutine(ScoreType scoreType)
    {
        scoreUI.SetImage(scoreType);
        yield return new WaitForSeconds(0.6f);
        scoreUI.Clear();
    }
}
