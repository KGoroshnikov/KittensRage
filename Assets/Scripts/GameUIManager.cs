using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Animator victoryAnim;
    [SerializeField] private Animator defeatAnim;
    [SerializeField] private Animator exitWarningAnim;
    
    [SerializeField] private Image[] winStars;
    [SerializeField] private Image[] defeatStars;
    [SerializeField] private Sprite[] starsSprite;

    [SerializeField] private Animator animatorFade;

    [SerializeField] private string nextLvlName;

    public void Win(int stars){
        cameraController.ActiveCameraContol(false);
        victoryAnim.gameObject.SetActive(true);
        victoryAnim.Play("VictoryAppear", -1, 0);

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

    public void ExitToMenu(){
        exitWarningAnim.gameObject.SetActive(true);
        exitWarningAnim.Play("ExitAppear", -1, 0);
    }

    public void CloseExitMenu(){
        exitWarningAnim.gameObject.SetActive(false);
    }

    public void ConfirmExit(){
        animatorFade.enabled = true;
        animatorFade.SetTrigger("FadeIn");
        Invoke("LoadMenu", 2);
    }

    public void NextLvl(){
        animatorFade.enabled = true;
        animatorFade.SetTrigger("FadeIn");
        Invoke("LoadNextLvl", 2);
    }

    public void ReloadLvl(){
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
