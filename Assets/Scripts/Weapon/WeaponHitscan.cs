using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitscan : Weapon
{
    [SerializeField] private GameObject Projectile;
    [SerializeField] private float Cooldown; // the time in seconds between shots
    [SerializeField] private bool Automatic; // whether the player can fire continuously by holding fire
    [SerializeField] private float Spread;   // the spread angle in degrees
    [SerializeField] private float Damage;   // how much damage a raycast will do
    [SerializeField] private string[] RaycastLayerMask = {"HitBox"};
    [SerializeField] private bool FriendlyFire;
    
    private float _heat;
    
    private bool _doFire;
    private bool _doAltFire;

    private LayerMask _raycastLayerMask;

    private void Start()
    {
        _raycastLayerMask = LayerMask.GetMask(RaycastLayerMask);
    }

    new void FixedUpdate()
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
        Ammo--;
        Vector3 hitpoint = HitScan(Damage, FriendlyFire, raycastOrigin.position, RandomSpread(raycastOrigin.forward, Spread), Mathf.Infinity, _raycastLayerMask);
        GameObject beam = Instantiate(Projectile, transform.position, transform.rotation);
        beam.GetComponent<Beam>().endPoint = hitpoint;
        beam.GetComponent<Beam>().startPoint = transform.position;
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
