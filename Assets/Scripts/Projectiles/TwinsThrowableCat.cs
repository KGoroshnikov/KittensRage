using System;
using UnityEngine;

namespace Projectiles
{
    public class TwinsThrowableCat : ThrowableCat
    {
        [SerializeField] private GameObject right;
        [SerializeField] private GameObject left;
        [SerializeField] private float twinsSplitRadius = 2;
        [SerializeField] private float twinsSplitTime = 1;
        [SerializeField] private float rotationSpeed = 180;
        private SphereCollider collider;
        
        private bool abilityUsed;
        private Rigidbody rbR;
        private Rigidbody rbL;

        protected override void Awake()
        {
            collider = GetComponent<SphereCollider>();
            if (right.TryGetComponent(out rbR))
            {
                rbR.useGravity = false;
                rbR.Sleep();
            }

            if (left.TryGetComponent(out rbL))
            {
                rbL.useGravity = false;
                rbL.Sleep();
            }
        }

        protected override void InFlightUpdate()
        {
            transform.Rotate(Vector3.forward, rotationSpeed * UnityEngine.Time.fixedDeltaTime);
            if (abilityUsed)
            {
                rbR.linearVelocity = Sling.ComputePathVelocity(Time);
                rbL.linearVelocity = Sling.ComputePathVelocity(Time);
                return;
            }
            if (!Input.GetMouseButton(0)) return;
            Debug.Log("Twins Activated!");
            SplitTwins();
            abilityUsed = true;
        }

        private void SplitTwins()
        {
            var shift = Vector3.forward * twinsSplitRadius;
            right.transform.localPosition += shift;
            left.transform.localPosition += shift;
            collider.radius += twinsSplitRadius;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsSent) return;
            if (other.gameObject == right) return;
            if (other.gameObject == left) return;
            
            rbR.useGravity = true;
            rbR.linearVelocity = Sling.ComputePathVelocity(Time);
            rbR.WakeUp();
            right.transform.parent = null;
            
            rbL.useGravity = true;
            rbL.linearVelocity = Sling.ComputePathVelocity(Time);
            rbL.WakeUp();
            left.transform.parent = null;
            
            IsSent = false;
            Destroy(gameObject);
        }
    }
}