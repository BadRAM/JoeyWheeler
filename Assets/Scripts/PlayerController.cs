using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxHealth = 200;
    [SerializeField] private float startingHealth = 100;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI weaponText;
    [SerializeField] private List<Weapon> weapons;
    private int _weapon;
    private float _health;
    private int _score;
    
    // Start is called before the first frame update
    void Start()
    {
        GameInfo.Player = GetComponent<PlayerController>();
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
        healthText.text = _health.ToString();
        scoreText.text = _score.ToString();
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
        return transform.position + Vector3.up; // revise if player height is not constant, ie. crouching is added.
    }
}
