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

        protected virtual void Update()
        {
            
        }
    }
}
