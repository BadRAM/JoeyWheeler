using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;
    [SerializeField] private UnityEvent trigger;

    public void Hurt(float damage)
    {
        trigger.Invoke();
        
        if (enemy != null)
        {
            enemy.Hurt(damage);
            return;
        }

        if (player != null)
        {
            player.Hurt(damage);
        }
    }
}
