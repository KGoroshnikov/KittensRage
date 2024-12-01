using UnityEngine;
using UnityEngine.VFX;

public class ThrowableCat : MonoBehaviour
{
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected float hitDamange;
    [SerializeField] protected float timeDespawn = 5;
    public GameManager.catTypes type;
    [SerializeField] protected int AmountOfAvaliableDamage;
    [SerializeField] protected AudioSource throwSound;
    [SerializeField] protected AudioSource hitSound;
    protected int maxAmountDmg;

    [SerializeField] protected VisualEffect vfxPuff;
    [SerializeField] protected VisualEffect trailVFX;

    public virtual void Start(){
        maxAmountDmg = AmountOfAvaliableDamage;
    }

    public virtual void Launch(Vector3 vel, Vector3? finalPos = null){
        rb.AddForce(vel, ForceMode.Impulse);
        if (trailVFX != null)trailVFX.Play();
        throwSound.Play();
    }

    public virtual void MakeMeKinematic(bool _state){
        rb.isKinematic = _state;
    }

    public virtual float GetMass(){
        return rb.mass;
    }

    public virtual void SetVelocity(Vector3 vel){
        rb.linearVelocity = vel;
    }

    public virtual Vector3 GetCustomGravity(){
        return Physics.gravity;
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        hitSound.Play();
        if (trailVFX != null)trailVFX.Stop();
        if (AmountOfAvaliableDamage == 0) return;
        if (other.gameObject.TryGetComponent<HealthManager>(out var manager))
        {
            manager.ChangeHealth(-hitDamange * (float)AmountOfAvaliableDamage / (float)maxAmountDmg);
            AmountOfAvaliableDamage--;
            if (AmountOfAvaliableDamage <= 0)
                Invoke("PlayVFX", timeDespawn);
                //Destroy(gameObject, timeDespawn);
        }
    }

    protected void PlayVFX(){
        if (vfxPuff != null){
            vfxPuff.transform.SetParent(null);
            vfxPuff.Play();
        }
        Destroy(gameObject);
    }
}