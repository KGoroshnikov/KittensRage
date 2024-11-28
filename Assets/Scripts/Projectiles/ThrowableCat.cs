using UnityEngine;

namespace Projectiles
{
    public class ThrowableCat : MonoBehaviour
    {
        [SerializeField] private float animationSpeed = 1;
        protected CatSling Sling { get; private set; }
        private Rigidbody rb;

        protected virtual void Awake()
        {
            return;
            rb = GetComponent<Rigidbody>();
            if (!rb) return;
            rb.useGravity = false;
            rb.Sleep();
        }

        protected float AnimationSpeed => animationSpeed;
        protected float Time { get; private set; }
        public bool IsSent { get; protected set; }

        public virtual void Send(CatSling sling)
        {
            return;
            if (TryGetComponent(out Animator animator)) animator.enabled = false;
            
            Sling = sling;
            transform.parent = null;
            IsSent = true;
            Time = 0;
        }

        protected void FixedUpdate()
        {
            return;
            if (!IsSent) return;
            transform.position = Sling.ComputePath(Time);
            
            var velocity = Sling.ComputePathVelocity(Time);
            // transform.forward = velocity.normalized;
            if (rb) rb.linearVelocity = velocity;
            
            Time += UnityEngine.Time.fixedDeltaTime * animationSpeed;
        }

        protected virtual void InFlightUpdate() {}

        protected virtual void OnCollisionEnter(Collision collision)
        {
            return;
            if (!IsSent) return;
            Sling.cat = null;
            transform.parent = null;
            IsSent = false;
            if (!rb) return;
            rb.useGravity = true;
            rb.WakeUp();
        }
    }
}