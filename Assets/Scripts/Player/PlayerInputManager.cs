using UnityEngine;
using UnityEngine.InputSystem;

namespace FG
{
    public class PlayerInputManager : SingletonMonoBehaviour<PlayerInputManager>
    {
        private InputSystem_Actions inputSystem;

        [Header("Camera Input")]
        [SerializeField] private Vector2 MouseDelta;
        public Vector2 MousePosition;

        [Header("Debug Inputs")]
        [SerializeField] private bool isRKeyActive = false;

        // ------------
        // UNITY EVENTS
        private void Update()
        {
            HandleAllInputs();
        }

        private void OnEnable()
        {
            if (inputSystem == null)
                inputSystem = new InputSystem_Actions();

            inputSystem.Enable();

            // CAMERA INPUT
            inputSystem.Player.Look.performed += x => MouseDelta = x.ReadValue<Vector2>();

            // DEBUG KEYS
            inputSystem.Player.R.performed += _ => isRKeyActive = true;
        }

        private void OnDisable()
        {
            if (inputSystem != null)
                inputSystem.Disable();
        }

        // ------------
        // MAIN METHODS
        private void HandleAllInputs()
        {
            // MAIN
            HandleCameraInput();
            // DEBUG
            HandleRKeyInput();
        }

        // -----------
        // MAIN INPUTS
        private void HandleCameraInput()
        {
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0.0f, Screen.width);
            mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0.0f, Screen.height);
            MousePosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        }

        // ------------
        // DEBUG INPUTS
        private void HandleRKeyInput()
        {
            if (isRKeyActive)
            {
                isRKeyActive = false;
                GameManager.instance.ChangeGameState(GameState.GAME_RESTART);
            }
        }
    }
}
