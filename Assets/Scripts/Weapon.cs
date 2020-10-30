using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Random = UnityEngine.Random;

// parent class to weapon scripts. not a functional weapon on it's own

public class Weapon : MonoBehaviour
{
    [SerializeField] private string weaponName;
    [SerializeField] protected GameObject Projectile;
    [SerializeField] protected Transform ProjectileSpawn;
    protected float heat;
    [SerializeField] private float Cooldown; // the time in seconds between shots
    [SerializeField] protected float Spread;   // the spread angle in degrees
    [SerializeField] private bool Automatic; // whether the player can fire continuously by holding fire

    private void FixedUpdate()
    {
        heat = Mathf.Max(0, heat - Time.deltaTime);

        if (Automatic && Input.GetButton("Fire1"))
        {
            TryFire();
        }
    }

    private void Update()
    {
        if (!Automatic && Input.GetButtonDown("Fire1"))
        {
            TryFire();
        }
    }

    // TryFire is called on button press, and every tick the fire button is held for automatic weapons.
    // This is where the conditions to fire are defined. the default is a simple cooldown after every shot.
    protected virtual void TryFire()
    {
        if (heat == 0)
        {
            Fire();
            heat = Cooldown;
        }
    }

    // Fire is called on TryFire success.
    // The default is a damage ray which also spawns an effect gameobject.
    protected virtual void Fire()
    {
        Quaternion aimTarget = Quaternion.LookRotation(RandomSpread(ProjectileSpawn.forward, Spread), ProjectileSpawn.up);
        Instantiate(Projectile, ProjectileSpawn.position, aimTarget);
    }

    // General purpose bullet spread functions. take a direction vector and a max angle and return a perturbed vector.
    protected Vector3 RandomSpread(Vector3 dir, float spread)
    {
        dir = dir.normalized;
        Vector3 angle = Vector3.ProjectOnPlane(UnityEngine.Random.insideUnitSphere, dir).normalized;
        spread = spread * Random.Range(0.0f, 1.0f);
        dir = Vector3.RotateTowards(dir, angle, Mathf.Deg2Rad * spread, 0.0f);
        return dir.normalized;
    }
    
    // this one takes an animationcurve as a custom spread distribution
    protected Vector3 RandomSpread(Vector3 dir, AnimationCurve spreadCurve)
    {
        dir = dir.normalized;
        Vector3 angle = Vector3.ProjectOnPlane(UnityEngine.Random.insideUnitSphere, dir).normalized;
        float spread = spreadCurve.Evaluate(Random.Range(0.0f, 1.0f));
        dir = Vector3.RotateTowards(dir, angle, Mathf.Deg2Rad * spread, 0.0f);
        return dir.normalized;
    }

    public string GetWeaponName()
    {
        return weaponName;
    }
}
