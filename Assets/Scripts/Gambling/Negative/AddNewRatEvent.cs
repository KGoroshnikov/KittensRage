using UnityEngine;

namespace Gambling.Negative
{
    [CreateAssetMenu(menuName = "Gambling/Negative/Add New Rat")]
    public class AddNewRatEvent : GamblingEvent
    {
        public override string Name => "Добавление крысы";
        public override void Execute(GamblingManager manager) => Instantiate(manager.ratTowerPrefab, manager.ratTowerParent);
    }
}