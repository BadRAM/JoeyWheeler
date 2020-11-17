using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AI for the boss monster

public class BossAI : AI
{
    [SerializeField] private float approachDistance = 10f; // close with player if further than this.
    [SerializeField] private float fleeDistance = 0f; // flee if player is closer than this. 

    private AITarget _target;

    protected override void Start()
    {
        base.Start();
        _target = new AITarget(FindObjectOfType<Player>().transform);
    }

    public override void AiBehavior()
    {
        float distanceToPlayer = _target.Distance(_monster.GetCenter());
        if (distanceToPlayer < fleeDistance)
        {
            RunAwayFromTarget(_target);
        }
        else if (distanceToPlayer > approachDistance)
        {
            ApproachTarget(_target);
        }
        else
        {
            Fire(_target);
        }
    }
}
