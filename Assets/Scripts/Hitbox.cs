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
            player.Hurt((int)damage);
        }
    }
    public void Knockback( Vector3 point)
    {
        trigger.Invoke();
        
        if (monster != null)
        {
            Debug.Log("knockback");
            monster.Knockback(point);
            return;
        }

        if (player != null)
        {
            //player.Knockback();
        }
    }

    public int Team()
    {
        if (monster != null)
        {
            return monster.Team;
        }

        return 0;
    }
}
