using UnityEngine;

public class World6BossFirework : MonoBehaviour {
    [SerializeField] private float travelSpeed = 10f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float verticalLift = 0.5f;
    [SerializeField] private Transform damageTrigger;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private LayerMask ignoreLayers;
    [SerializeField] private Transform explosionParticles;
    [SerializeField] private Rigidbody rb;
    private Quaternion rotToPlayer;
    private Vector3 targetPos;
   
    void FixedUpdate() {
        targetPos = Player.CharacterInstance.playerBehaviourTree.modelTransform.position;
        rb.linearVelocity = transform.forward * travelSpeed;
        Vector3 vecToPlayer = targetPos - transform.position;
        vecToPlayer.Normalize();
        vecToPlayer.y += verticalLift;
        vecToPlayer.Normalize();
        rotToPlayer = Quaternion.LookRotation(vecToPlayer, transform.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotToPlayer, rotationSpeed * Time.fixedDeltaTime);
        if (vecToPlayer.sqrMagnitude <= 0.1f) {
            Explode();
        }
    }
   
    private void OnTriggerEnter(Collider other) {
        if ((ignoreLayers & (1 << other.gameObject.layer)) != 0) {

        }
        else {
           
            Explode();
          
        }

    }


    public void Explode() {
        if (particles != null) {
            DetachParticles();
        }
        if (explosionParticles != null) {
            Instantiate(explosionParticles, transform.position, Quaternion.identity);
        }
        if (damageTrigger != null) {
            damageTrigger.gameObject.SetActive(true);
        }
        Destroy(gameObject, 0.1f); // Delay to allow damage trigger to activate
    }
    public void DetachParticles() {
        // This splits the particle off so it doesn't get deleted with the parent
        particles.transform.parent = null;

        particles.Stop();
        // This finds the particleAnimator associated with the emitter and then
        // sets it to automatically delete itself when it runs out of particles
        Destroy(particles.transform.gameObject, particles.main.startLifetime.constantMax);
    }


}
