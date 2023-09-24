using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gameboard : MonoBehaviour
{
    public GameData gameData;
    public GameObject cardPrefab;

    [SerializeField] private RectTransform cardsParent;

    private int rows;
    private int columns;

    private float spacing = 25.0f;
    
    private Card[,] cards;

    private List<int> imageIndexesToFetch;
    
    private void Start()
    {
        //TODO
        //Make this a proper function 
        rows = 3;
        columns = 4;
        
        InitializeGameboard();
    }

    /// <summary>
    /// Choose the image indexes that are to be used for this game session. 
    /// Indexes can range from 0 to 25 as there are 26 unique pictures in the game.
    /// </summary>
    public void InitializeGameboard()
    {
        imageIndexesToFetch = new List<int>();
        cards = new Card[rows, columns];

        int totalCards = rows * columns;

        for (int i = 0; i < totalCards / 2; i++)
        {
            int randomIndex = Random.Range(0, 26);
            //Add the index twice since two of the cards will have the same image
            imageIndexesToFetch.Add(randomIndex);
            imageIndexesToFetch.Add(randomIndex);
        }

        UtilityFunctions.Shuffle(imageIndexesToFetch);
        StartCoroutine(CreateCards());
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
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}