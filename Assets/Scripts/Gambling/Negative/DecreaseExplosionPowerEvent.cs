using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gambling.Negative
{
    [Serializable, CreateAssetMenu(menuName = "Gambling/Negative/Decrease Explosion Power")]
    public class DecreaseExplosionPowerEvent : GamblingEvent
    {
        [SerializeField] private float factor = 1.3f;
        
        public override string Name => "Уменьшение силы взрыва";
        private readonly List<GameObject> _objects = new();
        public override void Execute(GamblingManager manager)
        {
            GameObject.FindGameObjectsWithTag("Explosive", _objects);
            foreach (var o in _objects)
                if (o.TryGetComponent<Explosive>(out var exp))
                    exp.force /= factor;
        }
        public override void Execute(WheelOfFortune manager)
        {
            GameObject.FindGameObjectsWithTag("Explosive", _objects);
            foreach (var o in _objects)
                if (o.TryGetComponent<Explosive>(out var exp))
                    exp.force /= factor;
        }
    }
}