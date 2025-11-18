using NUnit.Framework;
using NUnit.Framework.Internal.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Multiplayer.Center.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SolitaireGame : MonoBehaviour
{
    public static SolitaireGame instance;

    [Header("Win Message")]
    public bool win = false;
    public GameObject winMessage;

    [Header("Positions")]
    public GameObject deckPos;
    public List<GameObject> bottomPos;
    public List<GameObject> topPos;

    [Header("Deck")]
    public List<Card> deck = new List<Card>();
    public List<Card> discardPile = new List<Card>();
    public List<Card> waste = new List<Card>();

    [Header("Bottom Sections")]
    public List<Card> bottom0 = new List<Card>();
    public List<Card> bottom1 = new List<Card>();
    public List<Card> bottom2 = new List<Card>();
    public List<Card> bottom3 = new List<Card>();
    public List<Card> bottom4 = new List<Card>();
    public List<Card> bottom5 = new List<Card>();
    public List<Card> bottom6 = new List<Card>();

    public List<Card>[] bottoms;

    [Header("Top Sections")]
    public List<Card> top0 = new List<Card>();
    public List<Card> top1 = new List<Card>();
    public List<Card> top2 = new List<Card>();
    public List<Card> top3 = new List<Card>();

    public List<Card>[] tops;


    [Header("Assets")]
    public List<Sprite> cardSprites = new List<Sprite>();
    public GameObject cardPrefab;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;

        bottoms = new List<Card>[] { bottom0, bottom1, bottom2, bottom3, bottom4, bottom5, bottom6 };
        tops = new List<Card>[] { top0, top1, top2, top3 };

        GenerateDeck();
        Shuffle<Card>(deck);
        Deal();
        GenerateSortedPresenters();
        //GeneratePresenters();

        winMessage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateDeck()
    {
        for(int i = 1; i <= 13; i++)
        { 
            foreach(Card.suits suit in Enum.GetValues(typeof(Card.suits)))
            {
                Card newCard = new Card(suit, i);
                deck.Add(newCard);
            }
        }
    }

    void Shuffle<T>(List<T> list)
    {
        System.Random rand = new System.Random();

        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rand.Next(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    void Deal()
    {
        for (int i = 0; i < 7; i ++)
        {
            for (int j = i; j < 7; j ++)
            {
                bottoms[j].Add(deck.Last<Card>());
                deck.RemoveAt(deck.Count - 1);
            }
        }    
    }

    void GenerateSortedPresenters()
    {
        for(int i = 0; i < 7; i ++)
        {
            foreach (Card card in bottoms[i])
            {
                Vector3 position = bottomPos[i].transform.position - new Vector3(0,0,.05f);     //default position
                GameObject parent = bottomPos[i];
                if (bottoms[i].Count > 0)
                {
                    for (int j = 0; j < bottoms[i].Count; j ++)
                    {
                        if (parent.transform.childCount > 0)
                        {
                            parent = parent.transform.GetChild(0).gameObject;
                            position = new Vector3 (parent.transform.position.x, parent.transform.position.y -.5f, parent.transform.position.z -.05f);
                        }
                    }
                }

                GameObject newPresenter = Instantiate(cardPrefab, position, Quaternion.identity);
                newPresenter.transform.parent = parent.transform;
                newPresenter.GetComponent<CardPresenter>().SetCard(card);
                newPresenter.GetComponent<CardPresenter>().row = i;
                newPresenter.name = card.suit.ToString() + card.value;
                if (card == bottoms[i][bottoms[i].Count - 1])
                {
                    newPresenter.GetComponent<CardPresenter>().faceUp = true;
                }
            }
        }
    }

    void GeneratePresenters()
    {
        float zOffset = 0f;

        foreach(Card card in deck)
        {
            GameObject newPresenter = Instantiate(cardPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z + zOffset), transform.rotation);
            newPresenter.GetComponent<CardPresenter>().SetCard(card);
            newPresenter.name = card.suit.ToString() + card.value;

            zOffset -= .05f;
        }
    }

    public void DealFromDeck()
    {
        //clear waste
        foreach (Card card in waste)
        {
            discardPile.Add(card);
        }
        waste.Clear();

        //find how many cards to add to waste
        int count = 0;

        if (deck.Count == 0)        //put discard pile back to deck
        {
            deck = new List<Card>(discardPile);
            deck.Reverse();
            discardPile.Clear();
        }
        else if (deck.Count >= 3)   //deal 3 
        {
            count = 3;
        }
        else                        //deal <3
        {
            count = deck.Count;
        }

        //add to waste
        for (int i = 0; i < count; i++)
        {
            Card card = deck[deck.Count-1];
            deck.Remove(card);
            waste.Add(card);
        }

        //PRESENTERS
        //clear waste
        foreach(Transform child in deckPos.transform)
        {
            Destroy(child.gameObject);
        }
        //new waste
        float xOffset = 2f;
        float zOffset = 0f;
        foreach(Card card in waste)
        {
            GameObject newPresenter = Instantiate(cardPrefab, new Vector3(deckPos.transform.position.x + xOffset, deckPos.transform.position.y, deckPos.transform.position.z + zOffset), transform.rotation);
            newPresenter.transform.parent = deckPos.transform;
            newPresenter.GetComponent<CardPresenter>().SetCard(card);
            newPresenter.GetComponent<CardPresenter>().faceUp = true;
            newPresenter.GetComponent<CardPresenter>().row = 100;
            newPresenter.name = card.suit.ToString() + card.value;

            xOffset += 1f;
            zOffset -= .05f;
        }
    }

    public bool CanStack(GameObject previousSelected, GameObject selected)
    {
        Card previousCard = previousSelected.GetComponent<CardPresenter>().card;
        Card selectedCard = selected.GetComponent<CardPresenter>().card;

        print("previous card is " +  previousCard.suit + previousCard.value);
        print("selected card is " + selectedCard.suit + selectedCard.value);

        if (selected.GetComponent<CardPresenter>().row == 100)
        {
            print("not stackable, destination is part of waste");
            return false;
        }
        if (selectedCard.suit != bottoms[selected.GetComponent<CardPresenter>().row].Last<Card>().suit && selectedCard.value != bottoms[selected.GetComponent<CardPresenter>().row].Last<Card>().value)
        {
            print("not stackable, destination is not at the bottom of the stack");
            return false;
        }


        if (previousCard != null && selectedCard != null)
        {
                if (previousCard.value == selectedCard.value - 1)
                {
                    bool previousCardRed = true;
                    bool selectedCardRed = true;

                    if (previousCard.suit == Card.suits.Spades || previousCard.suit == Card.suits.Clubs)
                    {
                        previousCardRed = false;
                    }
                    if (selectedCard.suit == Card.suits.Spades || selectedCard.suit == Card.suits.Clubs)
                    {
                        selectedCardRed = false;
                    }

                    if (previousCardRed == selectedCardRed)
                    {
                        print("not stackable");
                        return false;
                    }
                    else
                    {
                        print("stackable");
                        return true;
                    }
                }
        }

        print("not stackable");
        return false;
    }

    public void Stack(GameObject previousSelected, GameObject selected)
    {
        //reveal card before previousSelected card
        if (previousSelected.transform.parent.CompareTag("Card"))
        {
            print("flipped up " + previousSelected.transform.parent.name);
            previousSelected.transform.parent.GetComponent<CardPresenter>().faceUp = true; 
        }

        //move previousSelected stack of cards
        previousSelected.transform.position = selected.transform.position + new Vector3(0, -.5f,-.05f);
        previousSelected.transform.parent = selected.transform;

        //move child cards from old list to new list
        CardPresenter parentPresenter = selected.GetComponent<CardPresenter>();
        Transform child = null;
        if (selected.transform.childCount > 0)
        {
            child = selected.transform.GetChild(0);
        }
        //foreach (Transform child in selected.transform)
        while (child != null) 
        {
            CardPresenter childPresenter = child.GetComponent<CardPresenter>();

            //waste
            if (childPresenter.row == 100)
            {
                Card temp = waste.Single(x => x.suit == childPresenter.card.suit && x.value == childPresenter.card.value);
                waste.Remove(temp);
                bottoms[parentPresenter.row].Add(childPresenter.card);
                childPresenter.row = parentPresenter.row;
            }
            //other row
            else
            {
                Card temp = bottoms[childPresenter.row].Single(x => x.suit == childPresenter.card.suit && x.value == childPresenter.card.value);
                bottoms[childPresenter.row].Remove(temp);
                bottoms[parentPresenter.row].Add(childPresenter.card);
                childPresenter.row = parentPresenter.row;
            }

            if (child.childCount > 0)
            {
                child = child.GetChild(0);
            }
            else
            { 
                child = null;
            }
        }
    }

    public void StackToBottom(GameObject previousSelected, GameObject selected)
    {
        if (previousSelected.GetComponent<CardPresenter>().card.value == 13)
        {
            //reveal card before previousSelected card
            if (previousSelected.transform.parent.CompareTag("Card"))
            {
                print("flipped up " + previousSelected.transform.parent.name);
                previousSelected.transform.parent.GetComponent<CardPresenter>().faceUp = true;
            }

            previousSelected.transform.position = selected.transform.position + new Vector3(0, 0, -.05f);
            previousSelected.transform.parent = selected.transform;

            //move child cards from old list to new list
            //CardPresenter parentPresenter = selected.GetComponent<CardPresenter>();
            int row = bottomPos.IndexOf(selected);
            Transform child = null;
            if (selected.transform.childCount > 0)
            {
                child = selected.transform.GetChild(0);
            }
            while (child != null)
            {
                CardPresenter childPresenter = child.GetComponent<CardPresenter>();

                //waste
                if (childPresenter.row == 100)
                {
                    Card temp = waste.Single(x => x.suit == childPresenter.card.suit && x.value == childPresenter.card.value);
                    waste.Remove(temp);
                    bottoms[row].Add(childPresenter.card);
                    childPresenter.row = row;
                }
                //other row
                else
                {
                    Card temp = bottoms[childPresenter.row].Single(x => x.suit == childPresenter.card.suit && x.value == childPresenter.card.value);
                    bottoms[childPresenter.row].Remove(temp);
                    bottoms[row].Add(childPresenter.card);
                    childPresenter.row = row;
                }

                if (child.childCount > 0)
                {
                    child = child.GetChild(0);
                }
                else
                {
                    child = null;
                }
            }
        }
    }

    public void StackToTop(GameObject previousSelected, GameObject selected)
    {
        print("adding " + previousSelected + "to " + selected);
        int row = topPos.IndexOf(selected);
        int last = 0;
        Card.suits suit = previousSelected.GetComponent<CardPresenter>().card.suit;

        if (tops[row].Count > 0)
        {
            last = tops[row].Last<Card>().value;
            suit = tops[row].Last<Card>().suit;
        }

        //add to pile
        if (previousSelected.GetComponent<CardPresenter>().card.suit == suit)
        {
            if (previousSelected.GetComponent<CardPresenter>().card.value == last + 1)
            {
                //reveal card before previousSelected card
                if (previousSelected.transform.parent.CompareTag("Card"))
                {
                    print("flipped up " + previousSelected.transform.parent.name);
                    previousSelected.transform.parent.GetComponent<CardPresenter>().faceUp = true;
                }

                previousSelected.transform.position = selected.transform.position + new Vector3(0,0,-.5f * tops[row].Count);
                previousSelected.transform.parent = selected.transform;
                previousSelected.GetComponent<BoxCollider2D>().enabled = false;

                tops[row].Add(previousSelected.GetComponent<CardPresenter>().card);

                //remove from old list
                CardPresenter cardPresenter = previousSelected.GetComponent<CardPresenter>();

                //waste
                if (cardPresenter.row == 100)
                {
                    Card temp = waste.Single(x => x.suit == cardPresenter.card.suit && x.value == cardPresenter.card.value);
                    waste.Remove(temp);
                }
                //other row
                else
                {
                    Card temp = bottoms[cardPresenter.row].Single(x => x.suit == cardPresenter.card.suit && x.value == cardPresenter.card.value);
                    bottoms[cardPresenter.row].Remove(temp);
                }
            }
        }

        int total = 0;
        foreach (List<Card> cards in tops)
        {
            total += cards.Count;
        }

        if (total == 52)
        {
            win = true;
            print("you win");
            winMessage.SetActive(true);
        }

    }

    public void Restart()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
