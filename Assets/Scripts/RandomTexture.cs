using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTexture : MonoBehaviour
{
    public MatList wallTexs;
    public MatList floorTexs;

    public Renderer MR;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("oi m8 u got a loicense for that tex");
        if (MR == null)
        {
            MR = GetComponent<Renderer>();
        }
        Material[] mat = new Material[2];
        mat[0] = wallTexs.GetRandomMat();
        mat[1] = floorTexs.GetRandomMat();
        Debug.Log("Setting " + transform.name + "'s materials to " + mat[0].name + " and " + mat[1].name);
        MR.materials = mat;
    }
}
