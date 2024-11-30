using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gambling.Positive
{
    [Serializable, CreateAssetMenu(menuName = "Gambling/Positive/Increase Explosion Power")]
    public class IncreaseExplosionPowerEvent : GamblingEvent
    {
        [SerializeField] private float factor = 1.3f;
        
        public override string Name => "Увеличение силы взрыва";
        private readonly List<GameObject> _objects = new();
        public override void Execute(GamblingManager manager)
        {
            GameObject.FindGameObjectsWithTag("Explosive", _objects);
            foreach (var o in _objects)
                if (o.TryGetComponent<Explosive>(out var exp))
                    exp.force *= factor;
        }
    }
}