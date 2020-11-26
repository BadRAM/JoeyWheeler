using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardUseTarget : UseTarget
{
    private CardObject _cardObject;
    private Collider _collider;
    private void Start()
    {
        _cardObject = GetComponent<CardObject>();
        _collider = GetComponentInChildren<Collider>();
    }

    public override void Use(Player user)
    {
        _cardObject.Pickup(user);
    }

    public override String Description()
    {
        return "Pickup card: " + _cardObject.GetCard().Name;
    }
}
