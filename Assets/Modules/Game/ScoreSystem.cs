using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem instance;

    [SerializeField] private TextMeshProUGUI turnsAmountText;
    [SerializeField] private TextMeshProUGUI scoreAmountText;

    private int turns;
    private int score;

    private void Awake()
    {
        instance = this;
    }

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

    public void SaveScore()
    {
        PlayerPrefs.SetInt(PlayerPrefsKeys.Score, score);
        PlayerPrefs.SetInt(PlayerPrefsKeys.Turns, turns);
    }

    public void LoadScore()
    {
        score = PlayerPrefs.GetInt(PlayerPrefsKeys.Score, 0);
        turns = PlayerPrefs.GetInt(PlayerPrefsKeys.Turns, 0);

        UpdateScore();
    }
}