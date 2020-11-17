using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple card, instantiates a prefab.

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Spell", order = 1)]
public class Spell : Card
{
    public GameObject SpellPrefab;

    protected override void Action()
    {
        Instantiate(SpellPrefab, _origin, _direction);
    }
}
