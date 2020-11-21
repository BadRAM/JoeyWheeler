using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


// This card type instantiates a prefab where the player is looking rather than in the player's head.
[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/TargetSpell", order = 400)]
public class InstantiateAtLookTarget : Card
{
    public float range = Mathf.Infinity;
    public GameObject prefab;
    public string[] raycastLayerMask = {"Terrain", "HitBox"}; // Things that the raycast can hit. remove HitBox to make it only care about map geometry
    public direction spawnOrientation = direction.WorldUpAligned;
    
    public enum direction
    {
        Zero,
        WorldUpAligned,
        WorldUpRandom,
        NormalUpAligned,
        NormalUpRandom
    }

    protected override void Action()
    {
        Debug.DrawRay(_origin, _forward, Color.red, 5);
        LayerMask mask = LayerMask.GetMask(raycastLayerMask);
        RaycastHit hit;
        if (Physics.Raycast(_origin, _forward, out hit, range, mask))
        {
            Quaternion rot = Quaternion.identity;
            switch (spawnOrientation)
            {
                case direction.WorldUpAligned:
                    rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(_forward, Vector3.up).normalized, Vector3.up);
                    break;
                
                case direction.WorldUpRandom:
                    rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(Random.insideUnitSphere, Vector3.up).normalized, Vector3.up);
                    break;

                case direction.NormalUpAligned:
                    rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(_forward, hit.normal).normalized, hit.normal);
                    break;
                
                case direction.NormalUpRandom:
                    rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(Random.insideUnitSphere, hit.normal).normalized, hit.normal);
                    break;
            }
            if (spawnOrientation == direction.NormalUpAligned)
            {
                rot = Quaternion.LookRotation(Vector3.ProjectOnPlane(_forward, hit.normal).normalized, hit.normal);
            }

            Instantiate(prefab, hit.point, rot);
        }
    }
}
