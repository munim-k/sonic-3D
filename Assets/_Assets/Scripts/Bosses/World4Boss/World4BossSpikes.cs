using UnityEngine;

public class World4BossSpikes : MonoBehaviour
{
    [SerializeField] private GameObject[] spikes;
    [SerializeField] private bool spikesActive = false;
    [SerializeField] private float spikeDamage = 10f;
    [SerializeField] private float spikeDuration = 3f;
    [SerializeField] private float spikeCooldown = 5f;

    private float spikeCooldownTimer = 0f;

    void Update()
    {
        if (spikeCooldownTimer > 0)
        {
            spikeCooldownTimer -= Time.deltaTime;
        }
        else if (Input.GetKeyDown(KeyCode.Space)) // Example trigger for spikes
        {
            ToggleSpikes();
            spikeCooldownTimer = spikeCooldown;
        }
    }

    void ToggleSpikes()
    {
        spikesActive = !spikesActive;
        foreach (GameObject spike in spikes)
        {
            spike.SetActive(spikesActive);
        }
    }
}