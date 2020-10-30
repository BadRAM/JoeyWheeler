﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [SerializeField] private float maxHealth = 200;
    [SerializeField] private float startingHealth = 100;
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI weaponText;
    [SerializeField] private TextMeshProUGUI bossTimerText;
    [SerializeField] private TextMeshProUGUI interactPrompt;
    [SerializeField] private List<Weapon> weapons;
    [SerializeField] private Transform raycastOrigin;
    private int _weapon;
    private float _health;
    private int _score;

    private FPSWalk _walker;
    private Controls _input;

    private void Awake()
    {
        _input = new Controls();
        _input.Player.Enable();
        _input.Player.Use.performed += ctx => Use();
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
        _health = startingHealth;

        foreach (Weapon weapon in weapons)
        {
            weapon.enabled = false;
        }

        weapons[0].enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = "Health: " + _health;
        difficultyText.text = "Difficulty: " + _score;
        bossTimerText.text = "Boss spawns in: " + GameInfo.TimeToBossSpawn;

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
        
        

        GameInfo.RunTime += Time.deltaTime;
        GameInfo.TimeToBossSpawn -= Time.deltaTime;
    }

    // select a random weapon that is not the currently selected weapon
    public void SwitchWeapon()
    {
        weapons[_weapon].enabled = false;
        int nextWep = Random.Range(0, weapons.Count - 1);
        if (nextWep < _weapon)
        {
            _weapon = nextWep;
        }
        else
        {
            _weapon = nextWep + 1;
        }
        weapons[_weapon].enabled = true;
        weaponText.text = weapons[_weapon].GetWeaponName();
    }
    
    public void Hurt(float damage)
    {
        _health -= damage;
        
        if (_health <= 0)
        {
            //Die, I guess?
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

    private void Use()
    {
        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, 2f)
            && hit.transform.CompareTag("UseTarget"))
        {
            hit.transform.GetComponent<UseTarget>().Use();
        }
    }
}
