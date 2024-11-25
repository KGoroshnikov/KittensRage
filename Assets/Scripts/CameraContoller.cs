using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.1f;
    [SerializeField] private float zoomSpeed = 0.5f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 50f;

    private Vector2 touchStart;
    private Vector2 touchMid;
    private Camera cam;

    private int fps;

    void Start()
    {
        Application.targetFrameRate = 60;
        cam = Camera.main;
    }

    void Update()
    {
        fps = (int)(1.0/Time.deltaTime);
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStart = touch.position;
                touchMid = touchStart;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 direction = touchMid - touch.position;
                touchMid = touch.position;
                cam.transform.position += new Vector3(-direction.x, direction.y) * moveSpeed;
            }
        }

        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Zoom(difference * zoomSpeed);
        }
    }

    void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.red;
        guiStyle.fontSize = 30;
        GUI.Label(new Rect(150, 50, 300, 200), "FPS: " + fps.ToString(), guiStyle);
    }

    void Zoom(float increment)
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - increment, minZoom, maxZoom);
    }
}
