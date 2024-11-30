using UnityEngine;
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
            soundnosound[0].SetActive(false);
            soundnosound[1].SetActive(true);
        }
        if (!musicIsON) imgMusic.SetActive(true);
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

    public void TurnBloom(){
        bloomIsON = !bloomIsON;

        PlayerPrefs.SetInt("BloomSettings", bloomIsON ? 1 : 0);

        if (!bloomIsON) imgBloom.SetActive(true);
        else imgBloom.SetActive(false);

        if (!bloomIsON) QualitySettings.SetQualityLevel(1);
        else QualitySettings.SetQualityLevel(0);
    }

    public void WatchTrailerAgain(){
        PlayerPrefs.SetInt("WatchedTrailer", 0);
        animatorFade.SetTrigger("FadeIn");
        Invoke("watchTrailer", 2);
    }

    void watchTrailer(){
        SceneManager.LoadScene("ANIME");
    }

    public void MainSoundBtn(){
        soundIsON = !soundIsON;

        PlayerPrefs.SetInt("SoundSettings", soundIsON ? 1 : 0);

        if (!soundIsON){
            soundnosound[0].SetActive(false);
            soundnosound[1].SetActive(true);
        }
        else{
            soundnosound[0].SetActive(true);
            soundnosound[1].SetActive(false);
        }
    }
    public void MusicBtn(){
        musicIsON = !musicIsON;

        PlayerPrefs.SetInt("MusicSettings", musicIsON ? 1 : 0);

        if (!musicIsON) imgMusic.SetActive(true);
        else imgMusic.SetActive(false);
    }

    public void PlayButton(){
        if (!ableToPressBtns) return;
        animatorFade.enabled = true;
        animatorFade.SetTrigger("FadeIn");
        Invoke("LoadFirstLvl", 2);
    }

    void LoadFirstLvl(){
        SceneManager.LoadScene("LEVEL1");
    }

    public void ExitApp(){
        if (!ableToPressBtns) return;
        exitWarningAnim.gameObject.SetActive(true);
        exitWarningAnim.Play("ExitAppear", -1, 0);
    }

    public void CloseExitMenu(){
        exitWarningAnim.gameObject.SetActive(false);
    }

    public void ConfirmExit(){
        Application.Quit();
    }
}
