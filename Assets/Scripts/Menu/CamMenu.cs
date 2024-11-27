using UnityEngine;

public class CamMenu : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float amp;
    private Vector3 rot;
    private float t;

    void Start(){
        rot = transform.localEulerAngles;
    }

    void Update(){
        t += Time.deltaTime * speed;
        transform.localEulerAngles = rot + new Vector3(Mathf.Sin(t) * amp, Mathf.Cos(t) * amp, 0);
    }
}
