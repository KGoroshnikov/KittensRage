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

    public override void Launch(Vector3 vel, Vector3? finalpos = null)
    {
        //base.Launch(vel);
        startPos = transform.position;
        endPos = finalpos ?? Vector3.zero;
        rb.isKinematic = true;
        dir = vel.normalized;
        active = true;
        if (trailVFX != null)trailVFX.Play();
        throwSound.Play();
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
        hitSound.Play();
        if (trailVFX != null)trailVFX.Stop();
        if (AmountOfAvaliableDamage == 0) return;
        
        if (other.gameObject.TryGetComponent<HealthManager>(out var manager))
        {
            if (active){
                manager.ChangeHealth(-damageOnTouch);
            }
            else{
                manager.ChangeHealth(-hitDamange * (float)AmountOfAvaliableDamage / (float)maxAmountDmg);
                AmountOfAvaliableDamage--;
                if (AmountOfAvaliableDamage <= 0)
                    Invoke("PlayVFX", timeDespawn);
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