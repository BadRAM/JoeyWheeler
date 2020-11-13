using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Canvas topMenu;
    [SerializeField] private Canvas settingsMenu;

    private PauseState _state;

    private enum PauseState
    {
        Unpaused,
        Paused,
        Settings
    }

    private Controls _input;

    void Awake()
    {
        _input = new Controls();
        _input.Player.Enable();
        _input.Player.Pause.performed += ctx => ButtonPress();
    }

    // Start is called before the first frame update
    void Start()
    {
        topMenu.enabled = false;
        settingsMenu.enabled = false;
    }

    public void ButtonPress()
    {
        if (GameInfo.State == GameInfo.GameState.Active)
        {
            switch (_state)
            {
                case PauseState.Unpaused:
                    GameInfo.Paused = true;
                    topMenu.enabled = true;
                    settingsMenu.enabled = false; //redundant
                    _state = PauseState.Paused;
                    break;
            
                case PauseState.Paused:
                    GameInfo.Paused = false;
                    topMenu.enabled = false;
                    settingsMenu.enabled = false;
                    _state = PauseState.Unpaused;
                    break;
                
                case PauseState.Settings:
                    topMenu.enabled = true;
                    settingsMenu.enabled = false;
                    _state = PauseState.Paused;
                    break;
            }
        }
        else
        {
            GameInfo.Paused = false;
            topMenu.enabled = false;
            settingsMenu.enabled = false;
        }
    }

    public void OpenSettings()
    {
        topMenu.enabled = false;
        settingsMenu.enabled = true;
        FindObjectOfType<SettingsMenu>().ResetFields();
        _state = PauseState.Settings;
    }

    public void ReturnToTop()
    {
        topMenu.enabled = true;
        settingsMenu.enabled = false;
        _state = PauseState.Paused;
    }
}
