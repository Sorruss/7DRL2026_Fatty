using UnityEngine;

namespace FG
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        private PlayerManager player;

        // ------------
        // UNITY EVENTS
        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        protected override void Update()
        {
            base.Update();

            if (!canMove)
                return;

            
        }

        protected override void HandleAimingDirection()
        {
            Vector2 aimVector = PlayerInputManager.instance.MousePosition - (Vector2)player.transform.position;
            float degrees = Helpers.GetVectorAngle(aimVector);
            aimDirection = Helpers.Degrees2AimDirection(degrees);
        }
    }
}
