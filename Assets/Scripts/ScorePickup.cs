using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ScorePickup : MonoBehaviour
{
    [SerializeField] private List<Transform> Spawns;
    private int _currentSpawn;

    private void Start()
    {
        _currentSpawn = 0;
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("ScorePickupSpawn"))
        {
            Spawns.Add(i.transform);
            if (Vector3.Distance(transform.position, i.transform.position) <=
                Vector3.Distance(transform.position, Spawns[_currentSpawn].position))
            {
                _currentSpawn = Spawns.Count - 1;
            }
        }
        transform.position = Spawns[_currentSpawn].position;
        transform.rotation = Spawns[_currentSpawn].rotation;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Pickup(other.transform.parent.GetComponent<Player>());
        }
    }

    private void Pickup(Player p)
    {
//        p.SwitchWeapon();
//        p.IncrementScore();
        
        // randomly select a spawn aside from the current one.
        int i = Random.Range(0, Spawns.Count);
        if (i >= _currentSpawn)
        {
            i += 1;
        }

        _currentSpawn = i;
        transform.position = Spawns[_currentSpawn].position;
        transform.rotation = Spawns[_currentSpawn].rotation;
        
        Score.Increment(1);
    }
}
