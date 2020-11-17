using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

// The main player controller class. Handles health, hud, and score, but not movement. Interfaces with weapon scripts in prefabs to attack, and with an FPSWalk for movement.

public class Player : MonoBehaviour
{
    [SerializeField] private float maxHealth = 200;
    [SerializeField] private float startingHealth = 100;
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI weaponText;
    [SerializeField] private TextMeshProUGUI bossTimerText;
    [SerializeField] private TextMeshProUGUI interactPrompt;
    [SerializeField] private Canvas deathScreen;
    [SerializeField] private Canvas winScreen;
    [SerializeField] private Canvas pauseScreen;
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private Transform hand;
    private float _health;
    private int _score;

    private Weapon _weapon;
    private Transform _weaponTransform;

    private FPSWalk _walker;
    private Controls _input;

    private void Awake()
    {
        _input = new Controls();
        _input.Player.Enable();
        _input.Player.Use.performed += ctx => Use();
//        _input.Player.Pause.performed += ctx => TogglePause();
        _input.Player.Fire.performed += ctx => FirePressed();
        _input.Player.Fire.canceled += ctx => FireReleased();
        _input.Player.AltFire.performed += ctx => AltFirePressed();
        _input.Player.AltFire.canceled += ctx => AltFireReleased();
    }

    // Start is called before the first frame update
    void Start()
    {
        List<Transform> spawns = new List<Transform>();

        foreach (GameObject i in GameObject.FindGameObjectsWithTag("ScorePickupSpawn"))
        {
            spawns.Add(i.transform);
        }
        transform.position = spawns[Random.Range(0, spawns.Count-1)].position;
        
        _walker = GetComponent<FPSWalk>();
        
        GameInfo.Player = GetComponent<Player>();
        GameInfo.State = GameInfo.GameState.Active;
        _health = startingHealth;

        deathScreen.enabled = false;

        Time.timeScale = 1;
        
        Cursor.lockState = CursorLockMode.Locked;

        EquipRandomWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = "Health: " + _health;
        difficultyText.text = "Difficulty: " + GameInfo.GetDifficultyModifier();
        bossTimerText.text = "Boss spawns in: " + GameInfo.TimeToBossSpawn;
        if (_weapon != null)
        {
            weaponText.text = _weapon.GetWeaponName() + ", " + _weapon.Ammo;
        }
        else
        {
            weaponText.text = "no weapon";
        }

        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, 2f)
            && hit.transform.CompareTag("UseTarget"))
        {
            interactPrompt.text = hit.transform.GetComponent<UseTarget>().Description();
        }
        else
        {
            interactPrompt.text = "";
        }
        
        // update various aspects of game state.
        // these are wildly inefficient, and should be changed to only trigger on state change.
        if (GameInfo.Paused)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            deathScreen.enabled = false;
            winScreen.enabled = false;
            _walker.LockLook = true;
        }
        else
        {
            switch (GameInfo.State)
            {
                case GameInfo.GameState.Active:
                    Time.timeScale = 1;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    deathScreen.enabled = false;
                    winScreen.enabled = false;
                    _walker.LockLook = false;
                    break;
                
                case GameInfo.GameState.Failure:
                    Time.timeScale = 0;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    deathScreen.enabled = true;
                    winScreen.enabled = false;
                    _walker.LockLook = true;
                    break;
                
                case GameInfo.GameState.Victory:
                    Time.timeScale = 0;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    deathScreen.enabled = false;
                    winScreen.enabled = true;
                    _walker.LockLook = true;         
                    break;
            }
        }
                


        GameInfo.RunTime += Time.deltaTime;
        GameInfo.TimeToBossSpawn = Mathf.Max(0, GameInfo.TimeToBossSpawn - Time.deltaTime);
    }

    public void Hurt(float damage)
    {
        _health -= damage;

        if (_health < 0)
        {
            GameInfo.State = GameInfo.GameState.Failure;
        }
    }

    public void IncrementScore()
    {
        _score += 1;
    }    
    
    public void IncrementScore(int unhelpfulVariableName)
    {
        _score += unhelpfulVariableName;
    }

    public Vector3 GetCenter()
    {
        return _walker.GetCenter();
    }

    private void FirePressed()
    {
        if (_weapon != null)
        {
            _weapon.FirePressed();
        }
    }

    private void FireReleased()
    {
        if (_weapon != null)
        {
            _weapon.FireReleased();
        }
    }

    private void AltFirePressed()
    {
        if (_weapon != null)
        {
            _weapon.AltFirePressed();
        }
    }

    private void AltFireReleased()
    {
        if (_weapon != null)
        {
            _weapon.AltFireReleased();
        }
    }

    private void Use()
    {
        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, 2f)
            && hit.transform.CompareTag("UseTarget"))
        {
            hit.transform.GetComponent<UseTarget>().Use();
        }
    }
    
    public void EquipRandomWeapon()
    {
        EquipWeapon(weapons[Random.Range(0, weapons.Length)]);
    }

    private void EquipWeapon(GameObject weapon)
    {
        if (_weapon != null)
        {
            _weapon.Drop();
        }
        _weaponTransform = Instantiate(weapon, hand).transform;
        _weapon = _weaponTransform.GetComponent<Weapon>();
        _weapon.SetPlayer(this);
        _weapon.SetRaycastOrigin(raycastOrigin);
        weaponText.text = _weapon.name;
    }

    // clears weapon variables, only meant to be called from Weapon.DropWeapon
    public void DropWeapon()
    {
        _weapon = null;
        _weaponTransform = null;
    }
}
