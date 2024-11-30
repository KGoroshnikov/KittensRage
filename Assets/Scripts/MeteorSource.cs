using UnityEngine;
using Random = UnityEngine.Random;

public class MeteorSource : MonoBehaviour
{
    [SerializeField] private GameObject meteorPrefab;
    [SerializeField] private float fireballSpeed = 100;


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 1);
        Gizmos.DrawRay(transform.position, transform.forward);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, transform.lossyScale);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
            SpawnMeteor();
    }
#endif
   

    public void SpawnMeteor()
    {
        var position = transform.TransformPoint(Vector3.Scale(transform.lossyScale, 
            new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f)));
        var projectile = Instantiate(meteorPrefab, position, Quaternion.identity, null);
        projectile.transform.forward = transform.forward;
        if (projectile.TryGetComponent(out Rigidbody rb))
            rb.AddForce(transform.forward * fireballSpeed, ForceMode.Impulse);
    }
}