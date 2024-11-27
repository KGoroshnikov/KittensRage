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

    void Update()
    {
        fps = (int)(1.0/Time.deltaTime);
        if (!active) return;
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
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - increment, minZoom, maxZoom);
    }

    void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.red;
        guiStyle.fontSize = 36;
        GUI.Label(new Rect(200, 200, 300, 200), "FPS: " + fps.ToString(), guiStyle);
    }
}
