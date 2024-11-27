using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private Animator animatorFade;

    public void PlayButton(){
        animatorFade.enabled = true;
        animatorFade.SetTrigger("FadeIn");
        Invoke("LoadFirstLvl", 2);
    }

    void LoadFirstLvl(){
        SceneManager.LoadScene("LEVEL1");
    }
}
