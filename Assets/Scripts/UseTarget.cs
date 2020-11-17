using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Attach this script to usable objects so that the player can press use to use them.
// TODO: expand this class to support swapping cards from the player's hand.

public class UseTarget : MonoBehaviour
{
    [SerializeField] private string description;
    [SerializeField] private UnityEvent useEvent;

    public void Use()
    {
        useEvent.Invoke();
    }

    public string Description()
    {
        return description;
    }
}
