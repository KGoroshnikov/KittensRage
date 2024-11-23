using System;
using Math;
using UnityEngine;

public class Catshot : MonoBehaviour
{
    [SerializeField] private Transform anchor;
    [SerializeField] private float simDt = 0.1f;
    
    [Space, SerializeField, Range(0, 60)] private float angle = 45;
    [SerializeField, Range(3, 50)] private float force = 10;
    
    
    [Space]public Vector3 target;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(target + Vector3.back, target + Vector3.forward);
        Gizmos.DrawLine(target + Vector3.down, target + Vector3.up);
        Gizmos.DrawLine(target + Vector3.left, target + Vector3.right);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Recompute(angle, force);
    }


    public void Recompute(float angle, float force)
    {
        Vector3 prev, pos = anchor.transform.position;
        RaycastHit hit;
        var t = 0f;
        do
        {
            prev = pos;
            t += simDt;
            pos = ComputePath(angle, force, t);
            Debug.DrawLine(prev, pos, Color.cyan, 10);
            if (t > 100) return;
        } while (!Physics.Linecast(prev, pos, out hit));
        target = hit.point;
    }

    public Vector3 ComputePath(float angle, float force, float t)
    {
        var localGravity = anchor.transform.worldToLocalMatrix 
                           * new Vector4(Physics.gravity.x, Physics.gravity.y, Physics.gravity.z, 0);
        var vel = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ) * force;
        var localPos = ComputeTrajectory(Vector2.zero, vel, localGravity, t);
        return anchor.transform.localToWorldMatrix * new Vector4(localPos.x, localPos.y, 0, 1);
    }

    private Vector2 ComputeTrajectory(Vector2 start, Vector2 velocity, Vector2 gravity, float time) => 
        start + (velocity + gravity * (time * 0.5f)) * time;
}
