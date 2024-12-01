using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Animator victoryAnim;
    [SerializeField] private Animator defeatAnim;
    [SerializeField] private Animator exitWarningAnim;

    [SerializeField] private GameManager gameManager;
    
    [SerializeField] private Image[] winStars;
    [SerializeField] private Image[] defeatStars;
    [SerializeField] private Sprite[] starsSprite;

    [SerializeField] private Animator animatorFade;

    [SerializeField] private string nextLvlName;

    [SerializeField] private TMP_Text[] timeRequired;
    [SerializeField] private TMP_Text[] objectsDestroyed;
    [SerializeField] private TMP_Text lvlText;
    
    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip victorySound;

    [SerializeField] private AudioSource uisounds;
    [SerializeField] private AudioClip[] uiClips; 
    

    public void Win(int stars){
        cameraController.ActiveCameraContol(false);
        victoryAnim.gameObject.SetActive(true);
        victoryAnim.Play("VictoryAppear", -1, 0);
        source.Stop();
        source.PlayOneShot(victorySound);

        for(int i= 0; i < winStars.Length; i++) winStars[i].sprite = starsSprite[0];
        for(int i= 0; i < stars; i++) if(i < winStars.Length) winStars[i].sprite = starsSprite[1];
    }
    public void Loose(int stars){
        cameraController.ActiveCameraContol(false);
        defeatAnim.gameObject.SetActive(true);
        defeatAnim.Play("DefeatAppear", -1, 0);

        for(int i= 0; i < defeatStars.Length; i++) defeatStars[i].sprite = starsSprite[0];
        for(int i= 0; i < stars; i++) if(i < defeatStars.Length) defeatStars[i].sprite = starsSprite[1];
    }

    public void DisplayStats(float timeSec, int objDestr){
        int minutes = Mathf.FloorToInt(timeSec / 60);
        int seconds = Mathf.FloorToInt(timeSec % 60);
        for(int i = 0; i < timeRequired.Length; i++){
            timeRequired[i].text = "TIME REQUIRED: <color=#BA60FF>" + minutes + ":" + seconds + "</color>";
        }
        for(int i = 0; i < objectsDestroyed.Length; i++){
            objectsDestroyed[i].text = "OBJECTS DESTROYED: <color=#BA60FF>" + objDestr + "</color>";
        }
        string nameLvl = SceneManager.GetActiveScene().name.Substring(5);
        lvlText.text = "LEVEL <COLOR=#FFBA00>" + nameLvl + "</COLOR> COMPLETE!";
    }

    public void ExitToMenu(){
        uisounds.PlayOneShot(uiClips[2]);
        exitWarningAnim.gameObject.SetActive(true);
        exitWarningAnim.Play("ExitAppear", -1, 0);
    }

    public void CloseExitMenu(){
        uisounds.PlayOneShot(uiClips[0]);
        exitWarningAnim.gameObject.SetActive(false);
    }

    public void ConfirmExit(){
        uisounds.PlayOneShot(uiClips[2]);
        gameManager.TurnMusicOff();
        animatorFade.enabled = true;
        animatorFade.SetTrigger("FadeIn");
        Invoke("LoadMenu", 2);
    }

    public void NextLvl(){
        uisounds.PlayOneShot(uiClips[1]);
        gameManager.TurnMusicOff();
        animatorFade.enabled = true;
        animatorFade.SetTrigger("FadeIn");
        Invoke("LoadNextLvl", 2);
    }

    public void ReloadLvl(){
        uisounds.PlayOneShot(uiClips[1]);
        animatorFade.enabled = true;
        animatorFade.SetTrigger("FadeIn");
        Invoke("ReloadCurrentLvl", 2);
    }

    void ReloadCurrentLvl(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void LoadNextLvl(){
        SceneManager.LoadScene(nextLvlName);
    }

    void LoadMenu(){
        SceneManager.LoadScene("MENU");
    }
}
