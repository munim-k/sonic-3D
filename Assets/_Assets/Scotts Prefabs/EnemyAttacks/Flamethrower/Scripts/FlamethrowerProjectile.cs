using UnityEngine;

public class FlamethrowerProjectile : MonoBehaviour {
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform trigger;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private LayerMask ignoreLayers;
    private Vector3 vel = Vector3.forward;
    private void Start() {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.linearVelocity = vel;
    }


    public void SetVelocity(Vector3 velocity) {
        rb.isKinematic = false;
        vel = velocity;
        rb.linearVelocity = vel;
    }


    private void OnTriggerEnter(Collider other) {

        if ((ignoreLayers & (1 << other.gameObject.layer)) != 0) {

        }
        else {
            Destroy(this.gameObject);
        }
    }

}
