
using System;
using UnityEngine;

public class VelocityDamageHandler : MonoBehaviour
{
    [SerializeField] private float safeVelocityLimit = 10;
    [SerializeField] private float damageMultiplier = 1;


    private void OnCollisionEnter(Collision other)
    {
        var vel = other.relativeVelocity.magnitude;
        if (vel < safeVelocityLimit) return;
        var damage = vel * vel * damageMultiplier * 0.1f;
        // TODO: Maybe use idk??? Highly relative to engine collision tracking approach
        // if (other.gameObject.TryGetComponent<VelocityDamageHandler>(out _))
        // {
        //     if (other.gameObject.TryGetComponent<HealthManager>(out var manager2))
        //     {
        //         var halfDamage = damage * 0.5f;
        //         if (manager2.Health > halfDamage) 
        //             damage = halfDamage;
        //         manager2.ChangeHealth(-damage);
        //     }
        // }
        if (TryGetComponent<HealthManager>(out var manager1)) manager1.ChangeHealth(-damage);
        Debug.Log($"Objects {name} and {other.gameObject.name} collided too hard with damage {damage}!");
    }
}