using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class GigaCatAI : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Transform[] targetGroups;
        [Space]
        [SerializeField] private float damage = 10;
        [SerializeField] private float attackTime = 1;
        
        [Space]
        [SerializeField] private float brainUpdateTime = 0.1f;
        private HealthManager target;

        [SerializeField] private HealthManager mhp;

        [SerializeField] private Animator animator;

        [SerializeField] private Transform hpOrigin;
        [SerializeField] private Transform hpPivot;
        private Transform cam;
        
        [Header("Audio")]
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip[] hitSounds;

        private enum state{
            idle, walk, attack 
        }
        private state m_state;

        void Start(){
            cam = Camera.main.transform;
            //FindNewTarget();
            animator.SetTrigger("Idle");
        }

        public HealthManager getMHP(){
            return mhp;
        } 

        public void WheelStop(){
            mhp.canttakedamage = true;
            CancelInvoke();
            agent.SetDestination(transform.position);
            m_state = state.idle;
            animator.SetTrigger("Idle");
        }

        public void CancelWheelBreak(){
            mhp.canttakedamage = false;
            m_state = state.walk;
            animator.SetTrigger("Walk");
            FindNewTarget();
        }

        public void StartGame(){
            m_state = state.walk;
            animator.SetTrigger("Walk");
            FindNewTarget();
        }

        public void StopMe(){
            CancelInvoke();
            agent.SetDestination(transform.position);
            m_state = state.idle;
            animator.SetTrigger("Idle");
        }

        void Update(){
            hpOrigin.rotation = Quaternion.LookRotation(-cam.forward);
        }

        public void UpdateHPBar(float health, float healthMax){
            hpPivot.localScale = new Vector3(health / healthMax, 1, 1);
        }
        
        private void MoveToTarget()
        {
            if (m_state == state.idle) return;

            if (!target)
            {
                // Target Not Exists
                CancelInvoke(nameof(MoveToTarget));
                FindNewTarget();
                return;
            }
            if (m_state != state.walk){
                m_state = state.walk;
                animator.ResetTrigger("Attack");
                animator.SetTrigger("Walk");
            }
            agent.SetDestination(target.transform.position);
            if (agent.remainingDistance > agent.stoppingDistance) return;
            // Attack target
            CancelInvoke(nameof(MoveToTarget));
            InvokeRepeating(nameof(AttackTarget), attackTime, attackTime);
        }
        private void AttackTarget()
        {
            if (m_state == state.idle) return;
            
            if (!target)
            {
                // Target not exists
                //CancelInvoke(nameof(AttackTarget));
                //FindNewTarget();
                findTarget();
                if (!target){
                    CancelInvoke(nameof(AttackTarget));
                    FindNewTarget();
                    return;
                }
            }
            agent.SetDestination(target.transform.position);
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                // Move to target
                CancelInvoke(nameof(AttackTarget));
                InvokeRepeating(nameof(MoveToTarget), brainUpdateTime, brainUpdateTime);
                return;
            }
            if (m_state != state.attack){
                m_state = state.attack;
                animator.ResetTrigger("Walk");
                animator.SetTrigger("Attack");
            }
            // Attacking
            source.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length)]);
            target.ChangeHealth(-damage);
        }

        void findTarget(){
            var transforms = new List<HealthManager>();
            foreach (var group in targetGroups)
                group.GetComponentsInChildren(false, transforms);
            
            var minDst = float.MaxValue;
            foreach (var t in transforms)
            {
                var dst = Vector3.Distance(t.transform.position, transform.position);
                if (dst > minDst) continue;
                minDst = dst;
                target = t;
            }
        }

        private void FindNewTarget()
        {
            if (m_state == state.idle) return;

            findTarget();
            
            // Move to target
            InvokeRepeating(nameof(MoveToTarget), brainUpdateTime, brainUpdateTime);
        }
    }
}