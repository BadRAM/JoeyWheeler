using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for storing deck info.

public class Deck
{
    public List<Card> Undrawn;
    public Card[] Hand;
    public List<Card> Discards;

    public Deck(List<Card> cards)
    {
        Undrawn = new List<Card>(cards);
        Undrawn.Shuffle();
        Hand = new Card[5];
        Discards = new List<Card>();
    }

    // Draw num cards to the hand from the top (0) of the undrawn pile.
    // Returns true if successful, even partially, false if no cards were drawn. Will fail if hand is full, or pile is empty. 
    public bool Draw(int num)
    {
        bool didDraw = false;
        for (int i = 0; i < num; i++)
        {
            if (CardsInHand() < 5 && Undrawn.Count > 0)
            {
                AddCardToHand(Undrawn[0]);
                Undrawn.RemoveAt(0);
                didDraw = true;
            }
        }

        return didDraw;
    }

    // Shuffle discards back into deck
    public void Restore(int num)
    {
        for (int i = 0; i < num; i++)
        {
            if (Discards.Count > 0)
            {
                int x = Random.Range(0, Discards.Count);
                int y = Random.Range(0, Undrawn.Count);
                Undrawn.Insert(y, Discards[x]);
            }
        }
    }

    public void Shuffle()
    {
        Undrawn.Shuffle();
    }

    // move a card from the hand to the discards pile.
    public void Discard(int index)
    {
        Discards.Add(Hand[index]);
        Hand[index] = null;
    }

    public int CardsInHand()
    {
        int cards = 0;

        for (int i = 0; i < 5; i++)
        {
            if (Hand[i] != null)
            {
                cards++;
            }
        }
        return cards;
    }

    public string GetNameOfCardInHand(int index)
    {
        if (Hand[index] != null)
        {
            return Hand[index].name;
        }
        else
        {
            return "";
        }
    }

    public bool AddCardToHand(Card toAdd)
    {
        for (int i = 0; i < 5; i++)
        {
            if (Hand[i] == null)
            {
                Hand[i] = toAdd;
                return true;
            }
        }
        return false;
    }

    // Knocks cards out of the player's hand and deck when damage is taken.
    // returns true if deck was lowered below zero by attacks, and false if it was lowered to zero or above.
    public bool Damage(int damage)
    {
        // distribute damage between the hand and deck.
        int handDamage = Mathf.Clamp(damage / 5, 1, 5);
        int deckDamage = damage - handDamage;

        while (handDamage > 0)
        {
            if (CardsInHand() > 0)
            {
                int cardToRemove = Random.Range(0, CardsInHand());
                int i = 0;
                while (i < 5)
                {
                    if (Hand[i] == null)
                    {
                        i++;
                        continue;
                    }

                    if (cardToRemove == 0)
                    {
                        Discard(i);
                        handDamage--;
                        break;
                    }
                    cardToRemove--;
                }
            }
            else
            {
                // if there aren't enough cards in your hand to remove, carry the remaining damage over to the deck.
                deckDamage += handDamage;
                handDamage = 0;
            }
        }

        while (deckDamage > 0)
        {
            if (Undrawn.Count == 0)
            {
                return true;
            }
            Undrawn.RemoveAt(0);
            deckDamage--;
        }

        return false;
    }
}
