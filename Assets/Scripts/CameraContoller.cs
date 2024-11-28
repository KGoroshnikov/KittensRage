using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.1f;
    [SerializeField] private float zoomSpeed = 0.5f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 50f;
    [SerializeField] private float smoothTime = 0.1f;

    private Vector2 lastTouchPosition;
    private Vector3 velocity = Vector3.zero;
    private Vector3 targetPosition;
    private Camera cam;

    [SerializeField] private Transform camPosShoot;
    [SerializeField] private float camSizeShoot;
    [SerializeField] private float timeShootPos;
    private float camLerpT;
    private Vector3 beforeShootPos;
    private Quaternion beforeShootRot;
    private float beforeShootSize;
    private float sizeCamRef;
    private Quaternion rotRef;
    private enum shootingState{
        none, shooting, resetting
    }
    private shootingState m_shootingState;

    private int fps;

    private bool active;

    void Start()
    {
        cam = Camera.main;
        targetPosition = cam.transform.position;
        Application.targetFrameRate = 60;
    }

    public void ActiveCameraContol(bool _active){
        if (_active) targetPosition = cam.transform.position;
        active = _active;
    }
    
    public void StartShoot(){
        beforeShootPos = cam.transform.position;
        beforeShootRot = cam.transform.rotation;
        beforeShootSize = cam.orthographicSize;
        camLerpT = 0;

        m_shootingState = shootingState.shooting;
    }
    public void StopShoot(){
        camLerpT = 0;
        targetPosition = beforeShootPos;
        m_shootingState = shootingState.resetting;
    }

    void Update()
    {
        fps = (int)(1.0/Time.deltaTime);
        if (!active) return;
        if (m_shootingState == shootingState.shooting){
            camLerpT += Time.deltaTime / timeShootPos;
            if (camLerpT >= 1) camLerpT = 1;

            cam.transform.position = Vector3.Lerp(beforeShootPos, camPosShoot.position, Functions.SmoothLerp(camLerpT));
            cam.transform.rotation = Quaternion.Lerp(beforeShootRot, camPosShoot.rotation, Functions.SmoothLerp(camLerpT));
            cam.orthographicSize = Mathf.Lerp(beforeShootSize, camSizeShoot, Functions.SmoothLerp(camLerpT));
            return;
        }else if (m_shootingState == shootingState.resetting){
            camLerpT += Time.deltaTime / timeShootPos;
            if (camLerpT >= 1) camLerpT = 1;

            cam.transform.position = Vector3.Lerp(camPosShoot.position, beforeShootPos, Functions.SmoothLerp(camLerpT));
            cam.transform.rotation = Quaternion.Lerp(camPosShoot.rotation, beforeShootRot, Functions.SmoothLerp(camLerpT));
            cam.orthographicSize = Mathf.Lerp(camSizeShoot, beforeShootSize, Functions.SmoothLerp(camLerpT));
            if (camLerpT >= 1){
                cam.transform.rotation = beforeShootRot;
                cam.transform.position = beforeShootPos;
                cam.orthographicSize = beforeShootSize;
                m_shootingState = shootingState.none;
                targetPosition = cam.transform.position;
            }
            return;
        }
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                velocity = Vector3.zero;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = (Vector2)touch.position - lastTouchPosition;
                lastTouchPosition = touch.position;
                Vector3 forward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
                Vector3 right = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;
                Vector3 direction = right * -delta.x * moveSpeed + forward * -delta.y * moveSpeed * 1.5f;
                targetPosition += direction;
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Zoom(difference * zoomSpeed);
            targetPosition = cam.transform.position;
        }
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetPosition, ref velocity, smoothTime);
    }

    void Zoom(float increment)
    {
        //cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - increment, minZoom, maxZoom);
        beforeShootSize = Mathf.Clamp(cam.orthographicSize - increment, minZoom, maxZoom);
        cam.orthographicSize = beforeShootSize;
    }

    void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.red;
        guiStyle.fontSize = 36;
        GUI.Label(new Rect(200, 200, 300, 200), "FPS: " + fps.ToString(), guiStyle);
    }
}
