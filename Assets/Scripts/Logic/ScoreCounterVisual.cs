using System;
using Modules.Score;
using TMPro;
using UnityEngine;

public class ScoreCounterVisual : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI output;
    [SerializeField] private int numberOfCharacters = 2;

    private ScoreCounter scoreCounter;

    public TextMeshProUGUI Output => output;

    public void SetScoreCounter(ScoreCounter scoreCounter)
    {
        if (this.scoreCounter != null) Unsubscribe();
        this.scoreCounter = scoreCounter;
        Subscribe();
        UpdateScore(scoreCounter.Value);
    }

    public void UpdateScore(float value)
    {
        output.text = $"{Math.Round(value, numberOfCharacters)}";
    }

    private void Subscribe()
    { 
        scoreCounter.OnChangeValue += UpdateScore;
    }

    private void Unsubscribe()
    {
        scoreCounter.OnChangeValue -= UpdateScore;
    }
}
