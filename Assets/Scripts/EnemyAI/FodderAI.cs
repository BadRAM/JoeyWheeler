using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FodderAI : AI
{
    [SerializeField] private float approachDistance = 10f; // close with player if further than this.
    [SerializeField] private float fleeDistance = 0f; // flee if player is closer than this. 
    [SerializeField] private float visionRange = 30f; // how far the enemy can see
//    [SerializeField] private float commRange = 5f; // when this enemy can see the player, it will notify other enemies within this radius
    [SerializeField] private float persistence = 3f; // how long will the ai continue to chase the player after it's lost sight of them?
    [SerializeField] private int moveFrequency = 100;
    [SerializeField] private float moveRadius = 5;
    
    private float _lastSawPlayer;
    private bool _alerted;
    private Vector3 _destination;

    public override void AiBehavior()
    {
        float distanceToPlayer = DistanceToPlayer();
        if (distanceToPlayer <= visionRange && CanSeePlayer())
        {
            _alerted = true; 
            _lastSawPlayer -= Time.time;
        }
        else
        {
            if (_alerted && Time.time - _lastSawPlayer > persistence)
            {
                _alerted = false;
            }
        }

        if (_alerted)
        {
            if (distanceToPlayer < fleeDistance)
            {
                RunAwayFromPlayer();
            }
            else if (distanceToPlayer > approachDistance)
            {
                ApproachPlayer();
            }
            else
            {
                Fire();
                _agent.SetDestination(transform.position);
            }
        }
        else
        {
            if (Random.Range(0, moveFrequency) < 2)
            {
                _agent.SetDestination(GetCenter() + Random.insideUnitSphere * moveRadius);
                Debug.Log("Moving randomly");
            }
        }
    }
}

