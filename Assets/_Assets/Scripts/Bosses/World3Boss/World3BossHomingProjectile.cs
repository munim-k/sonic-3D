using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;

public class World3BossHomingProjectile : MonoBehaviour
{
    [SerializeField] private float travelSpeed = 10f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float verticalLift = 0.5f;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private LayerMask ignoreLayers;
    [SerializeField] private Rigidbody rb;
    private Quaternion rotToPlayer;
    void FixedUpdate()
    {
        rb.linearVelocity = transform.forward * travelSpeed;
        Vector3 vecToPlayer = Player.CharacterInstance.playerBehaviourTree.modelTransform.position - transform.position;
        vecToPlayer.Normalize();
        vecToPlayer.y += verticalLift;
        vecToPlayer.Normalize();
        rotToPlayer = Quaternion.LookRotation(vecToPlayer, transform.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotToPlayer, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((ignoreLayers & (1 << other.gameObject.layer)) != 0)
        {

        }
        else
        {
            DetachParticles();
            Destroy(this.gameObject);
        }

    }

    public void DetachParticles()
    {
        // This splits the particle off so it doesn't get deleted with the parent
        particles.transform.parent = null;

        particles.Stop();
        // This finds the particleAnimator associated with the emitter and then
        // sets it to automatically delete itself when it runs out of particles
        Destroy(particles.transform.gameObject, particles.main.startLifetime.constantMax);
    }


}
