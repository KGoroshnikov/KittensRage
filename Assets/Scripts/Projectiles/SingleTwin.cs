using UnityEngine;

public class SingleTwin : MonoBehaviour
{
    [SerializeField] private TwinsThrowableCat twinsThrowableCat;
    private bool damaged;

    void OnCollisionEnter(Collision other)
    {
        if (damaged) return;
        if (other.gameObject.TryGetComponent<HealthManager>(out var manager))
        {
            damaged = true;
            twinsThrowableCat.TwinDidDamage(gameObject, manager);
        }
    }
}
