using UnityEngine;

namespace FG
{
    public class PlayerStatsManager : CharacterStatsManager
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
