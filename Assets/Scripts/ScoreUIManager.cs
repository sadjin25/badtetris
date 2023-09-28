using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tetris.EnumTypes;

public class ScoreUIManager : MonoBehaviour
{
    public static ScoreUIManager Instance { get; private set; }

    [SerializeField] MinoUI scoreUI;

    void Awake()
    {
        if (Instance) Destroy(this);
        Instance = this;
    }

    public void ScoreUIInit()
    {
        GameManager.OnScoring += OnScoring;
    }

    void OnScoring(object s, GameManager.OnScoringArgs e)
    {
        StartCoroutine(ShowingScoreCoroutine(e.scoreType));
    }

    IEnumerator ShowingScoreCoroutine(ScoreType scoreType)
    {
        scoreUI.SetImage(scoreType);
        yield return new WaitForSeconds(0.6f);
        scoreUI.Clear();
    }
}
