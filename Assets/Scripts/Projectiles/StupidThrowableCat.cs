using UnityEngine;

public class StupidThrowableCat : ThrowableCat
{
    [SerializeField] private Vector3 customGravity;

    void Awake(){
        rb.useGravity = false;
    }

    void FixedUpdate(){
        rb.AddForce(customGravity, ForceMode.Acceleration);
    }
}