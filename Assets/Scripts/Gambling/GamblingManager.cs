using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gambling
{
    public class GamblingManager : MonoBehaviour
    {
        public Slingshot slingshot;
        public HealthManager gigaCatHealth;
        public MeteorSource[] meteorSources;
        public GameObject ratTowerPrefab;
        public Transform ratTowerParent;
        
        private Camera cam;
        private int _result;
        private bool _isRolling;
        private float _time;
        private float _startAngle;
        private float _endAngle;
        
        [Header("UI Settings")]
        [SerializeField] private RectTransform wheel;
        [SerializeField] private GameObject resultCard;
        [SerializeField] private TMP_Text cardText;
        [SerializeField] private float rollCount = 3;
        [SerializeField] private float rollTime = 3;
        
        
        // Хорошие:
        //  + Добавление случайного кота на уровень
        //  + Метеоритный удар по крысам (просто шары с неба падают)
        //  + Сила взрывов в два раза повышается 
        //  + Большой кот получает доп хп
        // Плохие:
        //  + Удаление случайного кота из колоды
        //  + Взрывы уменьшаются 
        //  + Добавление крысы на уровень на башне ( можно, чтобы справа от основного уровня появлялась башня с крысой, башня есть в префабах)
        //  - Управление инвертируется на выстрел
        [SerializeField] private GamblingEvent[] gamblingEvents;
        
        private void Start(){
            cam = Camera.main;
        }

        public void RollClicked()
        {
            _result = Roll();
            _isRolling = true;
            _time = 0;
            _startAngle = wheel.rotation.eulerAngles.z;
            _endAngle = _startAngle + 360 * (rollCount + (float) _result / gamblingEvents.Length);
        }
        public void RollExecute()
        {
            // Если без окна с названием события то закоментировать 1 строку ниже
            resultCard.SetActive(false);
            gamblingEvents[_result].Execute(this);
        }


        private void FixedUpdate()
        {
            if (!_isRolling) return;
            if (_time >= rollTime)
            {
                cardText.text = gamblingEvents[_result].Name;
                resultCard.SetActive(true);
                // Если без окна с названием события то закоментировать 2 строки выше и раскоментировать 1 ниже
                // RollExecute();
                _isRolling = false;
                return;
            }
            wheel.rotation = Quaternion.Euler(
                wheel.rotation.eulerAngles.x, 
                wheel.rotation.eulerAngles.y, 
                Mathf.Lerp(_startAngle, _endAngle, _time / rollTime));
            _time += Time.fixedDeltaTime;
        }


        public int Roll() => Random.Range(0, gamblingEvents.Length);

    }
}
