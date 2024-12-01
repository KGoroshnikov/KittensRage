using System;
using UnityEngine;

namespace Gambling.Positive
{
    [Serializable, CreateAssetMenu(menuName = "Gambling/Positive/Increase Giga Cat Health")]
    public class IncreaseGigaCatHealthEvent : GamblingEvent
    {
        [SerializeField] private float factor = 1.3f;
        public override string Name => "Укрепление большого кота";
        public override void Execute(GamblingManager manager)
        {
            var delta = manager.gigaCatHealth.HealthMax * (factor - 1);
            manager.gigaCatHealth.ChangeMaxHealth(delta);
        }
        public override void Execute(WheelOfFortune manager)
        {
            var delta = manager.gigaCatHealth.Health * factor;
            manager.gigaCatHealth.ChangeMaxHealth(delta);
        }
    }
}