using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Button which loads a scene

public class SceneLoadButton : MonoBehaviour
{
    public string SceneToLoad;

    public void Load()
    {
        SceneManager.LoadScene(SceneToLoad);
    }
}
