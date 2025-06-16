using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private float spikeDamage = 10f;
    [SerializeField] private float spikeDuration = 3f;
    [SerializeField] private float spikeCooldown = 5f;

    private bool spikesActive = false;
    private float attackTimer = 0f;
    private float attackCooldownTimer = 0f;

    void Awake()
    {
        attackTimer = spikeDuration;
        attackCooldownTimer = 0.1f;
    }

    void FixedUpdate()
    {
        if (spikesActive)
        {
            attackTimer -= Time.fixedDeltaTime;
            if (attackTimer <= 0)
            {
                ToggleSpikes();
                attackCooldownTimer = spikeCooldown;
            }
        }
        else
        {
            attackCooldownTimer -= Time.fixedDeltaTime;
            if (attackCooldownTimer <= 0)
            {
                ToggleSpikes();
                attackTimer = spikeDuration;
            }
        }
    }

    public void ToggleSpikes()
    {
        spikesActive = !spikesActive;
        // gameObject.SetActive(spikesActive);
    }
    public void SetSpikesState(bool state)
    {
        spikesActive = state;
        // gameObject.SetActive(spikesActive);
    }

    public void ApplyDamage(GameObject target)
    {
        // Implement damage application logic here
    }
}