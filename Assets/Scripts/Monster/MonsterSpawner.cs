using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

// Placeholder monster spawning script.

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private float DifficultyFactor;

    [SerializeField]
    private float Randomness; //The max amount of time that can be added or subtracted from the spawn time.

    private float SpawnTimer;
    [FormerlySerializedAs("enemyTypes")] [SerializeField] private List<GameObject> monsterTypes;
    [SerializeField] private GameObject boss;
    [SerializeField] private List<Transform> Waypoints;
    [SerializeField] private List<Vector3> _waypoints;
    private List<GameObject> _spawnPoints;

    private bool _bossSpawned;


    private void Start()
    {
        foreach (Transform n in Waypoints)
        {
            _waypoints.Add(n.position);
            Destroy(n.gameObject);
        }

        _spawnPoints = GameObject.FindGameObjectsWithTag("ScorePickupSpawn").ToList();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (SpawnTimer <= 10f / GameInfo.GetDifficultyModifier())
        {
            SpawnTimer += Time.deltaTime;
        }
        else
        {
            SpawnTimer = 0;
            SpawnMonster();
        }

        if (!_bossSpawned && GameInfo.TimeToBossSpawn <= 0)
        {
            SpawnBoss();
            _bossSpawned = true;
        }
    }

    public void SpawnMonster()
    {
        GameObject type = monsterTypes[Random.Range(0, monsterTypes.Count - 1)];
        Vector3 position = _spawnPoints[Random.Range(0, _spawnPoints.Count - 1)].transform.position;
        GameObject e = Instantiate(type, position, transform.rotation);
    }
    
    public void SpawnBoss()
    {
        Vector3 position = _spawnPoints[Random.Range(0, _spawnPoints.Count - 1)].transform.position;
        GameObject e = Instantiate(boss, position, transform.rotation);
    }
}