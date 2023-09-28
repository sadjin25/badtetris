using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tetris.EnumTypes;

public class ScoreUIManager : MonoBehaviour
{
    public static ScoreUIManager Instance { get; private set; }

    [SerializeField] MinoUI _scoreUI;
    [SerializeField] float _scoreImageShowingTime = 0.6f;
    int _score;

    void OnEnable()
    {
        GameEventManager.OnScoring += OnScoring;
    }

    void OnDisable()
    {
        GameEventManager.OnScoring -= OnScoring;
    }

    void Awake()
    {
        if (Instance) Destroy(this);
        Instance = this;
    }

    void OnScoring(object s, GameEventManager.OnScoringArgs e)
    {
        ScoreType scoreType = e.scoreType;
        AddScore(scoreType);
        _scoreUI.UpdateText(_score);
        StartCoroutine(ShowingScoreCoroutine(scoreType));
    }

    IEnumerator ShowingScoreCoroutine(ScoreType scoreType)
    {
        _scoreUI.SetImage(scoreType);
        yield return new WaitForSeconds(_scoreImageShowingTime);
        _scoreUI.ClearImage();
    }

    void AddScore(ScoreType scoreType)
    {
        if (scoreType == ScoreType.B2B)
        {
            _score += 250;
        }
        switch (scoreType)
        {
            case ScoreType.Tetris:
                _score += 1000;
                break;
            case ScoreType.TSpinSingle:
                _score += 500;
                break;
            case ScoreType.TSpinDouble:
                _score += 1000;
                break;
            case ScoreType.TSpinTriple:
                _score += 1500;
                break;
        }
    }
}
