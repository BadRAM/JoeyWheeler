using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
