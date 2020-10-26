using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    //[SerializeField] private float fireCooldown = 1;
    private float _fireHeat;
    private List<Vector3> _waypoints;
    private bool _attacking;
    protected EnemyWeapon _weapon;
    protected NavMeshAgent _agent;
    //protected string _state;
    [SerializeField] private float _targetRange = 15; // range to abandon path and attack player
    [SerializeField] private float _waypointRange = 3; // range to reach before targeting the next waypoint

    // default EnemyAI follows a sequence of waypoints, deviating to attack the player if the player gets too close.
    
    protected virtual void Start()
    {
        _weapon = GetComponent<EnemyWeapon>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.SetDestination(_waypoints[0]);
    }

    public virtual void AiBehavior()
    {
        if (!_attacking)
        {
            if (Vector3.Distance(transform.position, _waypoints[0]) < _waypointRange)
            {
                _waypoints.RemoveAt(0);
                if (_waypoints.Count == 0)
                {
                    _attacking = true;
                }
                else
                {
                    _agent.SetDestination(_waypoints[0]);
                }
            }

            if (Vector3.Distance(GameInfo.Player.GetCenter(), transform.position) < _targetRange
                && Physics.Linecast(transform.position, GameInfo.Player.GetCenter()))
            {
                //_attacking = true;
            }
        }

        if (_attacking)
        {
            if (_fireHeat == 0)
            {
                _fire();
                _fireHeat = 1;
                _agent.SetDestination(GameInfo.Player.GetCenter());
            }
        }
        

        _fireHeat = Mathf.Max(0, _fireHeat - Time.deltaTime);
    }

    public void SetWaypoints(List<Vector3> waypoints)
    {
        _waypoints = new List<Vector3>(waypoints);
        if (_agent != null)
        {
            _agent.SetDestination(_waypoints[0]);
        }
    }
    
    // finds the nearest waypoint and restarts pathing from there.
    protected virtual void retarget()
    {
        float d = Mathf.Infinity;
        int im = 0;
        for (int i = 0; i < _waypoints.Count; i++)
        {
            if (Vector3.Distance(transform.position, _waypoints[i]) < d)
            {
                im = i;
                d = Vector3.Distance(transform.position, _waypoints[i]);
            }
        }
        _waypoints.RemoveRange(0, im);
        _agent.SetDestination(_waypoints[0]);
    }

    protected bool _fire()
    {
        return _weapon.Fire(GameInfo.Player.GetCenter());
    }
    
    protected void runAway()
    {
        Vector3 targetPos = (transform.position - GameInfo.Player.GetCenter()).normalized * 2;
        
        _agent.SetDestination(transform.position + targetPos);
    }

    // old code for flocking enemies.
    /*
    protected List<KeyValuePair<Enemy, float>> scanAllies()
    {
        List<KeyValuePair<Enemy, float>> enemyDistances = new List<KeyValuePair<Enemy, float>>();
        
        foreach (Enemy e in GameInfo.Enemies)
        {
            enemyDistances.Add(new KeyValuePair<Enemy, float>(e, Vector3.Distance(e.transform.position, transform.position)));
        }
        
        enemyDistances.Remove(new KeyValuePair<Enemy, float>(GetComponent<Enemy>(), 0f));
            
        return enemyDistances.OrderBy(x => x.Value).ToList();
    }
    */
    
    public virtual int getFlip()
    {
        return 1;
    }
}
