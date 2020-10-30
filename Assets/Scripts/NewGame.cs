using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private float difficulty = 1;

    public void Load()
    {
        GameInfo.Reset(difficulty);
        SceneManager.LoadScene(sceneToLoad);
    }
}
