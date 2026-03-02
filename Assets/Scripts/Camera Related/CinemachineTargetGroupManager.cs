using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace FG
{
    public class CinemachineTargetGroupManager : SingletonMonoBehaviour<CinemachineTargetGroupManager>
    {
        private CinemachineTargetGroup targetGroup;

        [Header("Config")]
        [SerializeField] private Transform mouseFollowTransform;

        // ------------
        // UNITY EVENTS
        protected override void Awake()
        {
            base.Awake();

            targetGroup = GetComponent<CinemachineTargetGroup>();
        }

        private void Start()
        {
            AddTarget(mouseFollowTransform);
        }

        private void Update()
        {
            // POSITION OF MOUSE FOR CM TARGET GROUP
            mouseFollowTransform.position = PlayerInputManager.instance.MousePosition;
        }

        // ------------
        // MAIN METHODS
        public void AddTarget(Transform transform, float radius = 1.0f)
        {
            CinemachineTargetGroup.Target newTarget = new() { Weight = 1.0f, Radius = radius, Object = transform };
            //List<CinemachineTargetGroup.Target> targets = new() { newTarget };
            targetGroup.Targets.Add(newTarget);
        }
    }
}
