using UnityEngine;
using UnityEngine.SceneManagement;

public class ImageManager : MonoBehaviour
{
    private Vector3 mainScale;

    [SerializeField] private GameObject[] objs;
    [SerializeField] private SpriteRenderer[] sprites;
    [SerializeField] private frame[] frameData;
    public int currentFrame;

    private float[] tlerp;
    private float[] tMat;

    [System.Serializable]
    public class frame{
        public Sprite nextSprite;
        public Vector2 maskOffset;
        public float timeMask;
        public Material material;
        public int activeImg;
        public float maxScaleAdd;
        public float timeImg;
        public float fadeTime;
        [HideInInspector]
        public Vector3 targetScale;
    }
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private AudioSource audioSource;
    private bool fadingMusic;


    void Start()
    {
        bool alreadyWatched = PlayerPrefs.GetInt("WatchedTrailer", 0) == 1 ? true : false;
        PlayerPrefs.SetInt("WatchedTrailer", 1);

        if (alreadyWatched){
            SKIP();
        }

        tlerp = new float[]{0, 0};
        tMat = new float[]{0, 0};
        for(int i = 0; i < sprites.Length; i++){
            AdjustSpriteSize(sprites[i]);
        }

        for(int i = 0; i < frameData.Length; i++){
            frameData[i].targetScale = Vector3.one + Vector3.one * frameData[i].maxScaleAdd;
        }

        int _idx = frameData[currentFrame].activeImg;
        sprites[1-_idx].material.SetTexture("_MainTex_1", frameData[currentFrame].nextSprite.texture);

        musicManager.FadeMusic(audioSource, false, 4f, 0.5f);
    }

    void SKIP(){
        SceneManager.LoadScene("MENU");
    }

    void Update(){
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began){
                Invoke("SKIP", 3);
            }
            else if (touch.phase == TouchPhase.Ended){
                CancelInvoke("SKIP");
            }
        }

        if (currentFrame >= frameData.Length) return;

        int _idx = frameData[currentFrame].activeImg;
        tlerp[_idx] += Time.deltaTime / frameData[currentFrame].timeImg;
        tMat[_idx] += Time.deltaTime / frameData[currentFrame].timeMask;

        objs[_idx].transform.localScale = Vector3.Lerp(Vector3.one, frameData[currentFrame].targetScale, tlerp[_idx]);
        
        float fadeThreshold = 1 - (frameData[currentFrame].fadeTime / frameData[currentFrame].timeImg);
        if (tlerp[_idx] >= fadeThreshold)
        {
            float fadeProgress = Mathf.InverseLerp(fadeThreshold, 1, tlerp[_idx]);
            SetAlpha(sprites[_idx], 1 - fadeProgress);
            if (currentFrame != frameData.Length - 1) SetAlpha(sprites[1-_idx], fadeProgress);
        }

        float maskValue = Mathf.Lerp(0.8f, 1.4f, tMat[_idx]);
        sprites[_idx].material.SetFloat("_MaskScale", maskValue);

        if (tlerp[_idx] >= 1){
            tlerp[_idx] = 0;
            tMat[_idx] = 0;
            
            objs[_idx].transform.localScale = Vector3.one;
            sprites[_idx].material.SetFloat("_MaskScale", 0.8f);
            //objs[_idx].SetActive(false);
            //objs[1 - _idx].SetActive(true);
            currentFrame++;

            if (currentFrame >= frameData.Length){
                SceneManager.LoadScene("MENU");
                return;
            }

            if (currentFrame >= frameData.Length - 1 && !fadingMusic){
                fadingMusic = true;
                Invoke("FadeMusic", frameData[currentFrame].timeImg - 2);
            }

            _idx = frameData[currentFrame].activeImg;
            if (frameData[currentFrame].nextSprite != null) sprites[1-_idx].material.SetTexture("_MainTex_1", frameData[currentFrame].nextSprite.texture);
        }
    }

    void FadeMusic(){
        musicManager.FadeMusic(audioSource, true, 1.5f);
    }

    void SetAlpha(SpriteRenderer spriteRenderer, float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

    void AdjustSpriteSize(SpriteRenderer spriteRenderer)
    {
        Camera cam = Camera.main;
        float screenHeight = cam.orthographicSize * 2.0f;
        float screenWidth = screenHeight * cam.aspect;

        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        mainScale = transform.localScale;
        mainScale.x = screenWidth / spriteSize.x;
        mainScale.y = screenHeight / spriteSize.y;
        spriteRenderer.transform.localScale = mainScale;
    }
}
