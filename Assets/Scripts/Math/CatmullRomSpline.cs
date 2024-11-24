using UnityEngine;
using UnityEngine.Serialization;

namespace Math
{
    [ExecuteInEditMode]
    public class CatmullRomSpline : MonoBehaviour
    {
        [SerializeField] private bool autoUpdate = true;
        [SerializeField] private Transform[] controlPoints;
        [Space, SerializeField, Range(10, 1000)] private int segments = 50;
        
        
#if UNITY_EDITOR
        private void Update()
        {
            if (!autoUpdate) return;
            if (controlPoints.Length != transform.childCount)
                controlPoints = new Transform[transform.childCount];
            for (var i = 0; i < transform.childCount; i++)
                controlPoints[i] = transform.GetChild(i);
        }
#endif

        private void OnDrawGizmos()
        {
            // Anchors
            Gizmos.color = Color.yellow;
            foreach (var t in controlPoints)
            {
                if (t == null) continue;
                var position = t.position;
                Gizmos.DrawLine(position + Vector3.back, position + Vector3.forward);
                Gizmos.DrawLine(position + Vector3.down, position + Vector3.up);
                Gizmos.DrawLine(position + Vector3.left, position + Vector3.right);
            }

            if (controlPoints.Length < 4) return;
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

        public Vector3 Get(int i) => controlPoints[i + 2].position;
        public Vector3 Compute(int i, float t)
        {
            var pt = CatmullRom(
                controlPoints[i].position, 
                controlPoints[i + 1].position,
                controlPoints[i + 2].position,
                controlPoints[i + 3].position,
                t);
            return pt;
        }

        public Vector3 Compute(float t)
        {
            var x = t * LinearLength;
            var prev = Get(-1);
            for (var i = 0; i < Count; i++)
            {
                var curr = Get(i);
                var len = Vector3.Distance(prev, curr);
                if (x < len) return Compute(i, x / len);
                x -= len;
                prev = curr;
            }
            return Get(Count - 1);
        }

        public int Count => controlPoints.Length - 3;
        public int Segments => segments;

        public float LinearLength
        {
            get {
                var length = 0f;
                var prev = Get(-1);
                for (var i = 0; i < Count; i++)
                {
                    var curr = Get(i);
                    length += Vector3.Distance(prev, curr);
                    prev = curr;
                }
                return length;
            }
        }


        private static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            var t2 = t * t;
            var t3 = t2 * t;

            return 0.5f * (2 * p1 + (-p0 + p2) * t +
                           (2 * p0 - 5 * p1 + 4 * p2 - p3) * t2 +
                           (-p0 + 3 * p1 - 3 * p2 + p3) * t3);
        }
    }
}