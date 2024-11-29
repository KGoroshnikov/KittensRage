using UnityEngine;

namespace Gambling.Positive
{
    [CreateAssetMenu(menuName = "Gambling/Positive/Meteor Rain")]
    public class MeteorRainEvent : GamblingEvent
    {
        [SerializeField] private int meteorCount = 10;
        public override string Name => "Метедоитный дождь";
        public override void Execute(GamblingManager manager)
        {
            for (var i = 0; i < meteorCount; i++)
                manager.meteorSources[Random.Range(0, manager.meteorSources.Length)].SpawnMeteor();
        }
    }
}