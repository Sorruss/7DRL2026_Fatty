using UnityEngine;

namespace FG
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        private PlayerManager player;

        [Header("Debug")]
        public AimDirection aimDirection;

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

            HandleAiming();

            if (!canMove)
                return;

            
        }

        // ------
        // AIMING
        private void HandleAiming()
        {
            HandleAimingDirection();
            player.playerAnimatorManager.HandleAimingAnimatorParams(aimDirection);
        }

        private void HandleAimingDirection()
        {
            Vector2 aimVector = PlayerInputManager.instance.MousePosition - (Vector2)player.transform.position;
            float degrees = Helpers.GetVectorAngle(aimVector);
            
            if (degrees > 22.0f && degrees < 67.0f)
                aimDirection = AimDirection.UP_RIGHT;
            else if (degrees >= 67.0f && degrees < 112.0f)
                aimDirection = AimDirection.UP;
            else if (degrees >= 112.0f && degrees < 158.0f)
                aimDirection = AimDirection.UP_LEFT;
            else if (degrees <= 22.0f && degrees > -45.0f)
                aimDirection = AimDirection.RIGHT;
            else if (degrees <= -45.0f && degrees > -135.0f)
                aimDirection = AimDirection.DOWN;
            else if (degrees <= -135.0f || degrees >= 158.0f)
                aimDirection = AimDirection.LEFT;
        }
    }
}
