using UnityEngine;

namespace Projectiles
{
    public class ThrowableCat : MonoBehaviour
    {
        [SerializeField] private float animationSpeed = 1;
        protected CatSling Sling { get; set; }
        private Rigidbody rb;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
        }

        protected float Time { get; private set; }
        public bool IsSent { get; protected set; }

        public virtual void Send(CatSling sling)
        {
            Sling = sling;
            Sling.Cat = null;
            IsSent = true;
            Time = 0;
        }

        protected void FixedUpdate()
        {
            if (!IsSent) return;
            transform.position = Sling.ComputePath(Time);
            Time += UnityEngine.Time.fixedDeltaTime * animationSpeed;
            InFlightUpdate();
        }

        protected virtual void InFlightUpdate() {}

        protected virtual void OnCollisionEnter()
        {
            if (!IsSent) return;
            transform.parent = null;
            IsSent = false;
            rb.useGravity = true;
            rb.linearVelocity = Sling.ComputePathVelocity(Time);
        }
    }
}