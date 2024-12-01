﻿using System;
using UnityEngine;

namespace Gambling.Negative
{
    [Serializable, CreateAssetMenu(menuName = "Gambling/Negative/Decrease Giga Cat Health")]
    public class DecreaseGigaCatHealthEvent : GamblingEvent
    {
        [SerializeField] private float factor = .3f;
        public override string Name => "Ослабление большого кота";
        public override void Execute(GamblingManager manager)
        {
            var delta = manager.gigaCatHealth.HealthMax * (factor - 1);
            manager.gigaCatHealth.ChangeMaxHealth(-delta);
        }
        public override void Execute(WheelOfFortune manager)
        {
            var delta = manager.gigaCatHealth.Health * factor;
            manager.gigaCatHealth.ChangeMaxHealth(delta);
        }
    }
}