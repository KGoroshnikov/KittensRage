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
        private Collider collider;
        
        private bool abilityUsed;
        private float time;

        protected override void Awake()
        {
            collider = GetComponent<Collider>();
            for (var i = 0; i < transform.childCount; i++)
                if (transform.GetChild(i).TryGetComponent<Rigidbody>(out var rb)) 
                    rb.useGravity = false;
        }

        protected override void InFlightUpdate()
        {
            transform.Rotate(Vector3.forward, rotationSpeed * UnityEngine.Time.fixedDeltaTime);
            if (abilityUsed)
            {
                if (time > 1) return;
                var dt = UnityEngine.Time.fixedDeltaTime / twinsSplitTime;
                right.transform.localPosition += transform.right * (twinsSplitRadius * dt);
                left.transform.localPosition -= transform.right * (twinsSplitRadius * dt);
                time += dt;
                return;
            }
            if (Input.touchCount <= 0) return;
            SplitTwins();
            abilityUsed = true;
        }

        private void SplitTwins()
        {
            collider.bounds.Expand(twinsSplitRadius);
            time = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsSent) return;
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.useGravity = true;
                    rb.linearVelocity = Sling.ComputePathVelocity(Time);
                }
                child.parent = null;
            }
            IsSent = false;
            Destroy(gameObject);
        }
    }
}