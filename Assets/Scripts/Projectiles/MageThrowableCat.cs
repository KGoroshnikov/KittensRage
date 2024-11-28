using UnityEngine;
using UnityEngine.Serialization;

namespace Projectiles
{
    public class MageThrowableCat : ThrowableCat
    {
        [SerializeField] private GameObject fireballPrefab;
        [SerializeField] private float fireballSpeed = 100;
        [SerializeField] private float fireballSpawnDistance = 3;
        
        private bool abilityUsed;
        
        
        protected void InFlightUpdate()
        {
            if (abilityUsed) return;
            if (!Input.GetMouseButton(0)) return;
            
            var camera = Camera.main;
            if (!camera) return;
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit)) return;
            Debug.Log("Mage Activated!");
            SendFireball(hit);
            abilityUsed = false;
        }

        private void SendFireball(RaycastHit hit)
        {
            var dir = (hit.point - transform.position).normalized;
            var projectile = Instantiate(fireballPrefab, transform.position + dir * fireballSpawnDistance, Quaternion.identity, null);
            projectile.transform.forward = dir;
            if (projectile.TryGetComponent(out Rigidbody rb))
                rb.AddForce(dir * fireballSpeed, ForceMode.Impulse);
        }
    }
}