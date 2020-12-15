using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// The core monster controller class. All monsters must have a monster script attached to them.
// It interfaces with an AI script for behaviors, which in turn controls a weapon script which defines it's attack behaviors.

// TODO: Implement rigidbody switching for knockback.
// TODO: Refactor EnemyWeapon class into EnemyAttack, support multiple.

[RequireComponent(typeof(AI))]
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
    public NavMeshAgent _agent;
    [SerializeField]private GameObject disableOnDeath;
    [SerializeField]private GameObject enableOnDeath;
    private bool _isPhysical = false;
    [SerializeField] private float _knockbackDuration = 0.1f;
    public float knockbackStrength = 2f;
    private float _knockbackTimer;
    private Rigidbody _rigidbody;
    private float _distanceToGround;


    // Start is called before the first frame update
    void Start()
    {
        _weapon = GetComponent<EnemyWeapon>();
        _AI = GetComponent<AI>();
        _agent = GetComponent<NavMeshAgent>();
        _distanceToGround = GetComponent<Collider>().bounds.extents.y;
        //_agent.enabled = false;
    }

    void Update()
    {
        //Re-enable navigation after knockback duration
        if (_isPhysical)
        {
            _knockbackTimer += Time.deltaTime;
            if (_knockbackTimer > _knockbackDuration && IsGrounded())
            {
                _agent.enabled = true;
                _rigidbody.isKinematic = true;
                _isPhysical = false;
                _knockbackTimer = 0;
            }
        }
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

    public void Nickelback(Vector3 direction, float strength)
    {
        //Move in direction at strength
        Debug.Log("direction is " + direction);
        Debug.Log("strength is " + strength);
    }
    
    //Disable NavMesh agent, enable physics on collision
    public void Knockback(Vector3 point)
    {
        _rigidbody = GetComponent<Rigidbody>();
        _agent.enabled = false;
        _rigidbody.isKinematic = false;
        _isPhysical = true;
        Debug.Log("collision");
        Debug.Log(_rigidbody);
        if (_rigidbody != null)
        {
            Vector3 direction = point - transform.position;
            //direction.y = 0;
            _rigidbody.AddForce(direction.normalized * knockbackStrength, ForceMode.Impulse); // impulse allows mass to affect knockback strength
            Debug.Log(direction.normalized);
        Debug.Log("Knockback was called");
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, _distanceToGround + 0.1f);
    }
}
