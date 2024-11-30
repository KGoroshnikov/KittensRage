using System;
using UnityEngine;

namespace Gambling.Negative
{
    [Serializable, CreateAssetMenu(menuName = "Gambling/Positive/Increase Giga Cat Health")]
    public class DecreaseGigaCatHealthEvent : GamblingEvent
    {
        [SerializeField] private float factor = 1.3f;
        public override string Name => "Ослабление большого кота";
        public override void Execute(GamblingManager manager)
        {
            var delta = manager.gigaCatHealth.HealthMax * (factor - 1);
            manager.gigaCatHealth.ChangeMaxHealth(-delta);
        }
    }
}