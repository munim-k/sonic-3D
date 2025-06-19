using UnityEngine;

public class World5BossSmashParticles : MonoBehaviour {
    [SerializeField] private ParticleSystem smashParticles;
    [SerializeField] private GameObject smashHitbox;
    private void EnableSmashHitbox() {
        smashHitbox.SetActive(true);
        if (smashParticles != null) {
            smashParticles.Play();
        }
    }

}
