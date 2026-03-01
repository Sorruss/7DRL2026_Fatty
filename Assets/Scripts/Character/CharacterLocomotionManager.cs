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
        public bool canMove = true;

        // ------------
        // UNITY EVENTS
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        private void Update()
        {
            if (!canMove)
                return;

            /*// player look
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            body.up = mousePos - new Vector2(body.position.x, body.position.y);

            // player move
            var input = movementAction.action.ReadValue<Vector2>();
            rb.linearVelocity = input.normalized * speed;*/
        }
    }
}
