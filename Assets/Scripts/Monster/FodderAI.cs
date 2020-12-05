using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Basic AI for boring monsters

public class FodderAI : AI
{
    [SerializeField] private float approachDistance = 10f; // close with player if further than this.
    [SerializeField] private float fleeDistance = 0f; // flee if player is closer than this. 
    [SerializeField] private float visionRange = 30f; // how far the enemy can see
//    [SerializeField] private float commRange = 5f; // when this enemy can see the player, it will notify other enemies within this radius
    [SerializeField] private float persistence = 3f; // how long will the ai continue to chase the player after it's lost sight of them?
    [SerializeField] private int moveFrequency = 100;
    [SerializeField] private float moveRadius = 5;

    private float _lastSawTarget;
    private float _lastKnownPos;
    private AITarget _target;
    public States _state = States.Idle;

    public enum States
    {
        Idle,
        Pursuit,
        AttackRanged,
        AttackMelee
    }

    public override void AiBehavior()
    {
        switch (_state)
        {
            case States.Idle:
                if (Random.Range(0, moveFrequency) < 1)
                {
                    _agent.SetDestination(GetCenter() + Random.insideUnitSphere * moveRadius);
                    _target = FindNearestTarget();
                    if (_target != null && _target.Distance(_monster.GetCenter()) < visionRange)
                    {
                        _state = States.Pursuit;
                    }
                    else
                    {
                        _target = null;
                    }
                }
                break;

            case States.Pursuit:
                if (_target != null)
                {
                    _state = States.Idle;
                    break;
                }

                float distanceToTarget = _target.Distance(_monster.GetCenter());
                if (distanceToTarget > visionRange || !_target.CanSeeFrom(_monster.GetCenter())
                ) // is the target obscured?
                {
                    if (Time.time - _lastSawTarget > persistence)
                    {
                        _target = null;
                        _state = States.Idle;
                        break;
                    }
                    else
                    {
                        ApproachTarget(_target);
                    }
                }
                else
                {
                    _lastSawTarget = Time.time;
                    if (distanceToTarget < fleeDistance)
                    {
                        RunAwayFromTarget(_target);
                    }
                    else if (distanceToTarget > approachDistance)
                    {
                        ApproachTarget(_target);
                    }
                    else
                    {
                        Fire(_target);
                        _agent.SetDestination(transform.position);
                    }
                }
                Debug.DrawLine(transform.position, _agent.destination, Color.green);
                break;
        }
    }
}

