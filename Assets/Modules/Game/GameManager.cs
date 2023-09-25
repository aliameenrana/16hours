using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Accessibility;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void StartGame()
    {
        Gameboard.instance.StartGame(3, 4);
    }

    public void GameOver()
    {
        UIManager.instance.GameOver();
        AudioManager.instance.LevelComplete();
    }

    public void SaveGame()
    {
        Gameboard.instance.SaveBoard();
        ScoreSystem.instance.SaveScore();
    }

    public void LoadGame() 
    {
        Gameboard.instance.LoadBoard();
        ScoreSystem.instance.LoadScore();
    }
}