
using System;
using UnityEngine;

public class FallDamageHandler : MonoBehaviour
{
    [SerializeField] private float safeVelocityLimit = 10;
    [SerializeField] private float damageMultiplier = 1;


    private void OnCollisionEnter(Collision other)
    {
        var vel = other.relativeVelocity.magnitude;
        if (vel < safeVelocityLimit) return;
        var damage = vel * vel * damageMultiplier * 0.1f;
        if (other.gameObject.TryGetComponent<FallDamageHandler>(out _)) damage *= 0.5f;
        
        if (TryGetComponent<HealthManager>(out var manager1)) manager1.ChangeHealth(-damage);
        if (other.gameObject.TryGetComponent<HealthManager>(out var manager2)) manager2.ChangeHealth(-damage);
        
        Debug.Log($"Objects {name} and {other.gameObject.name} collided too hard with damage {damage}!");
    }
}