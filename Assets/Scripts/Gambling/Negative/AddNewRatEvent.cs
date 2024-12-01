using AI;
using UnityEngine;
using UnityEngine.VFX;

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
        public override void Execute(WheelOfFortune manager)
        {
            /*var parent = Random.Range(0, manager.ratTowerParentVariants.Count);
            Instantiate(manager.ratTowerPrefab, manager.ratTowerParentVariants[parent]);
            manager.ratTowerParentVariants.RemoveAt(parent);*/

            int amountSpawn = Random.Range(1, 3);
            for(int i = 0; i < amountSpawn; i++){
                Vector3 pos = manager.spawnPoints[Random.Range(0, manager.spawnPoints.Length)].position;
                GameObject rat = Instantiate(manager.ratPref, pos, Quaternion.Euler(0, -223f, 0));
                rat.GetComponent<ArcherAI>().AllowAttack(true);
                rat.transform.SetParent(manager.mainParent);

                Instantiate(manager.puffEffect.gameObject, pos, Quaternion.identity).GetComponent<VisualEffect>().Play();
            }
        }
    }
}