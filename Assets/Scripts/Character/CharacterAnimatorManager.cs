using UnityEngine;

namespace FG
{
    public class CharacterAnimatorManager : MonoBehaviour
    {
        private CharacterManager character;

        [Header("Animator Parameters Cache")]
        [HideInInspector] public int AnimParamAimUp = Animator.StringToHash("aimUp");
        [HideInInspector] public int AnimParamAimUpLeft = Animator.StringToHash("aimUpLeft");
        [HideInInspector] public int AnimParamAimUpRight = Animator.StringToHash("aimUpRight");
        [HideInInspector] public int AnimParamAimLeft = Animator.StringToHash("aimLeft");
        [HideInInspector] public int AnimParamAimRight = Animator.StringToHash("aimRight");
        [HideInInspector] public int AnimParamAimDown = Animator.StringToHash("aimDown");
        [HideInInspector] public int AnimParamAimIsIdle = Animator.StringToHash("isIdle");
        [HideInInspector] public int AnimParamAimIsMoving = Animator.StringToHash("isMoving");

        // ------------
        // UNITY EVENTS
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        // -------------------------------
        // ANIMATOR PARAMS RELATED METHODS
        // AIMING
        private void ResetAimingAnimatorParams()
        {
            character.animator.SetBool(AnimParamAimUp, false);
            character.animator.SetBool(AnimParamAimUpLeft, false);
            character.animator.SetBool(AnimParamAimUpRight, false);
            character.animator.SetBool(AnimParamAimLeft, false);
            character.animator.SetBool(AnimParamAimRight, false);
            character.animator.SetBool(AnimParamAimDown, false);
        }

        public void HandleAimingAnimatorParams(AimDirection aimDirection)
        {
            ResetAimingAnimatorParams();

            switch (aimDirection)
            {
                case AimDirection.UP: character.animator.SetBool(AnimParamAimUp, true); break;
                case AimDirection.UP_LEFT: character.animator.SetBool(AnimParamAimUpLeft, true); break;
                case AimDirection.UP_RIGHT: character.animator.SetBool(AnimParamAimUpRight, true); break;
                case AimDirection.LEFT: character.animator.SetBool(AnimParamAimLeft, true); break;
                case AimDirection.RIGHT: character.animator.SetBool(AnimParamAimRight, true); break;
                case AimDirection.DOWN: character.animator.SetBool(AnimParamAimDown, true); break;
                default: break;
            }
        }
    }
}
