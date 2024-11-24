using System;
using UnityEngine;

public class ThrowableCat : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float animationSpeed = 1;
    private CatSling catSling;

    private void Awake()
    {
        rb.useGravity = false;
    }

    private float time;
    private bool sent;
    public void Send(CatSling sling)
    {
        catSling = sling;
        catSling.Cat = null;
        sent = true;
        time = 0;
    }

    public void FixedUpdate()
    {
        if (!sent) return;
        transform.position = catSling.ComputePath(time);
        time += Time.fixedDeltaTime * animationSpeed;
    }

    private void OnCollisionEnter()
    {
        transform.parent = null;
        sent = false;
        rb.useGravity = true;
        rb.linearVelocity = catSling.ComputePathVelocity(time);
    }
}