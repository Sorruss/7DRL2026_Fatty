using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FG
{
    public class CharacterLocomotionManager : MonoBehaviour
    {
        private CharacterManager character;

        [Header("Ground Movement Config")]
        public float speed;

        [Header("Flags")]
        public bool isMoving = false;
        public bool canMove = true;

        // EVENT ACTIONS
        [HideInInspector] public event Action<bool> IsMovingChangeEvent;

        // ------------
        // UNITY EVENTS
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Update()
        {
            
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
    }
}
