using System;
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
        private HealthManager target;
        
        private void UpdateBrain()
        {
            if (agent.pathStatus != NavMeshPathStatus.PathComplete)
            {
                // Moving
                return;
            }
            // Attacking
            target.ChangeHealth(-damage);
        }

        private void FixedUpdate()
        {
            if (target) return;
            CancelInvoke(nameof(UpdateBrain));
            FindNewTarget();
        }

        public void FindNewTarget()
        {
            var transforms = new List<HealthManager>();
            foreach (var group in targetGroups)
                group.GetComponentsInChildren(false, transforms);

            var minDst = float.MaxValue;
            var targetPosition = transform.position;
            foreach (var t in transforms)
            {
                var dst = Vector3.Distance(t.transform.position, transform.position);
                if (dst > minDst) return;
                targetPosition = t.transform.position;
                minDst = dst;
                target = t;
            }
            
            agent.SetDestination(targetPosition);
            InvokeRepeating(nameof(UpdateBrain), attackTime, attackTime);
        }
    }
}