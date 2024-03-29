﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardUseTarget : UseTarget
{
    private Collider _collider;

    [SerializeField] private bool randomize;
    [SerializeField] private Card card1;
    [SerializeField] private Card card2;
    [SerializeField] private Card card3;

    [SerializeField] private CardList cardList;
    
    private void Start()
    {
        _collider = GetComponentInChildren<Collider>();
        if (randomize)
        {
            card1 = cardList.GetRandomCard();
            card2 = cardList.GetRandomCard();
            card3 = cardList.GetRandomCard();
            Debug.Log("Card3: " + card3.Name);
        }
    }

    public override void Use(Player user)
    {
        user.PickupCardPack(new CardPack(card1, card2, card3));
        Destroy(gameObject);
    }
}

public class CardPack
{
    public Card Card1;
    public Card Card2;
    public Card Card3;

    public CardPack(Card card1, Card card2, Card card3)
    {
        Card1 = card1;
        Card2 = card2;
        Card3 = card3;
    }
}
