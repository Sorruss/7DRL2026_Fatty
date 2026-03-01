using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace FG
{
    public class CinemachineTargetGroupManager : SingletonMonoBehaviour<CinemachineTargetGroupManager>
    {
        private CinemachineTargetGroup targetGroup;

        // ------------
        // UNITY EVENTS
        protected override void Awake()
        {
            base.Awake();

            targetGroup = GetComponent<CinemachineTargetGroup>();
        }

        // ------------
        // MAIN METHODS
        public void AddTarget(Transform transform)
        {
            CinemachineTargetGroup.Target newTarget = new() { Weight = 1.0f, Radius = 1.0f, Object = transform };
            List<CinemachineTargetGroup.Target> targets = new() { newTarget };
            targetGroup.Targets = targets;
        }
    }
}
