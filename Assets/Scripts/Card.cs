using UnityEngine;

[System.Serializable]
public class Card
{
    public enum suits {Clubs, Diamonds, Hearts, Spades}
    public suits suit;
    public int value;

    public Card(suits _suit, int _value)
    {
        suit = _suit;
        value = _value;
    }

}
