using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardUseTarget : UseTarget
{
    [SerializeField] private CardObject cardObject;
    private Collider _collider;
    private void Start()
    {
        _collider = GetComponentInChildren<Collider>();
    }

    public override void Use(Player user)
    {
        cardObject.Pickup(user);
    }

    public override String Description()
    {
        return "Pickup card: " + cardObject.GetName();
    }
}
