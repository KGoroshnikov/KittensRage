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
    
    [SerializeField] private AudioSource source;

    void Start(){
        //Invoke("PlayExplosion", 2);
    }

    void Update(){
        //if (Input.GetKeyDown(KeyCode.Space)) PlayExplosion();
        if (!animLight) return;
        t += Time.deltaTime / lightTime;
        t = Mathf.Clamp(t, 0, 1);
        if (t == 0 || t == 1) light.intensity = 0;
        else light.intensity = Mathf.Lerp(0, maxLight, Mathf.Sin(t * 3.14f));
    }

    public void PlayExplosion(){
        t = 0;
        if (destroyAfter) Destroy(gameObject, timeDestroy);
        vfx.Play();
        source.Play();
        animLight = true;
    }
}
