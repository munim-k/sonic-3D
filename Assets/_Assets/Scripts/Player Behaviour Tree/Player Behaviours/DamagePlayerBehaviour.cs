using System;
using UnityEngine;

namespace RagdollEngine {
    public class DamagePlayerBehaviour : PlayerBehaviour {
        // Serialized fields for audio and visual effects
        [SerializeField] AudioSource audioSource; // Audio source for playing sound effects
        [SerializeField] AudioClip damageAudioClip;
        [SerializeField] float cooldownTime; // Cooldown time before the player can take damage again
        // Private fields to manage state
        float cooldownTimer; // Timer for cooldown duration
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private GameObject HealthUIPrefab;
        //Include current and max health in Action

        public Action<int, int> onDamage; // Action to be invoked when the player takes damage
        private int currentHealth;

        public enum DamageType {
            Single,
            Persistent,
        }


        private void Start() {
            GameObject healthUI = Instantiate(HealthUIPrefab, playerBehaviourTree.canvas.transform);
            playerBehaviourTree.character.uis.Add(healthUI);
            healthUI.GetComponent<HealthUI>().Initialize(this);
            currentHealth = maxHealth;
        }


        // Evaluates whether the behavior should be activ
        public override bool Evaluate() {
            // Decrease the cooldown timer
            cooldownTimer = Mathf.Max(cooldownTimer - Time.fixedDeltaTime, 0);
            return DamageCheck();
        }

        // Checks if the player should take damage
        bool DamageCheck() {
            // If the cooldown timer is still active, return false
            if (cooldownTimer > 0)
                return false;

            // Iterate through all volumes the player is interacting with
            foreach (Volume thisVolume in volumes) {
                if (thisVolume is DamageVolume) {
                    DamageVolume thisDamageVolume = thisVolume as DamageVolume;
                    if (thisDamageVolume.damageDealt && thisDamageVolume.damageMode == DamageType.Single) {
                        continue; // Skip if damage has already been dealt by this volume and its a single damageType
                    }
                    // Deplete health based on the power of the damage volume
                    int damageAmount = Mathf.FloorToInt(thisDamageVolume.power);
                    currentHealth = Mathf.Max(currentHealth - damageAmount, 0);
                    //Reset damageDealt variable of damageVolume
                    thisDamageVolume.damageDealt = true; // Mark that damage has been dealt
                    // Play damage sound
                    audioSource.PlayOneShot(damageAudioClip);


                    // Set cooldown timer
                    cooldownTimer = cooldownTime;

                    //To apply knockback
                    Vector3 knockbackDir = modelTransform.position - thisDamageVolume.transform.position;
                    knockbackDir.Normalize();
                    knockbackDir.y = 0;
                    knockbackDir *= thisDamageVolume.horizontalKnockback;
                    knockbackDir.y += thisDamageVolume.verticalKnockback;
                    additiveVelocity += knockbackDir;
                    Debug.DrawLine(modelTransform.position, modelTransform.position + knockbackDir, Color.red, 3f);
                    moving = true;
                    print("velocity in damagebehaviour: " + additiveVelocity);
                    // Check if the player has no health left
                    if (currentHealth <= 0) {
                        character.Respawn(); // Respawn the player if health is depleted
                    }
                    onDamage?.Invoke(currentHealth, maxHealth); // Invoke the damage event
                    return true; // Damage was applied
                }
            }
            return false;
        }
    }
}
