using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : ScriptableObject
{
    // public fields for setting in the editor.
    // these are only the fields that ALL cards have.
    public string Name;
    public Sprite Art;
    public string Description;
    public string Fact;
    public CardType Type;
    public CardTier Tier;

    // protected fields, are populated when the card is activated, and are available to the implementing class' Action() function.
    protected GameObject _caster;
    protected int _team;
    protected Vector3 _origin;
    protected Vector3 _forward;
    protected Quaternion _direction;
    protected Transform _parent;

    public void Activate(Player caster, Transform origin)
    {
        _caster = caster.gameObject;
        _team = 0;
        _origin = origin.position;
        _forward = origin.forward;
        _direction = origin.rotation;
        _parent = origin;
        
        Action();
    }

//    public void Activate(Monster caster, Vector3 origin, Vector3 target)
//    {
//        
//    }

    protected abstract void Action();

    public enum CardType
    {
        Weapon,
        Monster,
        Familiar,
        Spell,
        Potion,
        Hex
    }

    public enum CardTier
    {
        Cantrip,
        Scholarly,
        Ancient,
        Forbidden
    }
}