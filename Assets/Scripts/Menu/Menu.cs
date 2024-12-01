using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Animator animatorFade;
    [SerializeField] private Animator animatorSigmaCat;
    [SerializeField] private Animator exitWarningAnim;
    private bool ableToPressBtns;


    [Header("Settings")]
    private bool musicIsON;
    private bool soundIsON;
    private bool bloomIsON;
    [SerializeField] private GameObject[] soundnosound;
    [SerializeField] private GameObject imgMusic;
    [SerializeField] private GameObject imgBloom;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] uiSounds;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioMixer audioMixer;
    

    void Start(){
        animatorSigmaCat.SetTrigger("Idle");
        animatorFade.enabled = true;
        animatorFade.SetTrigger("InstantFade");
        Invoke("FadeOut", 0.5f);
        Invoke("SetButtons", 1);

        musicIsON = PlayerPrefs.GetInt("MusicSettings", 1) == 1;
        soundIsON = PlayerPrefs.GetInt("SoundSettings", 1) == 1;
        bloomIsON = PlayerPrefs.GetInt("BloomSettings", 1) == 1;

        if (!soundIsON){
            audioMixer.SetFloat("VolumeMain", Mathf.Log10(0.001f)*20);
            soundnosound[0].SetActive(false);
            soundnosound[1].SetActive(true);
        }
        if (!musicIsON){
            audioMixer.SetFloat("VolumeMusic", Mathf.Log10(0.001f)*20);
            imgMusic.SetActive(true);
        }
        if (!bloomIsON){
            QualitySettings.SetQualityLevel(1);
            imgBloom.SetActive(true);
        }
        else QualitySettings.SetQualityLevel(0);
    }
    void FadeOut(){
        animatorFade.SetTrigger("FadeOut");
    }

    void SetButtons(){
        ableToPressBtns = true;
    }

    public void PlayBtnSound(){
        audioSource.PlayOneShot(uiSounds[0]);
    }
    public void PlayPlaySound(){
        audioSource.PlayOneShot(uiSounds[1]);
    }
    public void PlayCancelSound(){
        audioSource.PlayOneShot(uiSounds[2]);
    }

    public void TurnBloom(){
        PlayBtnSound();
        bloomIsON = !bloomIsON;

        PlayerPrefs.SetInt("BloomSettings", bloomIsON ? 1 : 0);

        if (!bloomIsON) imgBloom.SetActive(true);
        else imgBloom.SetActive(false);

        if (!bloomIsON) QualitySettings.SetQualityLevel(1);
        else QualitySettings.SetQualityLevel(0);
    }

    public void WatchTrailerAgain(){
        PlayBtnSound();
        musicManager.FadeMusic(musicSource, true, 1.5f);
        PlayerPrefs.SetInt("WatchedTrailer", 0);
        animatorFade.SetTrigger("FadeIn");
        Invoke("watchTrailer", 2);
    }

    void watchTrailer(){
        SceneManager.LoadScene("ANIME");
    }

    public void MainSoundBtn(){
        PlayBtnSound();
        soundIsON = !soundIsON;

        PlayerPrefs.SetInt("SoundSettings", soundIsON ? 1 : 0);

        if (!soundIsON){
            audioMixer.SetFloat("VolumeMain", Mathf.Log10(0.001f)*20);
            soundnosound[0].SetActive(false);
            soundnosound[1].SetActive(true);
        }
        else{
            audioMixer.SetFloat("VolumeMain", Mathf.Log10(1f)*20);
            soundnosound[0].SetActive(true);
            soundnosound[1].SetActive(false);
        }
    }
    public void MusicBtn(){
        PlayBtnSound();
        musicIsON = !musicIsON;

        PlayerPrefs.SetInt("MusicSettings", musicIsON ? 1 : 0);

        if (!musicIsON){
            audioMixer.SetFloat("VolumeMusic", Mathf.Log10(0.001f)*20);
            imgMusic.SetActive(true);
        }
        else{
            audioMixer.SetFloat("VolumeMusic", Mathf.Log10(1f)*20);
            imgMusic.SetActive(false);
        }
    }

    public void PlayButton(){
        PlayPlaySound();
        if (!ableToPressBtns) return;
        musicManager.FadeMusic(musicSource, true, 1.5f);
        animatorFade.enabled = true;
        animatorFade.SetTrigger("FadeIn");
        Invoke("LoadFirstLvl", 2);
    }

    void LoadFirstLvl(){
        SceneManager.LoadScene("LEVEL1");
    }

    public void ExitApp(){
        PlayCancelSound();
        if (!ableToPressBtns) return;
        exitWarningAnim.gameObject.SetActive(true);
        exitWarningAnim.Play("ExitAppear", -1, 0);
    }

    public void CloseExitMenu(){
        PlayBtnSound();
        exitWarningAnim.gameObject.SetActive(false);
    }

    public void ConfirmExit(){
        Application.Quit();
    }
}
