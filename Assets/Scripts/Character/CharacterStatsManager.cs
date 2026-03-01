using UnityEngine;

namespace FG
{
    public class CharacterStatsManager : MonoBehaviour
    {
        private CharacterManager character;

        [Header("Health")]
        public float maxHealth;
        public float health;

        [Header("Flags")]
        public bool isDead = false;

        // ------------
        // UNITY EVENTS
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        // --------------
        // HEALTH METHODS
        public void DamageHealth(float amount)
        {
            float newHealth = health - amount;
            if (newHealth <= 0)
            {
                health = 0.0f;
                character.Die();
                return;
            }

            health = newHealth;
        }

        public void AddHealth(float amount)
        {
            float newHealth = health + amount;
            if (newHealth > maxHealth)
                newHealth = maxHealth;

            health = newHealth;
        }
    }
}
