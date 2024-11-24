using UnityEngine;

namespace Math
{
    public class BezierPoint : MonoBehaviour
    {
       
        public float forward;
        public float back;
        
        private void OnDrawGizmos()
        {
            // Anchors
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position - transform.forward * back, transform.position + transform.forward * forward);
        }
    }
}