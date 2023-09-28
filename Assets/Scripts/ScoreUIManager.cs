using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tetris.EnumTypes;

public class ScoreUIManager : MonoBehaviour
{
    public static ScoreUIManager Instance { get; private set; }

    [SerializeField] MinoUI _scoreUI;

    void Awake()
    {
        if (Instance) Destroy(this);
        Instance = this;
    }

    public void ScoreUIInit()
    {
        GameEventManager.OnScoring += OnScoring;
    }

    void OnScoring(object s, GameEventManager.OnScoringArgs e)
    {
        StartCoroutine(ShowingScoreCoroutine(e.scoreType));
    }

    IEnumerator ShowingScoreCoroutine(ScoreType scoreType)
    {
        _scoreUI.SetImage(scoreType);
        yield return new WaitForSeconds(0.6f);
        _scoreUI.Clear();
    }
}
