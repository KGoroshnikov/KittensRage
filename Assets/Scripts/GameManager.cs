using System.Collections.Generic;
using AI;
using Projectiles;
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

    [SerializeField] private int[] catAmount;
    [SerializeField] private int maxCatTypes;
    public enum catTypes{
        err, cat, magician, doubles, stupid
    }
    private List<catTypes> chosenCats = new List<catTypes>();

    [System.Serializable]
    public class catToChoose{
        public GameObject obj;
        public GameObject prefThrow;
        public catTypes catType;
    }
    [SerializeField] private catToChoose[] allCatsToChoose;
    [SerializeField] private Transform posSpawn;
    [SerializeField] private Vector3 offsetSpawn;
    private List<ThrowableCat> spawnedPrefs = new List<ThrowableCat>();
    
    [SerializeField] private Slingshot catSling;

    [SerializeField] private GameUIManager gameUIManager;

    [SerializeField] private Slingshot slingshot;

    [SerializeField] private GameObject lvlObjects;
    private int startAmountOfObjects;
    [SerializeField] private GameObject king;
    [SerializeField] private GameObject bigCat;
    [SerializeField] private GigaCatAI gigaCatAI;
    [SerializeField] private float[] percentsDestructions;

    [SerializeField] private ArcherAI[] allRats;

    private float startTime;
    private float endTime;

    void Start(){
        animatorFade.enabled = true;
        animatorFade.SetTrigger("InstantFade");
        
        cam.transform.position = startCamPos.position;
        cam.transform.rotation = startCamPos.rotation;
        cam.orthographicSize = startEndCamSize.x;
        Invoke("StartGame", 1);

        startAmountOfObjects = lvlObjects.transform.childCount;
        InvokeRepeating("CheckLevel", 0.5f, 0.5f);
    }
    void CheckLevel(){
        float destruction = 1.0f - (float)lvlObjects.transform.childCount / (float)startAmountOfObjects;
        if (starsManager.getStars() < 1 && destruction > percentsDestructions[0]) starsManager.AddToQueue();
        if (starsManager.getStars() < 2 && destruction > percentsDestructions[1]) starsManager.AddToQueue();
        if (starsManager.getStars() < 3 && destruction > percentsDestructions[2]) starsManager.AddToQueue();

        if (king == null){
            CancelInvoke("CheckLevel");
            StopTimer();
            Invoke("Win", 3);
            gigaCatAI.StopMe();
            //Win();
            return;
        }
        if (bigCat == null){
            CancelInvoke("CheckLevel");
            StopTimer();
            Invoke("Loose", 3);
            //Loose();
        }
    }

    void StartGame(){
        animatorFade.SetTrigger("FadeOut");
        InvokeRepeating("CountDown", 1, 1);
    }

    public void StartTimer()
    {
        startTime = Time.time;
    }
    public void StopTimer()
    {
        endTime = Time.time;
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
        gameUIManager.DisplayStats(endTime- startTime, startAmountOfObjects - lvlObjects.transform.childCount);
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

                        if (chosenCats.Count >= maxCatTypes){ // подготовка к игре
                            tCam = 0;
                            camAnimator.enabled = false;
                            m_gameState = gameState.preparingGame;

                            for(int i = 0; i < allCatsToChoose.Length; i++) Destroy(allCatsToChoose[i].obj); // удаляем чтобы не мешали

                            for(int i = 0; i < chosenCats.Count; i++){ // идем по порядку выбора
                                for(int j = 0; j < allCatsToChoose.Length; j++){ // находим нужного кота и его префаб
                                    if (allCatsToChoose[j].catType == chosenCats[i]){

                                        for(int k = 0; k < catAmount[i]; k++){ // смотрим количество котов по порядку выбора
                                            GameObject prefCat = Instantiate(allCatsToChoose[j].prefThrow, 
                                                posSpawn.position + offsetSpawn * spawnedPrefs.Count, 
                                                allCatsToChoose[j].obj.transform.rotation);
                                            prefCat.GetComponent<Animator>().Play("Idle", -1, Random.value);
                                            spawnedPrefs.Add(prefCat.GetComponent<ThrowableCat>());
                                        }

                                        break;
                                    }
                                }
                            }

                            /*for(int i = 0; i < allCatsToChoose.Length; i++){
                                if (chosenCats.Contains(allCatsToChoose[i].catType)){
                                    GameObject prefCat = Instantiate(allCatsToChoose[i].prefThrow, 
                                            posSpawn.position + offsetSpawn * spawnedPrefs.Count, 
                                            allCatsToChoose[i].obj.transform.rotation);
                                    spawnedPrefs.Add(prefCat.GetComponent<ThrowableCat>());
                                }
                                Destroy(allCatsToChoose[i].obj);
                            }*/
                            catSling.GetCats(spawnedPrefs);
                        }
                    }
                }
            }
        }

        if (m_gameState == gameState.preparingGame){
            tCam += Time.deltaTime;
            if (tCam >= 1){ // старт игры
                tCam = 1;
                m_gameState= gameState.playing;
                UIBlackLines.enabled = true;
                UIBlackLines.SetTrigger("Hide");
                cameraController.ActiveCameraContol(true);
                gigaCatAI.StartGame();
                slingshot.ActivateMe(true);
                StartTimer();

                for(int i = 0; i < allRats.Length; i++){
                    allRats[i].AllowAttack();
                }

                //Invoke("Win", 5);
            }
            cam.transform.position = Vector3.Lerp(choosingCamPos.position, startGameCamPos.position, Functions.SmoothLerp(tCam));
            cam.transform.rotation = Quaternion.Lerp(choosingCamPos.rotation, startGameCamPos.rotation, Functions.SmoothLerp(tCam));
            cam.orthographicSize = Mathf.Lerp(startEndCamSize.y, startEndCamSize.z, Functions.SmoothLerp(tCam));
        }
    }
}
