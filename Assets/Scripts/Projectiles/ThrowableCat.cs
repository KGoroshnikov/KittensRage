using UnityEngine;

namespace Projectiles
{
    public class ThrowableCat : MonoBehaviour
    {
        [SerializeField] private float animationSpeed = 1;
        private CatSling catSling;
        private Rigidbody rb;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
        }

        private float time;
        private bool sent;
        public void Send(CatSling sling)
        {
            catSling = sling;
            catSling.Cat = null;
            sent = true;
            time = 0;
        }

        protected virtual void FixedUpdate()
        {
            if (!sent) return;
            transform.position = catSling.ComputePath(time);
            time += Time.fixedDeltaTime * animationSpeed;
        }

        protected virtual void OnCollisionEnter()
        {
            if (sent)
            {
                transform.parent = null;
                sent = false;
                rb.useGravity = true;
                rb.linearVelocity = catSling.ComputePathVelocity(time);
            }
        }
    }
}