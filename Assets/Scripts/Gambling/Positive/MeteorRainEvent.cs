using System.Collections.Generic;
using UnityEngine;

namespace Gambling.Positive
{
    [CreateAssetMenu(menuName = "Gambling/Positive/Meteor Rain")]
    public class MeteorRainEvent : GamblingEvent
    {
        public int meteorCount = 10;
        public float meteorSpeed;
        public override string Name => "Метедоитный дождь";
        private List<int> usedCoords = new List<int>();
        public override void Execute(GamblingManager manager)
        {
            /*for (var i = 0; i < meteorCount; i++)
                manager.meteorSources[Random.Range(0, manager.meteorSources.Length)].SpawnMeteor();*/
        }
        public override void Execute(WheelOfFortune manager)
        {
            usedCoords.Clear();
            for(int i = 0; i < meteorCount; i++){
                int idx = Random.Range(0, manager.meteorSources.Length);
                for(int j = 0; j < 10; j++){
                    idx = Random.Range(0, manager.meteorSources.Length);
                    if (!usedCoords.Contains(idx)){
                        usedCoords.Add(idx);
                        break;
                    }
                }
                var dir = manager.meteorSources[idx].transform.forward;
                var projectile = Instantiate(manager.fireballPref, manager.meteorSources[idx].position, Quaternion.identity, null);
                projectile.transform.forward = dir;
                if (projectile.TryGetComponent(out Rigidbody rb))
                    rb.AddForce(dir * meteorSpeed, ForceMode.Impulse);
            }
        }
    }
}