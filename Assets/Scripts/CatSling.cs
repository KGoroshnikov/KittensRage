using System;
using Math;
using Projectiles;
using UnityEngine;

public class CatSling : MonoBehaviour
{
    [SerializeField] private Transform anchor;
    [SerializeField] private float simDt = 0.1f;
    
    [Space, SerializeField, Range(0, 60)] private float angle = 45;
    [SerializeField, Range(3, 50)] private float force = 10;
    
    
    [Space]
    public Vector3 target;
    public ThrowableCat Cat;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(target + Vector3.back, target + Vector3.forward);
        Gizmos.DrawLine(target + Vector3.down, target + Vector3.up);
        Gizmos.DrawLine(target + Vector3.left, target + Vector3.right);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) Recompute();
        if (Input.GetKeyDown(KeyCode.Space)) Cat.Send(this);
    }


    public void Recompute()
    {
        Vector3 prev, pos = anchor.transform.position;
        RaycastHit hit;
        var t = 0f;
        do
        {
            prev = pos;
            t += simDt;
            pos = ComputePath(t);
            Debug.DrawLine(prev, pos, Color.cyan, 10);
            if (t > 100) return;
        } while (!Physics.Linecast(prev, pos, out hit));
        target = hit.point;
    }

    public Vector3 ComputePath(float t)
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
    public Vector3 ComputePathVelocity(float t)
    {
        var localGravity = anchor.transform.worldToLocalMatrix 
                           * new Vector4(Physics.gravity.x, Physics.gravity.y, Physics.gravity.z, 0);
        var vel = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ) * force;
        var localPos = ComputeTrajectoryVelocity(vel, localGravity, t);
        return anchor.transform.localToWorldMatrix * new Vector4(localPos.x, localPos.y, 0, 0);
    }

    private Vector2 ComputeTrajectory(Vector2 start, Vector2 velocity, Vector2 gravity, float time) => 
        start + (velocity + gravity * (time * 0.5f)) * time;
    
    private Vector2 ComputeTrajectoryVelocity(Vector2 velocity, Vector2 gravity, float time) => 
        velocity + gravity * time;
}
