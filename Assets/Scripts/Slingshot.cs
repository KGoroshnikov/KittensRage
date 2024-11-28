using System.Collections.Generic;
using Projectiles;
using Unity.Collections;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [Header("Settings")]
    //public GameObject projectilePrefab;
    public Transform slingOrigin;
    public float maxStretch = 3f;
    public float launchForceMultiplier = 10f;
    public float maxAngle = 60f;
    public float minAngle = 0f;

    [Header("Trajectory")]
    public LineRenderer trajectoryLine;
    public int trajectoryPointsCount = 30;

    [Header("Stupid")]
    public bool isNonStandardGravityEnabled = false;
    public float gravityStrength = 9.81f;
    public float zForce;
    
    [Header("Other Settings")]
    private Camera cam;
    [SerializeField] private CameraController cameraController;

    [SerializeField] private float minThresoldLaunch;
    

    private GameObject currentProjectile;
    private ThrowableCat currentCat;
    private bool isDragging = false;
    private Vector3 startPoint;

    private Vector3 direction;

    public List<ThrowableCat> queue;
    public float speedJumpIn;
    public float heightJump;
    private float tjump;
    private bool animateJump;
    private Vector3 startPosCat;

    private bool activedCam;
    private bool active;

    private bool loaded;

    void Start(){
        cam = Camera.main;
    }

    void Update()
    {
        HandleInput();

        if (animateJump){
            tjump += Time.deltaTime / speedJumpIn;
            if (tjump >= 1){
                tjump = 1;
                animateJump = false;
            }
            Vector3 lerped = Vector3.Lerp(startPosCat, slingOrigin.position, Functions.SmoothLerp(tjump));
            lerped.y = lerped.y + Mathf.Sin(tjump*3.14f)*heightJump;
            currentProjectile.transform.position = lerped;
        }
    }

    public void GetCats(List<ThrowableCat> throwableCats){
        queue.AddRange(throwableCats);
    }

    public void ActivateMe(bool _active){
        active = _active;
    }

    /*private void FixedUpdate()
    {
        if (currentProjectile != null)
        {
            if (isNonStandardGravityEnabled && !rb.isKinematic)
            {
                //rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

                // Применение нестандартной гравитации по оси Z
                rb.AddForce(Vector3.back * gravityStrength, ForceMode.Acceleration);  // Гравитация по оси Z
            }
        }
    }*/

    private void HandleInput()
    {
        if (!active) return;
        if (queue.Count <= 0 && !loaded) return;

        if (Input.touchCount < 1) return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Sling")){
                cameraController.StartShoot();
                activedCam = true;
            }
        }

        if (!activedCam) return;

        if (touch.phase == TouchPhase.Began)
        {
            StartDragging();
        }
        else if (touch.phase == TouchPhase.Moved && isDragging)
        {
            Dragging();
        }
        else if (touch.phase == TouchPhase.Ended && isDragging)
        {
            ReleaseProjectile();
        }
    }

    private void StartDragging()
    {
        //currentProjectile = Instantiate(projectilePrefab, slingOrigin.position, Quaternion.identity);
        direction = Vector3.zero;
        if (!loaded){
            currentProjectile = queue[queue.Count - 1].gameObject;
            currentCat = queue[queue.Count - 1];
            queue.RemoveAt(queue.Count - 1);

            startPosCat = currentProjectile.transform.position;
            tjump = 0;
            animateJump = true;

            //rb = currentProjectile.GetComponent<Rigidbody>();
            //rb.isKinematic = true;
            currentCat.MakeMeKinematic(true);

            loaded = true;
        }

        startPoint = GetMouseWorldPosition();
        if (!isNonStandardGravityEnabled)
            startPoint.z = slingOrigin.position.z;
        isDragging = true;
    }

    private void Dragging()
    {
        Vector3 currentPoint = GetMouseWorldPosition();
        if (!isNonStandardGravityEnabled)
            currentPoint.z = slingOrigin.position.z;

        direction = startPoint - currentPoint;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        angle = Mathf.Clamp(angle, minAngle, maxAngle);

        float clampedMagnitude = Mathf.Min(direction.magnitude, maxStretch);

        direction = new Vector3(
                clampedMagnitude * Mathf.Cos(angle * Mathf.Deg2Rad),
                clampedMagnitude * Mathf.Sin(angle * Mathf.Deg2Rad),
                zForce);

        //currentProjectile.transform.position = slingOrigin.position - direction;
        DrawTrajectory(direction);
    }

    private void ReleaseProjectile()
    {
        trajectoryLine.positionCount = 0;
        isDragging = false;

        cameraController.StopShoot();
        activedCam = false;

        if (direction.magnitude <= minThresoldLaunch) return;

        loaded = false;
        //rb.isKinematic = false;
        currentCat.MakeMeKinematic(false);
        //if (isNonStandardGravityEnabled) rb.useGravity = false;
        //rb.linearVelocity = Vector3.zero;
        currentCat.SetVelocity(Vector3.zero);
        currentCat.Launch(direction * launchForceMultiplier);
        //rb.AddForce(direction * launchForceMultiplier, ForceMode.Impulse);

    }

    private void DrawTrajectory(Vector3 launchDirection)
    {
        Vector3 startPosition = currentProjectile.transform.position;
        Vector3 velocity = launchDirection * launchForceMultiplier / currentCat.GetMass();

        trajectoryLine.positionCount = trajectoryPointsCount;
        for (int i = 0; i < trajectoryPointsCount; i++)
        {
            float time = i * 0.1f;
            Vector3 displacement = Vector3.zero;
            if (!isNonStandardGravityEnabled) 
                displacement.z = 0;

            if (isNonStandardGravityEnabled)
            {
                displacement = velocity * time + 0.5f * Vector3.back * gravityStrength * time * time;
            }
            else{
                displacement = velocity * time + 0.5f * Physics.gravity * time * time;
            }

            Vector3 drawPoint = startPosition + displacement;
            if (!isNonStandardGravityEnabled) drawPoint.z = slingOrigin.position.z;
            trajectoryLine.SetPosition(i, drawPoint);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(slingOrigin.position).z;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        if (!isNonStandardGravityEnabled) worldPosition.z = slingOrigin.position.z;
        return worldPosition;
    }
}
