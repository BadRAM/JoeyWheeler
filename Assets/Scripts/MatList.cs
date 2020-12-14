using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "NewMatList", menuName = "MatList", order = 0)]
public class MatList : ScriptableObject
{
    public List<Material> Materials;

    public Material GetRandomMat()
    {
        return Materials[Random.Range(0, Materials.Count)];
    }
}
