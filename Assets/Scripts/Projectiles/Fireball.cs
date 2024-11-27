using System;
using UnityEngine;

namespace Projectiles
{
    public class Fireball : MonoBehaviour
    {
        [SerializeField] private Explosion system;
        [SerializeField] private float force = 500f;
        [SerializeField] private float radiusFactor = 0.3f;
        [SerializeField] private float radiusExplosionMul = 0.2f;
        [SerializeField] private float damageMin = 1f;
        [SerializeField] private float chainReactionDelay = 0.3f;
       
        private float Radius => Mathf.Sqrt(force) * radiusFactor;
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }

        private  void OnCollisionEnter(Collision other)
        {
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
        
            system.transform.localScale *= Radius * radiusExplosionMul;
            system.transform.parent = null;
            system.PlayExplosion();
            Destroy(gameObject);
        }
    }
}