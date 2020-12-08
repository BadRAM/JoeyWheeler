using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple card, instantiates a prefab.

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/DaJump", order = 400)]
public class DaJump : Card
{
    public float Strength;
    protected override void Action()
    {
        Player p = _caster.GetComponent<Player>();
        _caster.GetComponent<Rigidbody>().velocity += p.raycastOrigin.forward * Strength;
    }
}