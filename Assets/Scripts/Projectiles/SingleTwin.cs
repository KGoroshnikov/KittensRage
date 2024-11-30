using UnityEngine;

public class SingleTwin : MonoBehaviour
{
    [SerializeField] private TwinsThrowableCat twinsThrowableCat;

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<HealthManager>(out var manager))
        {
            twinsThrowableCat.TwinDidDamage(gameObject, manager);
        }
    }
}
