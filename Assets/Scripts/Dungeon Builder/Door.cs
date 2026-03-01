using UnityEngine;

namespace FG
{
    public class Door : MonoBehaviour
    {
        private Animator animator;
        private BoxCollider2D doorOpenTrigger;

        private int AnimatorParamOpen = Animator.StringToHash("open");

        [Header("Config")]
        [SerializeField] private BoxCollider2D hardCollider;

        [Header("Flags")]
        public bool isBossRoom = false;
        public bool isOpen = false;
        public bool wasPreviouslyOpen = false;

        // -------------
        // UNITY METHODS
        private void Awake()
        {
            animator = GetComponent<Animator>();
            doorOpenTrigger = GetComponent<BoxCollider2D>();

            EnableHardCollider(false);
        }

        private void OnEnable()
        {
            animator.SetBool(AnimatorParamOpen, isOpen);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            PlayerManager player = collision.GetComponent<PlayerManager>();
            if (player == null)
                return;

            OpenDoor();
        }

        // ------------
        // MAIN METHODS
        private void OpenDoor()
        {
            if (isOpen)
                return;

            isOpen = true;
            wasPreviouslyOpen = true;
            EnableHardCollider(false);

            animator.SetBool(AnimatorParamOpen, isOpen);
        }

        private void CloseDoor()
        {
            if (!isOpen)
                return;

            isOpen = false;
            animator.SetBool(AnimatorParamOpen, isOpen);
        }

        public void LockDoor(bool lockDoor)
        {
            EnableTrigger(!lockDoor);

            if (lockDoor)
            {
                CloseDoor();
                EnableHardCollider(true);
            }
            else
            {
                if (wasPreviouslyOpen)
                {
                    OpenDoor();
                }
            }
        }

        // -------
        // HELPERS
        private void EnableHardCollider(bool enable)
        {
            hardCollider.enabled = enable;
        }

        private void EnableTrigger(bool enable)
        {
            doorOpenTrigger.enabled = enable;
        }
    }
}
