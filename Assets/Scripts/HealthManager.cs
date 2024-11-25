using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private float healthMax = 100;
    [SerializeField] private float health;
    [SerializeField] private UnityEvent onZeroHealth;
    [SerializeField] private UnityEvent<float, float> onHealthChanged;

    public float HealthMax => healthMax;
    public float Health => health;
    
    private void Awake() => health = healthMax;
    
    public void ChangeHealth(float delta)
    {
        health += delta;
        if (health > healthMax) health = healthMax;
        if (health < 0) onZeroHealth.Invoke();
        else onHealthChanged.Invoke(health, healthMax);
    }

    public void DestroyThis() => Destroy(gameObject);
}