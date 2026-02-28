using System.Collections.Generic;
using UnityEngine;

namespace FG
{
    // IT'S LIKE AN ENUM, SO CREATED ONCE IN THE PROJECT AND USED FOR EVERY NODE GRAPH
    [CreateAssetMenu(menuName = "Scriptable Object/Dungeon Editor/Room Node Type List")]
    public class RoomNodeTypeList : ScriptableObject
    {
        public List<RoomNodeType> roomNodeTypes;

        // ------------
        // UNITY EVENTS
#if UNITY_EDITOR
        private void OnValidate()
        {
            Helpers.ValidateEnumerableProperty(this, nameof(roomNodeTypes), roomNodeTypes);
        }
#endif
    }
}
