using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Animator animatorFade;
    [SerializeField] private Animator UIBlackLines;
    [SerializeField] private Animator camAnimator;
    [SerializeField] private Camera cam;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private StarsManager starsManager;
    [SerializeField] private Transform startCamPos;
    [SerializeField] private Vector3 startEndCamSize;
    [SerializeField] private Transform choosingCamPos;
    [SerializeField] private Transform startGameCamPos;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private float camLerpSpeed;
    private float tCam;
    private enum gameState{
        lvlWatching, goingToChoose, choosing, preparingGame, playing
    }
    private gameState m_gameState = gameState.lvlWatching;
    [SerializeField] private int countDown;

    [SerializeField] private int maxCatTypes;
    private enum catTypes{
        err, cat, magician, doubles, stupid
    }
    private List<catTypes> chosenCats = new List<catTypes>();

    [SerializeField] private GameUIManager gameUIManager;

    void Start(){
        animatorFade.enabled = true;
        animatorFade.SetTrigger("InstantFade");
        
        cam.transform.position = startCamPos.position;
        cam.transform.rotation = startCamPos.rotation;
        cam.orthographicSize = startEndCamSize.x;
        Invoke("StartGame", 1);
    }
    void StartGame(){
        animatorFade.SetTrigger("FadeOut");
        InvokeRepeating("CountDown", 1, 1);
    }

    void CountDown(){
        countDown--;
        if (countDown < 0){
            countDown = 0;
            countdownText.gameObject.SetActive(false);
            CancelInvoke("CountDown");
            m_gameState = gameState.goingToChoose;
            return;
        }
        countdownText.text = countDown.ToString();
    }

    void Win(){
        gameUIManager.Win(starsManager.getStars());
    }
    void Loose(){
        gameUIManager.Loose(starsManager.getStars());
    }

    void Update(){
        if (m_gameState == gameState.lvlWatching) return;
        
        if (m_gameState == gameState.goingToChoose){
            tCam += Time.deltaTime / camLerpSpeed;
            if (tCam >= 1){
                tCam = 1;
                m_gameState= gameState.choosing;
            }
            cam.transform.position = Vector3.Lerp(startCamPos.position, choosingCamPos.position, Functions.SmoothLerp(tCam));
            cam.transform.rotation = Quaternion.Lerp(startCamPos.rotation, choosingCamPos.rotation, Functions.SmoothLerp(tCam));
            cam.orthographicSize = Mathf.Lerp(startEndCamSize.x, startEndCamSize.y, Functions.SmoothLerp(tCam));
        }

        if (m_gameState == gameState.choosing && Input.touchCount == 1){
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit hit;
                Ray ray = cam.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out hit)) {
                    Transform objectHit = hit.transform;
                    catTypes newCat = catTypes.err;

                    if (hit.collider.CompareTag("Cat")) newCat = catTypes.cat;
                    else if (hit.collider.CompareTag("Magician")) newCat = catTypes.magician;
                    else if (hit.collider.CompareTag("Doubles")) newCat = catTypes.doubles;
                    else if (hit.collider.CompareTag("Stupid")) newCat = catTypes.stupid;

                    if (newCat != catTypes.err){
                        if (chosenCats.Contains(newCat)){
                            chosenCats.Remove(newCat);
                            hit.collider.GetComponent<Animator>().SetTrigger("Idle");
                        }
                        else {
                            chosenCats.Add(newCat);
                            hit.collider.GetComponent<Animator>().SetTrigger("Happy");
                        }
                        camAnimator.enabled = true;
                        camAnimator.SetTrigger("Choose");

                        if (chosenCats.Count >= maxCatTypes){
                            tCam = 0;
                            m_gameState = gameState.preparingGame;
                        }
                    }
                }
            }
        }

        if (m_gameState == gameState.preparingGame){
            tCam += Time.deltaTime;
            if (tCam >= 1){
                tCam = 1;
                m_gameState= gameState.playing;
                UIBlackLines.enabled = true;
                UIBlackLines.SetTrigger("Hide");
                cameraController.ActiveCameraContol(true);

                // delete in prod
                starsManager.enabled = true;
                Invoke("Win", 5);
            }
            cam.transform.position = Vector3.Lerp(choosingCamPos.position, startGameCamPos.position, Functions.SmoothLerp(tCam));
            cam.transform.rotation = Quaternion.Lerp(choosingCamPos.rotation, startGameCamPos.rotation, Functions.SmoothLerp(tCam));
            cam.orthographicSize = Mathf.Lerp(startEndCamSize.y, startEndCamSize.z, Functions.SmoothLerp(tCam));
        }
    }
}
