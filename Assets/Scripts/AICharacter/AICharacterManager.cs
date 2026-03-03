using UnityEngine;

namespace FG
{
    public class AICharacterManager : CharacterManager
    {
        // COMPONENTS
        [HideInInspector] public AICharacterLocomotionManager aiCharacterLocomotion;

        [Header("Config")]
        public string characterName = "AI Character";

        [Header("Chase")]
        public int chaseStartDistance = 50;

        protected override void Awake()
        {
            base.Awake();

            aiCharacterLocomotion = GetComponent<AICharacterLocomotionManager>();
        }
    }
}
