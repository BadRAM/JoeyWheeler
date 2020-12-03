using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TierArt", menuName = "Cards/TierArt", order = 0)]
public class TierArt : ScriptableObject
{
    public Sprite Tier1Border;
    public Sprite Tier2Border;
    public Sprite Tier3Border;
    public Sprite Tier4Border;

    public Sprite Type1Background;
    public Sprite Type2Background;
    public Sprite Type3Background;
    public Sprite Type4Background;
    public Sprite Type5Background;
    public Sprite Type6Background;

    public Sprite GetTierBorder(Card.CardTier tier)
    {
        switch (tier)
        {
            case Card.CardTier.Cantrip:
                return Tier1Border;
            case Card.CardTier.Scholarly:
                return Tier2Border;
            case Card.CardTier.Ancient:
                return Tier3Border;
            case Card.CardTier.Forbidden:
                return Tier4Border;
        }

        return Tier1Border;
    }

    public Sprite GetTypeBackground(Card.CardType type)
    {
        switch (type)
        {
            case Card.CardType.Weapon:
                return Type1Background;
            case Card.CardType.Monster:
                return Type2Background;
            case Card.CardType.Familiar:
                return Type3Background;
            case Card.CardType.Spell:
                return Type4Background;
            case Card.CardType.Potion:
                return Type5Background;
            case Card.CardType.Hex:
                return Type6Background;
        }

        return Type1Background;
    }
    
}
