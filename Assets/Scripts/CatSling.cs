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
    [SerializeField] private int maxTrajectoryPoints = 100;
    [SerializeField] private LineRenderer lineRenderer;
    
    [Space, Range(0, 60)] public float maxAngle = 60;
    [Range(0, 50)] public float maxForce = 50;
    private float currentAngle;
    private float currentForce;
    public Quaternion forwardFixer;
    
    [Space]
    public Vector3 target;
    [FormerlySerializedAs("Cat")] public ThrowableCat cat;
    [FormerlySerializedAs("Queue")] public List<ThrowableCat> queue;

    private Matrix4x4 w2l;
    private Matrix4x4 l2w;

    private Vector2 startTouchPos;
    private bool isTouching;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(target + Vector3.back, target + Vector3.forward);
        Gizmos.DrawLine(target + Vector3.down, target + Vector3.up);
        Gizmos.DrawLine(target + Vector3.left, target + Vector3.right);
    }

    public void GetCats(List<ThrowableCat> throwableCats){
        queue.AddRange(throwableCats);
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.R)) Recompute();
        if (Input.GetKeyDown(KeyCode.Space) && !cat.IsSent)
        {
            cat.Send(this);
            LoadMatrices();
            animator.Play("Launch", -1, 0);
        }*/

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                isTouching = true;
                startTouchPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                UpdateCatapult(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isTouching = false;
                LaunchCat();
            }
        }

        if (cat || queue.Count <= 0) return;
        cat = queue[0];
        queue.RemoveAt(0);
        cat.transform.SetParent(anchor);
        cat.transform.localPosition = Vector3.zero;
    }

    private void UpdateCatapult(Vector2 touchPos)
    {
        Vector2 dragVector = touchPos - startTouchPos;
        currentAngle = Mathf.Clamp(maxAngle * (dragVector.y / Screen.height), 0, maxAngle);
        currentForce = Mathf.Clamp(maxForce * (dragVector.magnitude / Screen.width), 0, maxForce);
        RotateAnchor(-currentAngle);
        Recompute();
    }

    private void LaunchCat()
    {
        if (cat != null && !cat.IsSent)
        {
            cat.Send(this);
            animator.Play("Launch", -1, 0);
        }
    }

    public void RotateAnchor(float angle) => anchor.Rotate(forwardFixer * anchor.right, angle);

    private void LoadMatrices()
    {
        w2l = anchor.transform.worldToLocalMatrix;
        l2w = anchor.transform.localToWorldMatrix;
    }

    /*public void Recompute()
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
    }*/
    public void Recompute()
    {
        LoadMatrices();
        List<Vector3> trajectoryPoints = new List<Vector3>();

        Vector3 prev, pos = anchor.transform.position;
        RaycastHit hit;
        var t = 0f;

        for (int i = 0; i < maxTrajectoryPoints; i++)
        {
            prev = pos;
            t += simDt;
            pos = ComputePath(t);
            trajectoryPoints.Add(pos);

            if (Physics.Linecast(prev, pos, out hit))
            {
                target = hit.point;
                trajectoryPoints.Add(target);
                break;
            }
        }

        DrawTrajectory(trajectoryPoints);
    }
    private void DrawTrajectory(List<Vector3> points)
    {
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
        lineRenderer.enabled = true;
    }

    public Vector3 ComputePath(float t)
    {
        var localGravity = w2l * new Vector4(Physics.gravity.x, Physics.gravity.y, Physics.gravity.z, 0);
        var vel = new Vector2(
            Mathf.Cos(currentAngle * Mathf.Deg2Rad),
            Mathf.Sin(currentAngle * Mathf.Deg2Rad)
        ) * currentForce;
        var localPos = ComputeTrajectory(Vector2.zero, vel, localGravity, t);
        return l2w * new Vector4(localPos.x, localPos.y, 0, 1);
    }
    public Vector3 ComputePathVelocity(float t)
    {
        var localGravity = w2l * new Vector4(Physics.gravity.x, Physics.gravity.y, Physics.gravity.z, 0);
        var vel = new Vector2(
            Mathf.Cos(currentAngle * Mathf.Deg2Rad),
            Mathf.Sin(currentAngle * Mathf.Deg2Rad)
        ) * currentForce;
        var localPos = ComputeTrajectoryVelocity(vel, localGravity, t);
        return l2w * new Vector4(localPos.x, localPos.y, 0, 0);
    }

    private Vector2 ComputeTrajectory(Vector2 start, Vector2 velocity, Vector2 gravity, float time) => 
        start + (velocity + gravity * (time * 0.5f)) * time;
    
    private Vector2 ComputeTrajectoryVelocity(Vector2 velocity, Vector2 gravity, float time) => 
        velocity + gravity * time;
}
