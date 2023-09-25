using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;

    [SerializeField] private TextMeshProUGUI toastText;
    [SerializeField] private CanvasGroup toastCanvasGroup;

    private void Awake()
    {
        instance = this;

        AddCallbacks();
    }

    private void AddCallbacks()
    {
        playButton.onClick.AddListener(OnClickPlayButton);
        playAgainButton.onClick.AddListener(OnClickPlayAgainButton);
        saveButton.onClick.AddListener(OnClickSaveButton);
        loadButton.onClick.AddListener(OnClickLoadButton);
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

    public void OnClickSaveButton()
    {
        GameManager.instance.SaveGame();
    }

    public void OnClickLoadButton() 
    {
        mainMenu.SetActive(false);
        scoreboardPanel.SetActive(true);
        GameManager.instance.LoadGame();
    }

    public void StartGame()
    {
        GameManager.instance.StartGame();
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void ShowToast(string message)
    {
        StartCoroutine(ShowToastCoroutine(message));
    }

    IEnumerator ShowToastCoroutine(string message)
    {
        toastText.text = message;
        
        while (toastCanvasGroup.alpha < 1.0f)
        {
            toastCanvasGroup.alpha += Time.deltaTime * 1.5f;
            yield return null;
        }
        toastCanvasGroup.alpha = 1.0f;
        yield return new WaitForSeconds(2.0f);
        while (toastCanvasGroup.alpha > 0.0f)
        {
            toastCanvasGroup.alpha -= Time.deltaTime * 1.5f;
            yield return null;
        }
        toastCanvasGroup.alpha = 0.0f;
    }
}