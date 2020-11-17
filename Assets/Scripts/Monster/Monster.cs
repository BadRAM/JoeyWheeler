﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// The core monster controller class. All monsters must have a monster script attached to them.
// It interfaces with an AI script for behaviors, which in turn controls a weapon script which defines it's attack behaviors.

[RequireComponent(typeof(AI))]
[RequireComponent(typeof(EnemyWeapon))]
public class Monster : MonoBehaviour
{
    public int Team = 1;
    [SerializeField] private bool IsBoss;
    [SerializeField] private float Health;
    [SerializeField] private float DeathDuration;
    private bool _alive = true;
    private float _timeOfDeath;
    private EnemyWeapon _weapon;
    private AI _AI;
    private Transform _playerTransform;
    private NavMeshAgent _agent;
    [SerializeField]private GameObject disableOnDeath;
    [SerializeField]private GameObject enableOnDeath;


    // Start is called before the first frame update
    void Start()
    {
        _weapon = GetComponent<EnemyWeapon>();
        _AI = GetComponent<AI>();
        _agent = GetComponent<NavMeshAgent>();
        //_agent.enabled = false;
    }

    private void FixedUpdate()
    {
        if (Health > 0)
        {
            _AI.AiBehavior();
        }
        else if (Time.time - _timeOfDeath > DeathDuration)
        {
            Destroy(gameObject);
        }
    }

    public void Hurt(float damageTaken)
    {
        Health -= damageTaken;
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (IsBoss)
        {
            GameInfo.State = GameInfo.GameState.Victory;
        }
        Health = 0;
        _alive = false;
        _timeOfDeath = Time.time;
        disableOnDeath.SetActive(false);
        enableOnDeath.SetActive(true);
    }

    public bool IsAlive()
    {
        return _alive;
    }

    public Vector3 GetCenter()
    {
        return transform.position + Vector3.up * _agent.height / 2f;
    }
}