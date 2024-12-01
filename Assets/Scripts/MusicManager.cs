using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private bool fadeMusic;
    private bool islowersound;
    private float timeFade;
    private float maxVol;
    private AudioSource audioSource;
    private float tLerp;
    private float startVol;

    public void FadeMusic(AudioSource _audioSource, bool _islowersound, float _timeFade = 1, float _maxVol = 1){
        startVol = _audioSource.volume;
        audioSource = _audioSource;
        islowersound = _islowersound;
        timeFade = _timeFade;
        maxVol = _maxVol;
        tLerp = 0;
        fadeMusic = true;
    }

    void Update()
    {
        if (!fadeMusic) return;
        tLerp += Time.deltaTime / timeFade;
        if (tLerp >= 1){
            tLerp = 1;
            fadeMusic = false;
        }
        if (islowersound) audioSource.volume = Mathf.Lerp(startVol, 0, tLerp);
        else audioSource.volume = Mathf.Lerp(startVol, maxVol, tLerp);
    }
}
