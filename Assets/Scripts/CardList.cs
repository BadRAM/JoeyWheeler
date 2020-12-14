using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "NewCardList", menuName = "CardList", order = 0)]
public class CardList : ScriptableObject
{
    public List<Card> Cards;

    public Card GetRandomCard()
    {
        return Cards[Random.Range(0, Cards.Count)];
    }
}
