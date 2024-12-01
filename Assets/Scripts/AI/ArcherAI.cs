using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI
{
    public class ArcherAI : MonoBehaviour
    {
        private Transform target;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Animator animator;
        [Space]
        [SerializeField] private Transform arrowSource;
        [SerializeField] private float attackTime = 1;
        [SerializeField] private float arrowSpeed = 10;
        [SerializeField] private float radiusAttack;
        [SerializeField, Range(0, 0.25f)] private float noiseFactor = 0.025f;
        
        [SerializeField] private AudioSource shootArrow;
        
        private enum state{
            idle, attack
        }
        private state m_state;
        private bool allowedToAttack;
        private void Start()
        {
            m_state = state.idle;
            animator.Play("IdleGuard", -1, Random.value);
            target = GameObject.FindWithTag("GigaCat").transform;
        }

        public void AllowAttack(bool _active){
            allowedToAttack = _active;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radiusAttack);
        }

        void Update(){
            if (target == null){
                if (m_state != state.idle){
                    m_state = state.idle;
                    animator.SetTrigger("Idle");
                }
                return;
            }
            if (!allowedToAttack) return;

            if (m_state == state.attack) {
                Vector3 dir = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.y);
                transform.forward = -dir;
                return;
            }
            if (Vector3.Distance(transform.position, target.position) <= radiusAttack){
                m_state = state.attack;
                animator.SetTrigger("Attack");
                InvokeRepeating(nameof(Attack), attackTime, attackTime);
            }
        }

        public void Attack()
        {
            if (!target || Vector3.Distance(transform.position, target.position) > radiusAttack || !allowedToAttack){
                CancelInvoke(nameof(Attack));
                if (m_state != state.idle){
                    m_state = state.idle;
                    animator.SetTrigger("Idle");
                }
                return;
            }
            var velocity = ComputeArrowVelocity().normalized * (1 - noiseFactor);
            velocity += Random.insideUnitSphere * noiseFactor;
            velocity *= arrowSpeed;
            
            var projectile = Instantiate(projectilePrefab, arrowSource.position, Quaternion.identity, null);
            arrowSource.transform.forward = velocity.normalized;
            projectile.transform.forward = velocity.normalized;
            if (projectile.TryGetComponent(out Rigidbody rb))
                rb.AddForce(velocity, ForceMode.Impulse);
            
            shootArrow.Play();
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