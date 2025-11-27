using System;
using UnityEngine;

public class ColliderCallback : MonoBehaviour
{
    //[Header("Settings")]
    //[Header("References")]
    //[Header("Input")]
    //[Header("Output")]

    public Action<Collider> _OnTriggerEnter;

    private void OnTriggerEnter(Collider other)
    {
        _OnTriggerEnter?.Invoke(other);
    }
}