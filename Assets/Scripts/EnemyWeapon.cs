﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float cooldown;
    [SerializeField] private Transform projectileSpawn;
    private float _heat;

    private void FixedUpdate()
    {
        _heat = Mathf.Max(0, _heat - Time.deltaTime);
    }

    
    public virtual bool Fire(Vector3 target) // returns true if shot fired successfully.
    {
        if (_heat == 0)
        {
            Instantiate(projectile, projectileSpawn.position,
                Quaternion.LookRotation(target - projectileSpawn.position, transform.up));
            _heat = cooldown;
            return true;
        }
        return false;
    }
}
