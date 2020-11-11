using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Canvas topMenu;
    [SerializeField] private Canvas settingsMenu;
    
    // Start is called before the first frame update
    void Start()
    {
        topMenu.enabled = false;
        settingsMenu.enabled = false;
    }

    public void SetPaused(bool isPaused)
    {
        if (isPaused)
        {
            topMenu.enabled = true;
            settingsMenu.enabled = false;
        }
        else
        {
            topMenu.enabled = false;
            settingsMenu.enabled = false;
        }
    }

    public void OpenSettings()
    {
        topMenu.enabled = false;
        settingsMenu.enabled = true;
        FindObjectOfType<SettingsMenu>().ResetFields();
    }

    public void ReturnToTop()
    {
        topMenu.enabled = true;
        settingsMenu.enabled = false;
    }
}
