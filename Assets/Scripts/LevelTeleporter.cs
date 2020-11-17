using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

// Teleports from one level to the next. WIP

public class LevelTeleporter : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    private int _currentSpawn;

    public void Start()
    {
        List<Transform> spawns = new List<Transform>();

        foreach (GameObject i in GameObject.FindGameObjectsWithTag("ScorePickupSpawn"))
        {
            spawns.Add(i.transform);
        }

        transform.position = spawns[Random.Range(0, spawns.Count-1)].position;
    }

    public void Use()
    {
        GameInfo.NewLevel();
        SceneManager.LoadScene(sceneToLoad);
    }
}
