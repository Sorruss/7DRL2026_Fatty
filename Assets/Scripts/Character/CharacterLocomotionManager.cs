using System;
using UnityEngine;

namespace FG
{
    public class CharacterLocomotionManager : MonoBehaviour
    {
        private CharacterManager character;

        [Header("Ground Movement Config")]
        public float speed;
        public AimDirection aimDirection;

        [Header("Flags")]
        public bool isMoving = false;
        public bool canMove = true;

        // EVENT ACTIONS
        [HideInInspector] public event Action<bool> IsMovingChangeEvent;
        [HideInInspector] public event Action<float, float> SpeedChangeEvent;

        // ------------
        // UNITY EVENTS
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();

            // TRIGGER SUBSCRIPTIONS
            SpeedChangeEvent?.Invoke(0.0f, speed);
        }

        protected virtual void Update()
        {
            // AIMING
            HandleAimingDirection();
            character.characterAnimatorManager.HandleAimingAnimatorParams(aimDirection);
        }

        // ----------------
        // MOVEMENT METHODS
        public void GroundMove(Vector2 movementInput)
        {
            character.ridigBody.linearVelocity = movementInput * speed;
        }

        public void SetIsMoving(bool value)
        {
            isMoving = value;
            IsMovingChangeEvent?.Invoke(value);
        }

        protected virtual void HandleAimingDirection()
        {

        }

        // -------
        // SETTERS
        private void SetSpeed(float newSpeed)
        {
            float oldSpeed = speed;
            speed = newSpeed;
            SpeedChangeEvent?.Invoke(oldSpeed, speed);
        }
    }
}
