﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Draws the number of cards set.

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/PotOfGreed", order = 400)]
public class PotOfGreed : Card
{
    public int DrawNumber;

    protected override void Action()
    {
        for (int i = 0; i < DrawNumber; i++)
        {
            _caster.GetComponent<Player>().Draw();
        }
    }
}
