using UnityEngine;

namespace FG
{
    [CreateAssetMenu(menuName = "Scriptable Object/Dungeon Editor/Room Node Type")]
    public class RoomNodeType : ScriptableObject
    {
        [Header("Config")]
        public string roomName;
        public bool isCreatableByUser = true;

        [Header("Room's type")]
        public bool isCorridor;
        public bool isCorridorHorizontal;
        public bool isCorridorVertical;
        public bool isChestRoom;
        public bool isEntrance;
        public bool isBossRoom;
        public bool isNone;

        // ------------
        // UNITY EVENTS
#if UNITY_EDITOR
        private void OnValidate()
        {
            Helpers.ValidateStringProperty(this, nameof(roomName), roomName);
        }
#endif
    }
}
