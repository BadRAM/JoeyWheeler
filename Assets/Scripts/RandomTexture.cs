using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTexture : MonoBehaviour
{
    public MatList wallTexs;
    public MatList floorTexs;
    
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.materials[0] = wallTexs.GetRandomMat();
        mr.materials[1] = floorTexs.GetRandomMat();
    }
}
