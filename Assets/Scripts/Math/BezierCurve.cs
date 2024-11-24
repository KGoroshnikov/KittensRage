using System.Collections.Generic;
using UnityEngine;

namespace Math
{
    [ExecuteInEditMode]
    public class BezierCurve : MonoBehaviour
    {
        [SerializeField] private bool autoUpdate = true;
        [SerializeField] private BezierPoint[] controlPoints;
        [Space, SerializeField, Range(10, 1000)] private int segments = 50;
        
        
#if UNITY_EDITOR
        private void Update()
        {
            if (!autoUpdate) return;
            var points = new List<BezierPoint>();
            for (var i = 0; i < transform.childCount; i++)
                if (transform.GetChild(i).TryGetComponent<BezierPoint>(out var point))
                    points.Add(point);
            controlPoints = points.ToArray();
        }
#endif
        
        private void OnDrawGizmos()
        {
            // Anchors
            Gizmos.color = Color.yellow;
            foreach (var t in controlPoints)
            {
                if (t == null) continue;
                var position = t.transform.position;
                Gizmos.DrawLine(position + Vector3.back, position + Vector3.forward);
                Gizmos.DrawLine(position + Vector3.down, position + Vector3.up);
                Gizmos.DrawLine(position + Vector3.left, position + Vector3.right);
            }
            
            if (controlPoints.Length < 2) return;
            // Line
            Gizmos.color = Color.green;
            var previousPoint = Compute(0);
            for (var j = 1; j <= Segments; j++)
            {
                var t = j / (float)Segments;
                var point = Compute(t);
                Gizmos.DrawLine(previousPoint, point);
                previousPoint = point;
            }
        }

        public BezierPoint Get(int i) => controlPoints[i];
        public Vector3 Compute(int i, float t)
        {
            return Bezier(controlPoints[i], controlPoints[i + 1], t);
        }
        public Vector3 Compute(float t)
        {
            var x = t * LinearLength;
            var prev = controlPoints[0].transform.position;
            for (var i = 1; i < Count; i++)
            {
                var curr = controlPoints[i].transform.position;
                var len = Vector3.Distance(prev, curr);
                if (x < len) return Compute(i - 1, x / len);
                x -= len;
                prev = curr;
            }
            return controlPoints[Count - 1].transform.position;
        }

        public int Count => controlPoints.Length;
        public int Segments => segments;

        public float LinearLength
        {
            get {
                var length = 0f;
                var prev = controlPoints[0].transform.position;
                for (var i = 1; i < Count; i++)
                {
                    var curr = controlPoints[i].transform.position;
                    length += Vector3.Distance(prev, curr);
                    prev = curr;
                }
                return length;
            }
        }

        private static Vector3 Bezier(BezierPoint a, BezierPoint b, float t)
        {
            var a0 = a.transform.position;
            var a1 = a0 + a.transform.forward * a.forward;
            
            var b0 = b.transform.position;
            var b1 = b0 - b.transform.forward * b.back;
            
            var a01 = Vector3.Lerp(a0, a1, t);
            var ab1 = Vector3.Lerp(a1, b1, t);
            var b10 = Vector3.Lerp(b1, b0, t);
            
            var ab01 = Vector3.Lerp(a01, ab1, t);
            var ab10 = Vector3.Lerp(ab1, b10, t);
            return Vector3.Lerp(ab01, ab10, t);
        }
    }
}