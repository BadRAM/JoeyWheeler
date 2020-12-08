using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple card, instantiates a prefab.

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/ExtraMag", order = 400)]
public class ExtraMag : Card
{
    protected override void Action()
    {
        Player p = _caster.GetComponent<Player>();
        if (p.weapon != null)
        {
            p.LoadWeapon();
        }
    }
}
