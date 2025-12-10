using System;
using UnityEngine;

public class ManaPickup : MonoBehaviour
{
    public RSO_Mana mana;
    public float amount = 20f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mana.Set(mana.Get() + amount);
            Destroy(gameObject);
        }
    }
}
