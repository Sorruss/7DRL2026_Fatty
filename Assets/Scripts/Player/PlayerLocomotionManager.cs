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
    }
}
