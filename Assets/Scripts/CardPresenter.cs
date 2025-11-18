using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CardPresenter : MonoBehaviour
{
    public Card card;
    public int row;     //100 = waste

    [Header("Visuals")]
    public Sprite cardFace;
    public Sprite cardBack;
    public SpriteRenderer spriteRenderer;

    [Header("States")]
    public bool faceUp;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (faceUp)
        {
            spriteRenderer.sprite = cardFace;
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }
    }

    public void SetCard(Card _card)
    {
        card = _card;
        UpdateFace();
    }

    void UpdateFace()
    {
        int i = 0;
        for (int j = 1; j <= 13; j++)
        {
            foreach (Card.suits suit in Enum.GetValues(typeof(Card.suits)))
            {
                if (card.suit == suit && card.value == j)
                {
                    cardFace = SolitaireGame.instance.cardSprites[i];
                    //spriteRenderer.sprite = cardFace;
                    break;
                }
                i++;
            }
        }
    }
}
