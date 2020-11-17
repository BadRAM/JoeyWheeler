using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A simple class which represents an AI's target. Does not inherit from monobehaviour

public class AITarget
{
    private Transform _target;

    public bool Alive()
    {
        if (_target.CompareTag("Player"))
        {
            return true;
        }

        if (_target.CompareTag("Monster"))
        {
            return _target.GetComponent<Monster>().IsAlive();
        }

        return false;
    }

    public Vector3 GetCenter()
    {
        if (_target.CompareTag("Player"))
        {
            return _target.GetComponent<Player>().GetCenter();
        }

        if (_target.CompareTag("Monster"))
        {
            return _target.GetComponent<Monster>().GetCenter();
        }
        return _target.position;
    }

    public float Distance(Vector3 from)
    {
        return Vector3.Distance(GetCenter(), from);
    }

    public bool CanSeeFrom(Vector3 from)
    {
        return !Physics.Linecast(from, GetCenter(), LayerMask.GetMask("Terrain", "Default"));
    }

    public int Team()
    {
        if (_target.CompareTag("Player"))
        {
            return 0;
//            return _target.GetComponent<Player>().Team;
        }

        if (_target.CompareTag("Monster"))
        {
            return _target.GetComponent<Monster>().Team;
        }

        return 0;
    }

    public AITarget(Transform target)
    {
        _target = target;
    }

    public string Name()
    {
        return _target.name;
    }
}
