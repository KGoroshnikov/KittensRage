using UnityEngine;
using UnityEngine.Serialization;

public class MageThrowableCat : ThrowableCat
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballSpeed = 100;
    [SerializeField] private float fireballSpawnDistance = 3;
    
    private bool abilityUsed;
    private float maxHeight;
    private bool waitingToFireball;
    private Transform king;

    void Start(){
        king = GameObject.FindWithTag("KingRat").transform;
    }

    public override void Launch(Vector3 vel)
    {
        base.Launch(vel);
        maxHeight = 0;
        waitingToFireball = true;
    }

    void FixedUpdate(){
        if (!waitingToFireball) return;
        if (transform.position.y >= maxHeight) maxHeight = transform.position.y;
        else if (!abilityUsed){ // выпускает шар на максимальной высоте
            SendFireball();
            abilityUsed = true;
        }
    }

    private void SendFireball()
    {
        GameObject[] allrats = GameObject.FindGameObjectsWithTag("Rat");
        Transform closetsRat;
        if (allrats.Length > 0){
            closetsRat = allrats[0].transform;
            float distClosest = Vector3.Distance(transform.position, closetsRat.position);
            foreach(GameObject rat in allrats)
                if (Vector3.Distance(transform.position, rat.transform.position) < distClosest){
                    distClosest = Vector3.Distance(transform.position, rat.transform.position);
                    closetsRat = rat.transform;
                }
        }
        else closetsRat = king;

        var dir = new Vector3(Random.value, Random.value, 0);
        if (closetsRat != null) dir = (closetsRat.position - transform.position).normalized;
        var projectile = Instantiate(fireballPrefab, transform.position + dir * fireballSpawnDistance, Quaternion.identity, null);
        projectile.transform.forward = dir;
        if (projectile.TryGetComponent(out Rigidbody rb))
            rb.AddForce(dir * fireballSpeed, ForceMode.Impulse);
    }
}