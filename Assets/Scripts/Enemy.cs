using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AI))]
public class Enemy : MonoBehaviour
{

    [SerializeField] private float Health;
    [SerializeField] private float DeathDuration;
    private bool _alive;
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
        Health = 0;
        _timeOfDeath = Time.time;
        disableOnDeath.SetActive(false);
        enableOnDeath.SetActive(true);
    }

    public bool GetAlive()
    {
        return _alive;
    }
}
