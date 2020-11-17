using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//card which gives the player a weapon.

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Weapon", order = 100)]
public class WeaponCard : Card
{
    public GameObject weapon;

    protected override void Action()
    {
        _caster.GetComponent<Player>().EquipWeapon(weapon);
    }
}
