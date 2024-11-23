using UnityEngine;

public class Explosive : MonoBehaviour
{
    [SerializeField] private ParticleSystem system;
    [SerializeField] private float force = 100f;
    [SerializeField] private float radiusFactor = 10f;
    
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
            if (o != gameObject && Vector3.Distance(o.transform.position, transform.position) < Radius)
            {
                if (o.TryGetComponent(out Explosive explosive))
                    explosive.Explode(objects);

                if (o.TryGetComponent<Rigidbody>(out var rb))
                    rb.AddExplosionForce(force, transform.position, Radius);
            }
        system.transform.localScale *= Radius;
        system.transform.parent = null;
        system.gameObject.SetActive(true);
        Destroy(gameObject);
    }
}