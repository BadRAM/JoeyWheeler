using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStartingDeck", menuName = "Cards/StartingDeck", order = 0)]
public class StartingDeck : ScriptableObject
{
    public List<Card> Cards;
}
