using System;
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
        
        
        public GamblingEvent Roll() => gamblingEvents[Random.Range(0, gamblingEvents.Length)];

    }
}
