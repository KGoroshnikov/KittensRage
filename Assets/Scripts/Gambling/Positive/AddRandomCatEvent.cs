using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gambling.Positive
{
    [Serializable, CreateAssetMenu(menuName = "Gambling/Positive/Add Random Cat")]
    public class AddRandomCatEvent : GamblingEvent
    {
        [SerializeField] private List<ThrowableCat> variants = new();
        public override string Name => "Добавление случайного кота";
        public override void Execute(GamblingManager manager)
        {
            var queue = manager.slingshot.queue;
            queue.Add(variants[Random.Range(0, variants.Count)]);
        }
    }
}