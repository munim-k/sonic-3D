using UnityEngine;

namespace RagdollEngine
{
    public class DamagePlayerBehaviour : PlayerBehaviour
    {
        // Serialized fields for audio and visual effects
        [SerializeField] AudioSource audioSource; // Audio source for playing sound effects
        [SerializeField] AudioClip knockbackAudioClip; // Sound effect for knockback
        [SerializeField] AudioClip stumbleAudioClip; // Sound effect for stumble
        [SerializeField] AudioClip ringSpreadAudioClip; // Sound effect for ring scattering
        [SerializeField] ParticleSystem ringScatterEffectPrefab; // Particle effect for ring scattering

        // Serialized fields for knockback and stumble parameters
        [SerializeField] float knockbackDistance; // Distance the player is pushed during knockback
        [SerializeField] float knockbackTime; // Duration of the knockback effect
        [SerializeField] float knockbackHeight; // Height of the knockback arc
        [SerializeField] float stumbleSpeedMultiplier; // Speed reduction during stumble
        [SerializeField] float minStumbleSpeed; // Minimum speed required to trigger stumble
        [SerializeField] float knockbackAnimationTime; // Duration of the knockback animation
        [SerializeField] float stumbleAnimationTime; // Duration of the stumble animation
        [SerializeField] float cooldownTime; // Cooldown time before the player can take damage again

        // Enum to define the type of damage mode
        public enum DamageMode
        {
            Knockback, // Player is pushed back
            Stumble    // Player stumbles but remains in place
        }

        // Private fields to manage state
        DamageMode damageMode; // Current damage mode
        Vector3 knockbackDirection; // Direction of the knockback
        bool knockback; // Whether the player is currently in knockback
        bool stumbling; // Whether the player is currently stumbling
        bool damageCheck; // Whether damage is being checked
        float knockbackTimer; // Timer for knockback duration
        float animationTimer; // Timer for animation duration
        float cooldownTimer; // Timer for cooldown duration

        // Updates the player's state in the LateUpdate phase
        void LateUpdate()
        {
            // Update the stumbling state based on the animation timer
            stumbling = stumbling && animationTimer > 0;

            // Update the animator to reflect the stumbling state
            animator.SetBool("Stumbling", stumbling);
        }

        // Evaluates whether the behavior should be active
        public override bool Evaluate()
        {
            // Decrease the cooldown timer
            cooldownTimer = Mathf.Max(cooldownTimer - Time.fixedDeltaTime, 0);

            // Check if the player is taking damage or if damage is being checked
            return EvaluateDamage() || DamageCheck();
        }

        // Handles the knockback effect
        void Knockback()
        {
            // Override the model's transform to control its position and rotation
            overrideModelTransform = true;

            // Decrease the knockback timer
            knockbackTimer -= Time.fixedDeltaTime;

            // Calculate the progress of the knockback effect (0 to 1)
            float t = Mathf.Clamp01(1 - (knockbackTimer / knockbackTime));

            // Determine the "up" direction based on the ground's normal or default to Vector3.up
            Vector3 up = groundInformation.ground ? groundInformation.hit.normal : Vector3.up;

            // Rotate the model to face the knockback direction
            modelTransform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(-knockbackDirection, up), up);

            // Calculate the player's position during the knockback arc
            modelTransform.position = (groundInformation.cast ? groundInformation.hit.point : playerTransform.position - (modelTransform.up * height))
                + (up * knockbackHeight * Mathf.Sin(t * Mathf.PI));

            // If knockback is complete or the timer runs out
            if (!knockback || knockbackTimer < 0)
            {
                // Stop the player's velocity if grounded
                if (groundInformation.ground)
                    additiveVelocity = -RB.linearVelocity;

                return;
            }

            // If the player is not grounded, stop the knockback
            if (!groundInformation.cast)
            {
                knockback = false;
                return;
            }

            // Apply knockback velocity
            additiveVelocity = -RB.linearVelocity
                + (knockbackDirection * knockbackDistance / knockbackTime);
        }

        // Evaluates whether the player is currently taking damage
        bool EvaluateDamage()
        {
            // If the behavior was not active in the previous frame, return false
            if (!wasActive) return false;

            // Decrease the animation timer
            animationTimer -= Time.fixedDeltaTime;

            // If the animation timer has expired, return false
            if (animationTimer <= 0) return false;

            // Handle the current damage mode
            switch (damageMode)
            {
                case DamageMode.Knockback:
                    Knockback(); // Apply knockback effect
                    break;
                case DamageMode.Stumble:
                    // If the player's speed is too low or they are not grounded, stop stumbling
                    if (RB.linearVelocity.magnitude < minStumbleSpeed || !groundInformation.ground)
                    {
                        animationTimer = 0;
                        return false;
                    }
                    break;
            }

            return true; // Damage is still being applied
        }

        // Checks if the player should take damage
        bool DamageCheck()
        {
            // Store the previous damage check state
            bool wasDamageCheck = damageCheck;

            // Reset the damage check state
            damageCheck = false;

            // If the cooldown timer is still active, return false
            if (cooldownTimer > 0) return false;

            // Iterate through all volumes the player is interacting with
            foreach (Volume thisVolume in volumes)
                if (thisVolume is DamageVolume)
                {
                    // Cast the volume to a DamageVolume
                    DamageVolume thisDamageVolume = thisVolume as DamageVolume;

                    // Set the damage mode based on the volume
                    damageMode = thisDamageVolume.damageMode;

                    // Handle the damage mode
                    switch (damageMode)
                    {
                        case DamageMode.Knockback:
                            // Initialize knockback parameters
                            knockbackTimer = knockbackTime;
                            animationTimer = knockbackAnimationTime;
                            cooldownTimer = cooldownTime;

                            // Calculate the knockback direction
                            knockbackDirection = Vector3.ProjectOnPlane(-RB.linearVelocity, Vector3.up).normalized;
                            if (knockbackDirection.magnitude == 0)
                                knockbackDirection = -modelTransform.forward;

                            // Set knockback state
                            knockback = true;
                            stumbling = false;

                            // Apply knockback effect
                            Knockback();

                            // Play knockback sound
                            audioSource.PlayOneShot(knockbackAudioClip);

                            // Trigger knockback animation
                            animator.SetTrigger("Knockback");
                            break;

                        default: // Stumble mode
                            // If damage was already checked or the player's speed is too low, continue
                            if (wasDamageCheck || RB.linearVelocity.magnitude < minStumbleSpeed)
                            {
                                damageCheck = true;
                                continue;
                            }

                            // Initialize stumble parameters
                            knockbackTimer = 0;
                            animationTimer = stumbleAnimationTime;
                            cooldownTimer = 0;

                            // Set stumble state
                            knockback = false;
                            stumbling = true;

                            // Apply stumble velocity
                            additiveVelocity = -RB.linearVelocity * (1 - stumbleSpeedMultiplier);

                            // Play stumble sound
                            audioSource.PlayOneShot(stumbleAudioClip);

                            // Trigger stumble animation
                            animator.SetTrigger("Stumble");
                            break;
                    }

                    // Handle ring scattering if the damage volume has power
                    if (thisDamageVolume.power > 0)
                    {
                        if (!Rings.HasRings(playerBehaviourTree) || Rings.GetRings(playerBehaviourTree) == 0)
                            character.Respawn(); // Respawn if no rings
                        else
                        {
                            // Calculate lost rings
                            int rings = Rings.GetRings(playerBehaviourTree);
                            int lost = Mathf.FloorToInt(thisDamageVolume.power);

                            // Subtract lost rings
                            Rings.AddRings(playerBehaviourTree, -lost);

                            // Play ring scattering sound
                            audioSource.PlayOneShot(ringSpreadAudioClip);

                            // Instantiate and play ring scatter effect
                            ParticleSystem ringScatterEffect = Instantiate(ringScatterEffectPrefab);
                            ringScatterEffect.transform.position = playerTransform.position;
                            ringScatterEffect.emission.SetBursts(new ParticleSystem.Burst[]
                                {
                                    new()
                                    {
                                        count = Mathf.Min(rings, lost)
                                    }
                                });
                            ringScatterEffect.Play();
                        }
                    }

                    // Set damage check state
                    damageCheck = true;

                    return true; // Damage was applied
                }

            return false; // No damage was applied
        }
    }
}
