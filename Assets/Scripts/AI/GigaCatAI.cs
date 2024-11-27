using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

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
        
        private void MoveToTarget()
        {
            if (!target)
            {
                // Target Not Exists
                CancelInvoke(nameof(MoveToTarget));
                FindNewTarget();
                return;
            }
            agent.SetDestination(target.transform.position);
            if (agent.remainingDistance > agent.stoppingDistance) return;
            // Attack target
            CancelInvoke(nameof(MoveToTarget));
            InvokeRepeating(nameof(AttackTarget), attackTime, attackTime);
        }
        private void AttackTarget()
        {
            if (!target)
            {
                // Target not exists
                CancelInvoke(nameof(AttackTarget));
                FindNewTarget();
                return;
            }
            agent.SetDestination(target.transform.position);
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                // Move to target
                CancelInvoke(nameof(AttackTarget));
                InvokeRepeating(nameof(MoveToTarget), brainUpdateTime, brainUpdateTime);
                return;
            }
            // Attacking
            target.ChangeHealth(-damage);
        }

        private void FindNewTarget()
        {
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
            
            // Move to target
            InvokeRepeating(nameof(MoveToTarget), brainUpdateTime, brainUpdateTime);
        }

        private void Start() => FindNewTarget();
    }
}