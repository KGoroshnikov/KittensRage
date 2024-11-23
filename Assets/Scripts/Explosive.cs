using UnityEngine;
using UnityEngine.Serialization;

public class Explosive : MonoBehaviour
{
    [SerializeField] private ParticleSystem system;
    [SerializeField] private float force = 500f;
    [SerializeField] private float radiusFactor = 0.3f;
    [SerializeField] private float damageMin = 1f;
    
    private float Radius => Mathf.Sqrt(force) * radiusFactor;

    public void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;
        var objects = FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Explode(objects);
    }

    public void Explode(GameObject[] objects)
    {
        foreach (var o in objects)
            if (o != gameObject)
            {
                var dst = Vector3.Distance(o.transform.position, transform.position);
                if (dst > Radius) continue;
                
                if (o.TryGetComponent(out Explosive explosive))
                {
                    explosive.Explode(objects);
                    continue;
                }

                if (o.TryGetComponent(out HealthManager manager))
                {
                    var damage = damageMin * Radius / dst;
                    Debug.Log($"Object {o.name} damaged: {damage}");
                    manager.ChangeHealth(-damage);
                }


                if (o.TryGetComponent<Rigidbody>(out var rb))
                    rb.AddExplosionForce(force, transform.position, Radius);
            }
        system.transform.localScale *= Radius;
        system.transform.parent = null;
        system.gameObject.SetActive(true);
        Destroy(gameObject);
    }
}