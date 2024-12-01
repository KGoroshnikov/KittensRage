using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TwinsThrowableCat : ThrowableCat
{
    private int twinsRemain = 2;
    [SerializeField] private Rigidbody[] childsRB;
    [SerializeField] private VisualEffect[] puffVFX;
    [SerializeField] private VisualEffect[] trailsVFX;
    [SerializeField] private Vector3 offsetVel;
    [SerializeField] private Animator animator;
    private Dictionary<GameObject, int> childDamageCount = new Dictionary<GameObject, int>();

    public override void Start()
    {
        base.Start();
        childDamageCount.Add(childsRB[0].gameObject, AmountOfAvaliableDamage);
        childDamageCount.Add(childsRB[1].gameObject, AmountOfAvaliableDamage);
    }

    public override void Launch(Vector3 vel, Vector3? finalPos = null)
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
        for(int i = 0; i < trailsVFX.Length; i++) trailsVFX[i].Play();
        throwSound.Play();
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
        int aval = childDamageCount[who];
        hitSound.Play();
        hpDamaged.ChangeHealth(-hitDamange * (float)aval / (float)maxAmountDmg);

        childDamageCount[who] = childDamageCount[who] - 1;
        int idx = (childsRB[0] != null && who == childsRB[0].gameObject) ? 0 : 1;
        trailsVFX[idx].Stop();
        if (childDamageCount[who] <= 0){
            puffVFX[idx].transform.SetParent(null);
            puffVFX[idx].Play();
            Destroy(who);
            twinsRemain--;
        }
        if (twinsRemain <= 0) Destroy(gameObject, timeDespawn);
    }

    protected override void OnCollisionEnter(Collision other)
    {
        // no
    }
}