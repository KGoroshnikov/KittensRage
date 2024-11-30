using UnityEngine;

public class StupidThrowableCat : ThrowableCat
{
    //[SerializeField] private Vector3 customGravity;
    [SerializeField] private float timeReach;
    [SerializeField] private float forceAfter;
    [SerializeField] private float damageOnTouch;
    private Vector3 dir;
    private bool active;
    private float tlerp;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool mainDamaged;

    public override void Launch(Vector3 vel, Vector3? finalpos = null)
    {
        //base.Launch(vel);
        startPos = transform.position;
        endPos = finalpos ?? Vector3.zero;
        rb.isKinematic = true;
        dir = vel.normalized;
        active = true;
    }

    void Update(){
        if (!active) return;
        tlerp += Time.deltaTime / timeReach;
        transform.position = Vector3.Lerp(startPos, endPos, Functions.SmoothLerp(tlerp));
        if (tlerp >= 1){
            active = false;
            rb.isKinematic = false;
            rb.AddForce(forceAfter * dir, ForceMode.Impulse);
        }
    }

    protected override void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<HealthManager>(out var manager))
        {
            if (active){
                manager.ChangeHealth(-damageOnTouch);
            }
            else if (!mainDamaged){
                mainDamaged = true;
                manager.ChangeHealth(-hitDamange);
                Destroy(gameObject, timeDespawn);
            }
        }
    }

    void Awake(){
        //rb.useGravity = false;
    }

    void FixedUpdate(){
        //rb.AddForce(customGravity, ForceMode.Acceleration);
    }
}