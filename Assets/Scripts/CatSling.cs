using System;
using System.Collections.Generic;
using Projectiles;
using UnityEngine;
using UnityEngine.Serialization;

public class CatSling : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform anchor;
    [SerializeField] private float simDt = 0.1f;
    
    [Space, Range(0, 60)] public float angle = 45;
    [Range(0, 50)] public float force = 10;
    
    
    [Space]
    public Vector3 target;
    [FormerlySerializedAs("Cat")] public ThrowableCat cat;
    [FormerlySerializedAs("Queue")] public List<ThrowableCat> queue;

    private Matrix4x4 w2l;
    private Matrix4x4 l2w;
    
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
        if (Input.GetKeyDown(KeyCode.Space) && !cat.IsSent)
        {
            // TODO: Здесь вызывать анимацию
            animator.Play("Launch", -1, 0);
            LoadMatrices();
            cat.Send(this);
        }

        if (cat || queue.Count <= 0) return;
        cat = queue[0];
        queue.RemoveAt(0);
        cat.transform.SetParent(anchor);
        cat.transform.localPosition = Vector3.zero;
    }

    public void RotateAnchor(float angle) => anchor.Rotate(anchor.forward, angle);

    private void LoadMatrices()
    {
        w2l = anchor.transform.worldToLocalMatrix;
        l2w = anchor.transform.localToWorldMatrix;
    }

    public void Recompute()
    {
        LoadMatrices();
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
        var localGravity = w2l * new Vector4(Physics.gravity.x, Physics.gravity.y, Physics.gravity.z, 0);
        var vel = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ) * force;
        var localPos = ComputeTrajectory(Vector2.zero, vel, localGravity, t);
        return l2w * new Vector4(localPos.x, localPos.y, 0, 1);
    }
    public Vector3 ComputePathVelocity(float t)
    {
        var localGravity = w2l * new Vector4(Physics.gravity.x, Physics.gravity.y, Physics.gravity.z, 0);
        var vel = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ) * force;
        var localPos = ComputeTrajectoryVelocity(vel, localGravity, t);
        return l2w * new Vector4(localPos.x, localPos.y, 0, 0);
    }

    private Vector2 ComputeTrajectory(Vector2 start, Vector2 velocity, Vector2 gravity, float time) => 
        start + (velocity + gravity * (time * 0.5f)) * time;
    
    private Vector2 ComputeTrajectoryVelocity(Vector2 velocity, Vector2 gravity, float time) => 
        velocity + gravity * time;
}
