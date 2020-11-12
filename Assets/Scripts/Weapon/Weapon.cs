using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Random = UnityEngine.Random;

// Abstract weapon class, defines what input a weapon can receive from it's user, and provides helpful utility functions that weapons will frequently need.

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] private string weaponName;
    [SerializeField] protected float ammo;

    protected void FixedUpdate()
    {
        if (ammo <= 0)
        {
            // drop weapon.
        }
    }

    public abstract void FirePressed();
    public abstract void FireReleased();
    public abstract void AltFirePressed();
    public abstract void AltFireReleased();

    // TryFire is called on button press, or every tick the fire button is held for automatic weapons.
    // This is where the conditions to fire are defined. the default is a simple cooldown after every shot.
    protected abstract void TryFire();

    // Fire is called on TryFire success.
    protected abstract void Fire();

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
    
    protected Vector3 HitScan(float damage, Vector3 origin, Vector3 direction, float length, LayerMask layerMask)
    {
        //LayerMask layerMask = LayerMask.GetMask("HitBox");
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, length))
        {
            if (hit.transform.CompareTag("Player"))
            {
                hit.transform.GetComponentInParent<Player>().Hurt(damage);
            }
            
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponentInParent<Enemy>().Hurt(damage);
            }
            return hit.point;
        }
        return origin + direction * length;
    }

    public string GetWeaponName()
    {
        return weaponName;
    }
}
