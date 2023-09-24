using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField]private Image cardImage;
    private int id;

    public void SetProperties(CardProperties cardProperties) 
    {
        cardImage.sprite = cardProperties.sprite;
        id = cardProperties.id;
    }
}

public class CardProperties
{
    public int id;
    public Sprite sprite;
}