using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private Animator animatorFade;
    [SerializeField] private Animator animatorSigmaCat;
    [SerializeField] private Animator exitWarningAnim;
    private bool ableToPressBtns;

    void Start(){
        animatorSigmaCat.SetTrigger("Idle");
        animatorFade.enabled = true;
        animatorFade.SetTrigger("InstantFade");
        Invoke("FadeOut", 0.5f);
        Invoke("SetButtons", 1);
    }
    void FadeOut(){
        animatorFade.SetTrigger("FadeOut");
    }

    void SetButtons(){
        ableToPressBtns = true;
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
