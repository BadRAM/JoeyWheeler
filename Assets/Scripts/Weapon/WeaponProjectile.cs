﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class WeaponProjectile : Weapon
{
    
    [SerializeField] private GameObject Projectile;
    [SerializeField] private Transform ProjectileSpawn;
    [SerializeField] private float Cooldown; // the time in seconds between shots
    [SerializeField] private bool Automatic; // whether the player can fire continuously by holding fire
    [SerializeField] protected float Spread;   // the spread angle in degrees

    private bool _doFire;
    private bool _doAltFire;
    
    private float _heat;

    void FixedUpdate()
    {
        base.FixedUpdate();
        _heat = Mathf.Max(0, _heat - Time.deltaTime);

        if (_doFire)
        {
            TryFire();
            if (!Automatic)
            {
                _doFire = false;
            }
        }
    }
    
    protected override void TryFire()
    {
        if (_heat == 0)
        {
            Fire();
            _heat = Cooldown;
        }
    }

    protected override void Fire()
    {
        ammo--;
        Quaternion aimTarget = Quaternion.LookRotation(RandomSpread(ProjectileSpawn.forward, Spread), ProjectileSpawn.up);
        Instantiate(Projectile, ProjectileSpawn.position, aimTarget);
    }

    public override void FirePressed()
    {
        _doFire = true;
    }

    public override void FireReleased()
    {
        _doFire = false;
    }

    public override void AltFirePressed()
    {
        _doAltFire = true;
    }

    public override void AltFireReleased()
    {
        _doAltFire = false;
    }
}
