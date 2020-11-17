using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private Player player;
    [FormerlySerializedAs("enemy")] [SerializeField] private Monster monster;
    [SerializeField] private UnityEvent trigger;

    public void Hurt(float damage)
    {
        trigger.Invoke();
        
        if (monster != null)
        {
            monster.Hurt(damage);
            return;
        }

        if (player != null)
        {
            player.Hurt(damage);
        }
    }
}
