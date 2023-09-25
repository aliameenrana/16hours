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
    }

    public void StartGame(int rows, int columns)
    {
        flippedCards = new List<Card>();
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
            //int randomIndex = Random.Range(0, 26);
            int randomIndex = GetUniqueRandomIndex();
            //Add the index twice since two of the cards will have the same image
            imageIndexesToFetch.Add(randomIndex);
            imageIndexesToFetch.Add(randomIndex);
        }

        UtilityFunctions.Shuffle(imageIndexesToFetch);
        scoreSystem.Init();
    }

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
}