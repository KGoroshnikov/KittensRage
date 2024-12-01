using UnityEngine;

namespace Gambling
{
    public abstract class GamblingEvent : ScriptableObject
    {
        public abstract string Name { get; }
        
        public abstract void Execute(GamblingManager manager);
        public abstract void Execute(WheelOfFortune manager);
    }
}