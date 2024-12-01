using UnityEngine;

namespace Gambling.Negative
{
    [CreateAssetMenu(menuName = "Gambling/Negative/Add New Rat")]
    public class AddNewRatEvent : GamblingEvent
    {
        public override string Name => "Добавление крысы";
        public override void Execute(GamblingManager manager)
        {
            var parent = Random.Range(0, manager.ratTowerParentVariants.Count);
            Instantiate(manager.ratTowerPrefab, manager.ratTowerParentVariants[parent]);
            manager.ratTowerParentVariants.RemoveAt(parent);
        }
    }
}