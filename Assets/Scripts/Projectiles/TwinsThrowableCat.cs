using UnityEngine;

namespace Projectiles
{
    public class TwinsThrowableCat : ThrowableCat
    {
        [SerializeField] private GameObject right;
        [SerializeField] private float twinsSplitRadius;
        [SerializeField] private float rotationSpeed = 180;
        
        
        private bool abilityUsed;
        
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (abilityUsed)
            {
                transform.Rotate(Vector3.forward, rotationSpeed * Time.fixedDeltaTime);
                return;
            }
            if (Input.touchCount <= 0) return;
            SplitTwins();
            abilityUsed = true;
        }

        private void SplitTwins()
        {
            // TODO: Splitting Twins
            
        }

        protected override void OnCollisionEnter()
        {
            base.OnCollisionEnter();
            transform.DetachChildren();
            Destroy(gameObject);
        }
    }
}