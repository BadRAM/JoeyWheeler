using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class AI : MonoBehaviour
{
    //[SerializeField] private float fireCooldown = 1;
    private float _fireHeat;
    private List<Vector3> _waypoints;
    private bool _attacking;
    private LayerMask _walls;
    protected EnemyWeapon _weapon;
    protected NavMeshAgent _agent;
    //protected string _state;

    // default EnemyAI follows a sequence of waypoints, deviating to attack the player if the player gets too close.
    
    protected virtual void Start()
    {
        _weapon = GetComponent<EnemyWeapon>();
        _agent = GetComponent<NavMeshAgent>();
        _walls = LayerMask.GetMask("Terrain", "Default");
    }

    public abstract void AiBehavior();

    protected bool Fire()
    {
        return _weapon.Fire(GameInfo.Player.GetCenter());
    }
    
    protected void RunAwayFromPlayer()
    {
        Vector3 targetPos = (transform.position - GameInfo.Player.GetCenter()).normalized * 2;
        
        _agent.SetDestination(transform.position + targetPos);
    }

    protected void ApproachPlayer()
    {
        _agent.SetDestination(GameInfo.Player.GetCenter());
    }

    protected Vector3 GetCenter()
    {
        return transform.position;
    }

    protected bool CanSeePlayer()
    {
        return !Physics.Linecast(GetCenter(), GameInfo.Player.GetCenter(), _walls);
    }

    protected float DistanceToPlayer()
    {
        return Vector3.Distance(GetCenter(), GameInfo.Player.GetCenter());
    }
}
