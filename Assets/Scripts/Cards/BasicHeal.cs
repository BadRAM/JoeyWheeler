using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//card which restores cards from discard pile.

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/BasicHeal", order = 400)]
public class BasicHeal : Card
{
    public int cardsToRestore;

    protected override void Action()
    {
        _caster.GetComponent<Player>().deck.Restore(cardsToRestore);
    }
}
