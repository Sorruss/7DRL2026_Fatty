using UnityEngine;

namespace FG
{
    public class AICharacterLocomotionManager : CharacterLocomotionManager
    {
        private AICharacterManager aiCharacter;

        protected override void Awake()
        {
            base.Awake();

            aiCharacter = GetComponent<AICharacterManager>();
        }

        
    }
}
