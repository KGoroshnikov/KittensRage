using System;
using UnityEngine;

public class TwinsThrowableCat : ThrowableCat
{
    private int twinsRemain = 2;
    [SerializeField] private Rigidbody[] childsRB;
    [SerializeField] private Vector3 offsetVel;
    [SerializeField] private Animator animator;

    public override void Launch(Vector3 vel)
    {
        animator.enabled = false;
        if (childsRB[0] != null){
            childsRB[0].transform.SetParent(null);
            childsRB[0].AddForce(vel, ForceMode.Impulse);
        }
        if (childsRB[1] != null) {
            childsRB[1].transform.SetParent(null);
            childsRB[1].AddForce(vel + offsetVel, ForceMode.Impulse);
        }
    }

    public override void MakeMeKinematic(bool _state)
    {
        if (childsRB[0] != null)childsRB[0].isKinematic = _state;
        if (childsRB[1] != null)childsRB[1].isKinematic = _state;
    }

    public override void SetVelocity(Vector3 vel)
    {
        if (childsRB[0] != null)childsRB[0].linearVelocity = vel;
        if (childsRB[1] != null)childsRB[1].linearVelocity = vel;
    }

    public override float GetMass()
    {
        if (childsRB[0] != null) return childsRB[0].mass;
        if (childsRB[1] != null) return childsRB[1].mass;
        return 1;
    }

    public void TwinDidDamage(GameObject who, HealthManager hpDamaged){
        Debug.Log("A");
        hpDamaged.ChangeHealth(-hitDamange);

        Destroy(who, timeDespawn);
        twinsRemain--;
        if (twinsRemain <= 0) Destroy(gameObject, timeDespawn);
    }

    protected override void OnCollisionEnter(Collision other)
    {
        // no
    }
}