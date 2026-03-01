using UnityEngine;

namespace FG
{
    public class CharacterManager : MonoBehaviour
    {
        [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;
        [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
        [HideInInspector] public CharacterStatsManager characterStatsManager;
        [HideInInspector] public SpriteRenderer spriteRenderer;
        [HideInInspector] public Animator animator;

        // ------------
        // UNITY EVENTS
        protected virtual void Awake()
        {
            characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterStatsManager = GetComponent<CharacterStatsManager>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        // ------------
        // MAIN METHODS
        public void Die()
        {
            characterStatsManager.isDead = true;
            characterLocomotionManager.canMove = false;
            //Destroy(gameObject);
        }
    }
}
