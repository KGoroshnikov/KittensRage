using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Explosive : MonoBehaviour
{
    [SerializeField] private Transform[] affectedGroups;
    
    [SerializeField] private Explosion system;
    [SerializeField] private float force = 500f;
    [SerializeField] private float radiusFactor = 0.3f;
    [SerializeField] private float radiusExplosionMul = 0.2f;
    [SerializeField] private float damageMin = 1f;
    [SerializeField] private float chainReactionDelay = 0.3f;
    
    [Space, SerializeField] private bool explodeByClick = true;
    
    private float Radius => Mathf.Sqrt(force) * radiusFactor;

    public void Update()
    {
        if (!explodeByClick || !Input.GetKeyDown(KeyCode.E)) return;
        Explode();
    }


    public void Explode()
    {
        var rigidbodies = new List<Rigidbody>();
        foreach (var group in affectedGroups)
            group.GetComponentsInChildren(false, rigidbodies);

        foreach (var rb in rigidbodies)
            if (rb.gameObject != gameObject)
            {
                var dst = Vector3.Distance(rb.transform.position, transform.position);
                if (dst > Radius) continue;

                if (rb.TryGetComponent(out Explosive explosive))
                {
                    explosive.Invoke(nameof(Explode), chainReactionDelay);
                    continue;
                }

                if (rb.TryGetComponent(out HealthManager manager))
                {
                    var dmgScale = Radius / dst;
                    var damage = damageMin * dmgScale * dmgScale;
                    Debug.Log($"Object {rb.name} damaged: {damage}");
                    manager.ChangeHealth(-damage);
                }

                rb.AddExplosionForce(force, transform.position, Radius);
            }
        
        system.transform.localScale *= Radius * radiusExplosionMul;
        system.transform.parent = null;
        system.PlayExplosion();
        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}