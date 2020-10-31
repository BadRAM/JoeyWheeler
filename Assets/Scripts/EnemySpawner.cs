using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float DifficultyFactor;

    [SerializeField]
    private float Randomness; //The max amount of time that can be added or subtracted from the spawn time.

    private float SpawnCountDown;
    [SerializeField] private List<GameObject> enemyTypes;
    [SerializeField] private List<Transform> Waypoints;
    [SerializeField] private List<Vector3> _waypoints;


    private void Start()
    {
        foreach (Transform n in Waypoints)
        {
            _waypoints.Add(n.position);
            Destroy(n.gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (SpawnCountDown >= 0f)
        {
            SpawnCountDown -= Time.deltaTime;
        }
        else
        {
            SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        SpawnCountDown = DifficultyFactor; //+ Random.Range(-Randomness, Randomness);
        GameObject e = Instantiate(enemyTypes[Random.Range(0, enemyTypes.Count - 1)], transform.position, transform.rotation);
    }
}