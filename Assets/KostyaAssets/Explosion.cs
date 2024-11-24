using UnityEngine;
using UnityEngine.VFX;

public class Explosion : MonoBehaviour
{
    public bool destroyAfter;
    public float timeDestroy;
    [SerializeField] private VisualEffect vfx;
    [SerializeField] private Light light;
    private bool animLight;
    [SerializeField] private float lightTime = 0.5f;
    private float t;
    private float maxLight = 1000;

    void Start(){
        //Invoke("PlayExplosion", 2);
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Space)) PlayExplosion();
        if (!animLight) return;
        t += Time.deltaTime / lightTime;
        t = Mathf.Clamp(t, 0, 1);
        light.intensity = Mathf.Lerp(0, maxLight, Mathf.Sin(t * 3.14f));
    }

    public void PlayExplosion(){
        t = 0;
        if (destroyAfter) Destroy(gameObject, timeDestroy);
        vfx.Play();
        animLight = true;
    }
}
