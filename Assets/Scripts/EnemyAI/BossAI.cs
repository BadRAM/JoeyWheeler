using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : AI
{
    [SerializeField] private float approachDistance = 10f; // close with player if further than this.
    [SerializeField] private float fleeDistance = 0f; // flee if player is closer than this. 
    
    public override void AiBehavior()
    {
        float distanceToPlayer = DistanceToPlayer();
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
        }
    }
}
