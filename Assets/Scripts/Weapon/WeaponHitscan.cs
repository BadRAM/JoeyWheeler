﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitscan : Weapon
{
    [SerializeField] private GameObject Projectile;
    [SerializeField] private Transform ProjectileSpawn;
    [SerializeField] private float Cooldown; // the time in seconds between shots
    [SerializeField] private bool Automatic; // whether the player can fire continuously by holding fire
    [SerializeField] private float Spread;   // the spread angle in degrees
    [SerializeField] private Transform RaycastOrigin;
    [SerializeField] private float Damage;   // how much damage a raycast will do
    [SerializeField] private string[] RaycastLayerMask = {"HitBox"};
    
    private float _heat;
    
    private bool _doFire;
    private bool _doAltFire;

    private LayerMask _raycastLayerMask;

    private void Start()
    {
        _raycastLayerMask = LayerMask.GetMask(RaycastLayerMask);
    }

    void FixedUpdate()
    {
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
        Vector3 hitpoint = HitScan(Damage, RaycastOrigin.position, RandomSpread(RaycastOrigin.forward, Spread), Mathf.Infinity, _raycastLayerMask);
        GameObject beam = Instantiate(Projectile, ProjectileSpawn.position, ProjectileSpawn.rotation);
        beam.GetComponent<Beam>().endPoint = hitpoint;
        beam.GetComponent<Beam>().startPoint = ProjectileSpawn.position;
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
