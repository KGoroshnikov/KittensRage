using UnityEngine;

namespace Projectiles
{
    public class MageThrowableCat : ThrowableCat
    {
        [SerializeField] private GameObject fireballPrefab;
        [SerializeField] private float speed;
        [SerializeField] private Vector3 normal;
        
        private bool abilityUsed;
        
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (abilityUsed || Input.touchCount <= 0) return;
            var touch = Input.GetTouch(0);
            
            var camera = Camera.main;
            if (!camera) return;
            var ray = camera.ScreenPointToRay(touch.position);
            if (!Physics.Raycast(ray, out var hit)) return;
            SendFireball(hit);
            abilityUsed = false;
        }

        private void SendFireball(RaycastHit hit)
        {
            var dir = hit.point - transform.position;
            var projectile = Instantiate(fireballPrefab, transform.position + dir, Quaternion.identity, null);
            projectile.transform.forward = dir;
            if (projectile.TryGetComponent(out Rigidbody rb))
                rb.linearVelocity = dir * speed;
        }
    }
}