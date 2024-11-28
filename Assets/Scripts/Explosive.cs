using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Explosive : MonoBehaviour
{
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
        foreach (var coll in Physics.OverlapSphere(transform.position, Radius))
            if (coll.gameObject != gameObject)
            {
                var dst = Vector3.Distance(coll.transform.position, transform.position);

                if (coll.TryGetComponent(out Explosive explosive))
                {
                    explosive.Invoke(nameof(Explode), chainReactionDelay);
                    continue;
                }

                if (coll.TryGetComponent(out HealthManager manager))
                {
                    var dmgScale = Radius / dst;
                    var damage = damageMin * dmgScale * dmgScale;
                    Debug.Log($"Object {coll.name} damaged: {damage}");
                    manager.ChangeHealth(-damage);
                }

                if (coll.TryGetComponent(out Rigidbody rb))
                    rb.AddExplosionForce(force, transform.position, Radius);
            }
        
        //system.transform.localScale *= Radius * radiusExplosionMul;
        system.transform.SetParent(null);
        system.PlayExplosion();
        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
    }
}