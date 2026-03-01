using System;
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

        // EVENT ACTIONS
        [HideInInspector] public event Action<float> HealthChangeEvent;

        // ------------
        // UNITY EVENTS
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        private void OnEnable()
        {
            HealthChangeEvent += OnHealthChanged;
        }

        private void OnDisable()
        {
            HealthChangeEvent -= OnHealthChanged;
        }

        // ------
        // EVENTS
        private void OnHealthChanged(float newValue)
        {
            if (newValue <= 0.0f)
            {
                health = 0.0f;
                character.Die();
            }
            else if (newValue > maxHealth)
            {
                health = maxHealth;
            }
        }

        // --------------
        // HEALTH METHODS
        public void DamageHealth(float amount)
        {
            health -= amount;
            HealthChangeEvent?.Invoke(health);
        }

        public void AddHealth(float amount)
        {
            health += amount;
            HealthChangeEvent?.Invoke(health);
        }
    }
}
