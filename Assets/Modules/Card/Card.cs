using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField]private Image cardImage;
    [SerializeField]private Image cardBackImage;

    private CardProperties cardProperties;

    private bool isFlipped = false;

    public void SetProperties(CardProperties cardProperties) 
    {
        cardImage.sprite = cardProperties.sprite;
        this.cardProperties = cardProperties;
    }

    /// <summary>
    /// Flip the card to show the picture
    /// </summary>
    public void FlipUp()
    {
        cardBackImage.enabled = false;
    }

    /// <summary>
    /// Flip the card to hide the picture
    /// </summary>
    public void FlipDown()
    {
        cardBackImage.enabled = true;
        isFlipped = false;
    }
    
    /// <summary>
    /// Makes sure an already flipped card isn't flipped again
    /// </summary>
    public void OnClickCard()
    {
        if (!isFlipped) 
        {
            FlipUp();
            isFlipped = true;
            Gameboard.instance.OnCardFlipped(this);
        }
    }

    public int GetID()
    {
        return cardProperties.id;
    }

    public bool Equals(Card x)
    {
        return (x.cardProperties.id == cardProperties.id);
    }

    public void ScheduleDestruction()
    {
        StartCoroutine(Destroy());
    }

    /// <summary>
    /// Blinking animation before destroying the card
    /// </summary>
    /// <returns></returns>
    IEnumerator Destroy()
    {
        float timer = 2.0f;
        bool status = cardImage.enabled;

        //Show the matching image for 0.25 seconds before disappearing
        yield return new WaitForSeconds(0.25f);

        while(timer > 0.0f) 
        {
            cardImage.enabled = !status;
            status = cardImage.enabled;
            timer -= 0.25f;
            yield return new WaitForSeconds(0.25f);
        }
        Gameboard.instance.UnRegisterCard();
        Destroy(gameObject);
    }
}

public class CardProperties
{
    public int id;
    public Sprite sprite;
}