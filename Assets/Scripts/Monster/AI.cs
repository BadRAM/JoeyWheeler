using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;

// Abstract class that monster AIs must implement.

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyWeapon))]
public abstract class AI : MonoBehaviour
{
    protected EnemyWeapon _weapon;
    protected NavMeshAgent _agent;
    protected Monster _monster;
    //protected string _state;
    
    protected virtual void Start()
    {
        _weapon = GetComponent<EnemyWeapon>();
        _agent = GetComponent<NavMeshAgent>();
        _monster = GetComponent<Monster>();
    }

    public abstract void AiBehavior();

    protected bool Fire(AITarget target)
    {
        return _weapon.Fire(target.GetCenter());
    }
    
    protected void RunAwayFromTarget(AITarget target)
    {
        Vector3 targetPos = (transform.position - target.GetCenter()).normalized * 2;
        
        _agent.SetDestination(transform.position + targetPos);
        
    }

    protected void ApproachTarget(AITarget target)
    {
        if(_agent.enabled){
            //_agent.enabled = true;
            _agent.SetDestination(target.GetCenter());
        //}
        //else{
        //    _agent.enabled = false;
        }
    }

    protected Vector3 GetCenter()
    {
        return transform.position;
    }

    // find the nearest hostile monster or player in line of sight, return an AITarget reference to it.
    protected AITarget FindNearestTarget()
    {
        AITarget best = null;
        List<AITarget> targets = new List<AITarget>();
        Debug.Log("Finding target" + _monster);
        if (_monster.Team != 0)
        {
            targets.Add(new AITarget(FindObjectOfType<Player>().transform));
            Debug.Log("Targets of "+targets[0].Name()+" found");
        }
        
        foreach (Monster i in FindObjectsOfType<Monster>())
        {
            if (i.IsAlive() && i.Team != _monster.Team)
            {
                targets.Add(new AITarget(i.transform));
                Debug.DrawLine(_monster.GetCenter(), i.GetCenter(), Color.blue);
            }
        }

        foreach (AITarget i in targets)
        {
            if (best == null || i.Distance(_monster.GetCenter()) > best.Distance(_monster.GetCenter()) && i.CanSeeFrom(_monster.GetCenter()))
            {
                best = i;
            }
        }
        
        return best;
    }
}
