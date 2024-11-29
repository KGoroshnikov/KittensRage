using UnityEngine;

namespace Gambling.Negative
{
    [CreateAssetMenu(menuName = "Gambling/Negative/Invert Control")]
    public class InvertControlEvent : GamblingEvent
    {
        public override string Name => "Инвертирование управления";
        public override void Execute(GamblingManager manager)
        {
            // TODO: Инвертирование управления
        }
    }
}