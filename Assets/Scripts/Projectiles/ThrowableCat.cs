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

        private float time;
        public bool IsSent { get; private set; }

        public virtual void Send(CatSling sling)
        {
            Sling = sling;
            Sling.Cat = null;
            IsSent = true;
            time = 0;
        }

        protected virtual void FixedUpdate()
        {
            if (!IsSent) return;
            transform.position = Sling.ComputePath(time);
            time += Time.fixedDeltaTime * animationSpeed;
        }

        protected virtual void OnCollisionEnter()
        {
            if (!IsSent) return;
            transform.parent = null;
            IsSent = false;
            rb.useGravity = true;
            rb.linearVelocity = Sling.ComputePathVelocity(time);
        }
    }
}