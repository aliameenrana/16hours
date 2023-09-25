using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gameboard : MonoBehaviour
{
    public static List<Card> flippedCards;
    public static Gameboard instance;

    [SerializeField] private GameData gameData;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private RectTransform cardsParent;

    private int rows;
    private int columns;
    private float spacing = 50.0f;
    private int cardsOnBoard = 0;
    
    private Card[,] cards;
    private List<int> imageIndexesToFetch;

    private ScoreSystem scoreSystem;

    private void Awake()
    {
        instance = this;
        scoreSystem = GetComponent<ScoreSystem>();
        flippedCards = new List<Card>();
    }

    public void StartGame(int rows, int columns)
    {
        flippedCards.Clear();
        this.rows = rows;
        this.columns = columns;

        Init();
        StartCoroutine(CreateCards());
    }

    /// <summary>
    /// Choose the image indexes that are to be used for this game session. 
    /// Indexes can range from 0 to 25 as there are 26 unique pictures in the game.
    /// </summary>
    private void Init()
    {
        imageIndexesToFetch = new List<int>();
        cards = new Card[rows, columns];

        int totalCards = rows * columns;

        for (int i = 0; i < totalCards / 2; i++)
        {
            int randomIndex = GetUniqueRandomIndex();
            //Add the index twice since two of the cards will have the same image
            imageIndexesToFetch.Add(randomIndex);
            imageIndexesToFetch.Add(randomIndex);
        }

        UtilityFunctions.Shuffle(imageIndexesToFetch);
        scoreSystem.Init();
    }

    /// <summary>
    /// Returns a unique random number from 0 to 26
    /// </summary>
    /// <returns></returns>
    private int GetUniqueRandomIndex()
    {
        int randomIndex = Random.Range(0, 26);
        if (imageIndexesToFetch.Contains(randomIndex))
        {
            return GetUniqueRandomIndex();
        }
        else
        {
            return randomIndex;
        }
    }

    /// <summary>
    /// The coroutine that spawns the cards on the board. 
    /// It takes into account the dimensions of the parent rectTransform and adjusts the size of cards accordingly.
    /// </summary>
    /// <returns></returns>
    IEnumerator CreateCards()
    {
        GameObject cardObject;
        int cardsMade = 0;
        cardsOnBoard = 0;
        // Get the size of the parent
        float parentWidth = cardsParent.rect.width; 
        float parentHeight = cardsParent.rect.height; 
        
        // Calculate the size of each cell
        float cellWidth = (parentWidth - (spacing * (columns - 1))) / columns; 
        float cellHeight = (parentHeight - (spacing * (rows - 1))) / rows;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                CardProperties cardProperties = new CardProperties();
                cardProperties.sprite = gameData.pictures[imageIndexesToFetch[cardsMade]];
                cardProperties.id = imageIndexesToFetch[cardsMade];
                cardObject = Instantiate(cardPrefab, cardsParent);

                RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(cellWidth, cellHeight);
                rectTransform.anchoredPosition = new Vector2(j * (cellWidth + spacing), -i * (cellHeight + spacing));

                Card card = cardObject.GetComponent<Card>();
                card.SetProperties(cardProperties);
                cards[i,j] = card;

                cardsMade++;
                cardsOnBoard++;
                yield return new WaitForSeconds(0.2f);
            }
        }

        foreach (var card in cards)
        {
            card.FlipUp();
        }

        yield return new WaitForSeconds(3.0f);

        foreach (var card in cards)
        {
            card.FlipDown();
        }
    }

    /// <summary>
    /// If we have some cards flipped, check if the card we just flipped is one of them. If yes, destroy both and add to score. 
    /// If not, Un-flip all cards back to their natural state
    /// </summary>
    /// <param name="card"></param>
    public void OnCardFlipped(Card card)
    {
        if (flippedCards.Count > 0) 
        {
            Card sameCard = flippedCards.Find(o => o.Equals(card));
            if (sameCard != null) 
            {
                sameCard.ScheduleDestruction();
                card.ScheduleDestruction();
                flippedCards.Remove(sameCard);
                scoreSystem.RecordScore(1);
            }
            else
            {
                card.FlipDown();
                foreach (var flippedCard in flippedCards)
                {
                    flippedCard.FlipDown();
                }
                flippedCards.Clear();
                scoreSystem.RecordScore(0);
            }
        }
        else
        {
            flippedCards.Add(card);
        }
    }

    public void UnRegisterCard()
    {
        cardsOnBoard -= 1;
        CheckEnd();
    }

    private void CheckEnd()
    {
        if (cardsOnBoard == 0) 
        {
            GameManager.instance.GameOver();
        }
    }

    public void SaveBoard()
    {
        string boardString = "";
        for(int i = 0; i < rows; i++) 
        {
            for (int j = 0; j < columns; j++)
            {
                boardString += GetString(cards[i, j]);
                if (j < columns - 1)
                {
                    boardString += ",";
                }
            }

            if (i < rows - 1)
            {
                boardString += "\n";
            }
        }
        PlayerPrefs.SetString(PlayerPrefsKeys.Board, boardString);
        UIManager.instance.ShowToast("Game successfully saved");
    }

    public void LoadBoard()
    {
        string boardString = PlayerPrefs.GetString(PlayerPrefsKeys.Board, "");
        
        if (boardString == "") 
        {
            Debug.LogError("Error: No saved game found");
            UIManager.instance.ShowToast("Error: No saved game found");
        }
        else
        {
            int[,] cardIndexes = StringToArray(boardString);
            rows = cardIndexes.GetLength(0);
            columns = cardIndexes.GetLength(1);

            if (cards != null) 
            {
                foreach (var item in cards)
                {
                    Destroy(item.gameObject);
                }
            }

            StartCoroutine(CreateLoadedCards(cardIndexes));
        }
    }

    IEnumerator CreateLoadedCards(int[,] cardIndexes)
    {
        GameObject cardObject;

        cardsOnBoard = 0;

        if (cards == null)
        {
            cards = new Card[rows, columns];
        }
        
        // Get the size of the parent
        float parentWidth = cardsParent.rect.width;
        float parentHeight = cardsParent.rect.height;

        // Calculate the size of each cell
        float cellWidth = (parentWidth - (spacing * (columns - 1))) / columns;
        float cellHeight = (parentHeight - (spacing * (rows - 1))) / rows;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                CardProperties cardProperties = new CardProperties();
                int index = cardIndexes[i, j];
                
                if (index < 0)
                    continue;

                cardProperties.sprite = gameData.pictures[cardIndexes[i, j]];
                cardProperties.id = cardIndexes[i, j];
                cardObject = Instantiate(cardPrefab, cardsParent);

                RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(cellWidth, cellHeight);
                rectTransform.anchoredPosition = new Vector2(j * (cellWidth + spacing), -i * (cellHeight + spacing));

                Card card = cardObject.GetComponent<Card>();
                card.SetProperties(cardProperties);
                cards[i, j] = card;

                cardsOnBoard++;
                yield return new WaitForSeconds(0.2f);
            }
        }

        UIManager.instance.ShowToast("Game successfully loaded");
    }

    // Parse a string back into a 2D integer array
    private int[,] StringToArray(string arrayString)
    {
        string[] rows = arrayString.Split('\n');
        int rowsCount = rows.Length;
        int colsCount = rows[0].Split(',').Length;

        int[,] newArray = new int[rowsCount, colsCount];

        for (int i = 0; i < rowsCount; i++)
        {
            string[] elements = rows[i].Split(',');
            for (int j = 0; j < colsCount; j++)
            {
                newArray[i, j] = int.Parse(elements[j]);
            }
        }

        return newArray;
    }

    private string GetString(Card card)
    {
        if (card == null) 
        {
            return "-1";
        }
        else
        {
            return card.GetID().ToString();
        }
    }
}