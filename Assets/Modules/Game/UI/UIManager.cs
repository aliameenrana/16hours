using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject scoreboardPanel;
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button playButton;

    private void Awake()
    {
        instance = this;

        AddCallbacks();
    }

    private void AddCallbacks()
    {
        playButton.onClick.AddListener(OnClickPlayButton);
        playAgainButton.onClick.AddListener(OnClickPlayAgainButton);
    }

    public void OnGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    //The functions for play again and play should be kept separate as game conditions in both can be different. 
    //Currently it is only that the gameOverPanel is active, but in future for example it can be some data that
    //needs to be handled and cleaned before starting the new game. So it is not a good idea to have a single function

    public void OnClickPlayAgainButton()
    {
        gameOverPanel.SetActive(false);
        StartGame();
    }

    public void OnClickPlayButton()
    {
        mainMenu.SetActive(false);
        scoreboardPanel.SetActive(true);
        StartGame();
    }

    public void StartGame()
    {
        GameManager.instance.StartGame();
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }
}