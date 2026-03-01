using System.Collections;
using UnityEngine;

namespace FG
{
    public class DoorLightningControl : MonoBehaviour
    {
        private bool isLit = false;
        private Door door;

        // ------------
        // UNITY EVENTS
        private void Awake()
        {
            door = GetComponentInParent<Door>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            PlayerManager player = collision.GetComponent<PlayerManager>();
            if (player == null)
                return;

            LitDoor();
        }

        // ------------
        // MAIN METHODS
        public void LitDoor()
        {
            Material varibleMaterial = new(ResourcesManager.instance.variableLitShader);

            if (!isLit)
            {
                SpriteRenderer[] renderers = GetComponentsInParent<SpriteRenderer>();
                foreach (SpriteRenderer renderer in renderers)
                    StartCoroutine(LitDoorCoroutine(renderer, varibleMaterial));

                isLit = true;
            }
        }

        // ----------
        // COROUTINES
        private IEnumerator LitDoorCoroutine(SpriteRenderer renderer, Material material)
        {
            renderer.material = material;
            for (float i = 0.05f; i < 1.0f; i += Time.deltaTime / GameManager.instance.roomFadeInTime)
            {
                material.SetFloat(ResourcesManager.instance.materialOpacityString, i);
                yield return null;
            }

            renderer.material = ResourcesManager.instance.defaultLitMaterial;

            yield return null;
        }
    }
}
