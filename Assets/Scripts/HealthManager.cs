using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private float healthMax = 100;
    [SerializeField] private float health;
    [SerializeField] private Transform breakVFX;
    [SerializeField] private UnityEvent onZeroHealth;
    [SerializeField] private UnityEvent<float, float> onHealthChanged;
    [SerializeField] private UnityEvent onHealthChangedMini;

    public float HealthMax => healthMax;
    public float Health => health;
    
    private void Awake() => health = healthMax;
    
    public void ChangeHealth(float delta)
    {
        health += delta;
        if (health > healthMax) health = healthMax;
        if (health < 0){
            if (breakVFX != null) breakVFX.SetParent(null);
            onZeroHealth.Invoke();
        }
        else{
            onHealthChanged.Invoke(health, healthMax);
            onHealthChangedMini.Invoke();
        }
    }

    public void ChangeMaxHealth(float delta)
    {
        healthMax += delta;
        health += delta;
    }

    public void DestroyThis() => Destroy(gameObject);
}