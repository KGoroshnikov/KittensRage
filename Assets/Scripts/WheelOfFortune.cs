using Gambling;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.VFX;

public class WheelOfFortune : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform wheel;       // Ссылка на колесо
    public float minSpeed = 200f;     // Минимальная скорость вращения
    public float maxSpeed = 1000f;    // Максимальная скорость вращения
    public float decelerationTime = 3f; // Время замедления
    
    private bool isSpinning = false;  // Флаг вращения колеса
    private float currentSpeed = 0f;  // Текущая скорость колеса
    private Vector2 previousDragPosition; // Позиция пальца на предыдущем кадре

    [SerializeField] private GamblingEvent[] gamblingEvents;
    
    [Header("GigaCat")]
    public HealthManager gigaCatHealth;

    [Header("Meteors")]
    public Transform[] meteorSources;
    public GameObject fireballPref;


    [Header("AddRat")]
    public GameObject ratPref;
    public Transform[] spawnPoints;
    public VisualEffect puffEffect;
    public Transform mainParent;

    [Header("AddRemoveCat")]
    public Slingshot slingshot;

    private int eventId = 0;

    [Header("Other")]
    [SerializeField] private Animator animator;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform[] icons;

    private bool canUseMe;

    public void ActivateMe(){
        gigaCatHealth = gameManager.getGigaHealth();
        mainParent = gameManager.getMainParent();

        canUseMe = true;
        animator.SetTrigger("Enable");
        cameraController.ActiveCameraContol(false);
    }

    void CloseWheel(){
        gameManager.CloseWheel();
    }

    public void DisableMe(){
        animator.SetTrigger("Disable");
        cameraController.ActiveCameraContol(true);
        gameManager.GetBackFromWheel();
        Invoke("CloseWheel", 1.5f);
    }

    void UseEvent(){
        gamblingEvents[eventId].Execute(this);
        DisableMe();
    }

    void Update()
    {
        /*float finalAngle = wheel.eulerAngles.z % 360f;
        int slotCount = 8;
        float slotAngle = 360f / slotCount;

        // Учитываем слот снизу
        float adjustedAngle = (finalAngle + 180f) % 360f;
        int resultSlot = Mathf.FloorToInt(adjustedAngle / slotAngle);

        Debug.Log($"Выпал слот (снизу): {resultSlot}");*/

        for(int i = 0; i < icons.Length; i++){
            icons[i].transform.localEulerAngles = -wheel.localEulerAngles;
        }

        if (isSpinning)
        {
            // Вращаем колесо
            wheel.Rotate(0, 0, currentSpeed * Time.deltaTime);

            // Уменьшаем скорость экспоненциально
            currentSpeed -= currentSpeed * Time.deltaTime / decelerationTime;

            // Проверяем, если скорость слишком мала
            if (Mathf.Abs(currentSpeed) < 10f)
            {
                currentSpeed = 0f;
                isSpinning = false;
                DetermineResult();
            }
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canUseMe) return;
        if (!isSpinning)
        {
            previousDragPosition = eventData.position;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!canUseMe) return;
        if (!isSpinning)
        {
            Vector2 currentDragPosition = eventData.position;

            // Получаем экранные координаты центра колеса
            Vector2 wheelCenter = RectTransformUtility.WorldToScreenPoint(Camera.main, wheel.position);

            // Вычисляем угол между центром колеса и текущей/предыдущей позицией пальца
            float previousAngle = Mathf.Atan2(previousDragPosition.y - wheelCenter.y, previousDragPosition.x - wheelCenter.x) * Mathf.Rad2Deg;
            float currentAngle = Mathf.Atan2(currentDragPosition.y - wheelCenter.y, currentDragPosition.x - wheelCenter.x) * Mathf.Rad2Deg;

            // Разница углов (с учётом перехода через 360/0)
            float angleDelta = Mathf.DeltaAngle(previousAngle, currentAngle);

            // Поворачиваем колесо в соответствии с изменением угла
            wheel.Rotate(0, 0, angleDelta);

            // Обновляем предыдущую позицию пальца
            previousDragPosition = currentDragPosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canUseMe) return;
        if (!isSpinning)
        {
            // Рассчитываем скорость вращения на основе последнего изменения угла
            currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed) * Mathf.Sign(currentSpeed);
            isSpinning = true;
            canUseMe = false;
        }
    }

    private void DetermineResult()
    {
        float finalAngle = wheel.eulerAngles.z % 360f;
        int slotCount = 8;
        float slotAngle = 360f / slotCount;

        // Учитываем слот снизу
        float adjustedAngle = (finalAngle + 180f) % 360f;
        int resultSlot = Mathf.FloorToInt(adjustedAngle / slotAngle);

        Debug.Log($"Выпал слот (снизу): {resultSlot}");
        eventId = resultSlot;
        Invoke("UseEvent", 2);
    }
}
