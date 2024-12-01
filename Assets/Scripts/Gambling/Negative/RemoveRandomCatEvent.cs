using UnityEngine;

namespace Gambling.Negative
{
    [CreateAssetMenu(menuName = "Gambling/Negative/Remove Random Cat")]
    public class RemoveRandomCatEvent : GamblingEvent
    {
        public override string Name => "Удаление случайного кота";
        public override void Execute(GamblingManager manager)
        {
            var queue = manager.slingshot.queue;
            queue.RemoveAt(Random.Range(0, queue.Count));
        }
        public override void Execute(WheelOfFortune manager)
        {
            /*var queue = manager.slingshot.queue;
            queue.RemoveAt(Random.Range(0, queue.Count));*/
            manager.slingshot.RemoveCat();
        }
    }
}