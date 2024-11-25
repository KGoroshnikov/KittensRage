using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI
{
    public class ArcherAI : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private GameObject projectilePrefab;
        [Space]
        [SerializeField] private Transform arrowSource;
        [SerializeField] private float attackTime = 1;
        [SerializeField] private float arrowSpeed = 10;
        [SerializeField, Range(0, 0.25f)] private float noiseFactor = 0.025f;
        private void Start()
        {
            InvokeRepeating(nameof(Attack), attackTime, attackTime);
        }

        public void Attack()
        {
            if (!target) CancelInvoke(nameof(Attack));
            var velocity = ComputeArrowVelocity().normalized * (1 - noiseFactor);
            velocity += Random.insideUnitSphere * noiseFactor;
            velocity *= arrowSpeed;
            
            var projectile = Instantiate(projectilePrefab, arrowSource.position, Quaternion.identity, null);
            arrowSource.transform.forward = velocity.normalized;
            projectile.transform.forward = velocity.normalized;
            if (projectile.TryGetComponent(out Rigidbody rb))
                rb.linearVelocity = velocity;
        }

        private Vector3 ComputeArrowVelocity()
        {
            var t = Vector3.Distance(target.position, transform.position) / arrowSpeed;
            var velocity = ComputeVelocity(t).normalized * arrowSpeed;
            for (var i = 0; i < 10; i++)
            {
                t = ComputeTime(velocity);
                velocity = ComputeVelocity(t).normalized * arrowSpeed;
            }
            return velocity;
        }

        private float ComputeTime(Vector3 velocity)
        {
            var delta = target.position - transform.position;
            var det = Vector3.Scale(velocity, velocity) + 2 * Vector3.Scale(Physics.gravity, delta);
            var sqrt = new Vector3(Mathf.Sqrt(det.x), Mathf.Sqrt(det.y), Mathf.Sqrt(det.z));
            var sub = velocity - sqrt;
            var add = velocity + sqrt;

            var sum = 0f;
            var div = 3;
            SelectOptimalTime(delta.x, sub.x, add.x, ref sum, ref div);
            SelectOptimalTime(delta.y, sub.y, add.y, ref sum, ref div);
            SelectOptimalTime(delta.z, sub.z, add.z, ref sum, ref div);
            return sum * 2 / div;
        }

        private static void SelectOptimalTime(float delta, float sub, float add, ref float sum, ref int div)
        {
            if (delta * sub > 0)
            {
                if (delta * add > 0)
                    sum += Mathf.Min(delta / add, delta / sub);
                else sum += delta / sub;
            }
            else if (delta * add > 0) sum += delta / add;
            else div -= 1;
        }

        private Vector3 ComputeVelocity(float time) => 
            (target.position - transform.position) / time - Physics.gravity * (time * 0.5f);
    }
}