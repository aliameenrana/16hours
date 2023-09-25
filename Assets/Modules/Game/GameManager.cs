using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Accessibility;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Gameboard gameBoard;

    private void Awake()
    {
        instance = this;
    }

    public void StartGame()
    {
        gameBoard.StartGame(3, 4);
    }

    public void GameOver()
    {
        UIManager.instance.GameOver();
    }
}