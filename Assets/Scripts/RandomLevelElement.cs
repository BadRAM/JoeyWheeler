using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLevelElement : MonoBehaviour
{
    [SerializeField] private float SpawnChance;

    // Start is called before the first frame update
    void Start()
    {
        if (Random.value < SpawnChance)
        {
            DestroyImmediate(gameObject);
        }
    }
}
