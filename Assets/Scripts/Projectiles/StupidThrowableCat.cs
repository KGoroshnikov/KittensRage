using UnityEngine;

public class StupidThrowableCat : ThrowableCat
{
    [SerializeField] private Vector3 customGravity;

    void Awake(){
        rb.useGravity = false;
    }

    public override Vector3 GetCustomGravity()
    {
        return customGravity;
    }

    void FixedUpdate(){
        rb.AddForce(customGravity, ForceMode.Acceleration);
    }
}