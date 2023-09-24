using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Gameboard gameBoard;

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        gameBoard.StartGame(3, 4);
    }
}