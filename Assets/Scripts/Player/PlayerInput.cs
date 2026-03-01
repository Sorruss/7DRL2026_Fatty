using UnityEngine;

namespace FG
{
    public class PlayerInput : SingletonMonoBehaviour<PlayerInput>
    {
        private InputSystem_Actions inputSystem;

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

            // DEBUG KEYS
            inputSystem.Player.R.performed += _ => isRKeyActive = true;
        }

        private void OnDisable()
        {
            inputSystem.Disable();
        }

        // ------------
        // MAIN METHODS
        private void HandleAllInputs()
        {
            HandleRKeyInput();
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
