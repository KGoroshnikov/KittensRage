using UnityEngine;

namespace Projectiles
{
    public class StupidThrowableCat : ThrowableCat
    {
        [SerializeField] private float angle = 45;
        public override void Send(CatSling sling)
        {
            sling.RotateAnchor(-angle);
            base.Send(sling);
        }

        protected override void OnCollisionEnter()
        {
            base.OnCollisionEnter();
            Sling.RotateAnchor(angle);
        }
    }
}