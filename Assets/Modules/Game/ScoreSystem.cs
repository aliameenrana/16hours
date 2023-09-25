using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnsAmountText;
    [SerializeField] private TextMeshProUGUI scoreAmountText;

    private int turns;
    private int score;

    public void RecordScore(int score)
    {
        turns++;
        this.score += score;

        UpdateScore();
    }

    public void Init()
    {
        turns = 0;
        score = 0;
        UpdateScore();
    }

    private void UpdateScore()
    {
        turnsAmountText.text = turns.ToString();
        scoreAmountText.text = score.ToString();
    }
}