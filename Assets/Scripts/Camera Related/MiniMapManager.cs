using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace FG
{
    public class MiniMapManager : MonoBehaviour
    {
        private Transform playerTransform;

        [Header("Config")]
        [SerializeField] private GameObject playerFollow;

        // ------------
        // UNITY EVENTS
        private void Start()
        {
            StartCoroutine(WaitForGameManagerInit());
        }

        private void Update()
        {
            if (playerTransform != null)
                playerFollow.transform.position = playerTransform.position;
        }

        private IEnumerator WaitForGameManagerInit()
        {
            while (GameManager.instance.player == null)
                yield return new WaitForFixedUpdate();

            playerTransform = GameManager.instance.player.transform;

            CinemachineCamera cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
            cinemachineCamera.Follow = playerTransform;

            yield return null;
        }
    }
}
