using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;

public class World7BossSpear : MonoBehaviour {
    [SerializeField] private float speed = 10f;
    [SerializeField] private float verticalLift = 0.5f;
    [SerializeField] private bool isFired = false;
    [SerializeField] private Transform spearCollisionParticles;
    [SerializeField] private LayerMask excludeMask;
    private bool isStopped = false;
    private Rigidbody rb;

    // Update is called once per frame

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate() {
        Vector3 playerPos = Player.CharacterInstance.playerBehaviourTree.modelTransform.position;
        playerPos.y += verticalLift;
        Vector3 playerDir = playerPos - transform.position;
        if (!isFired) {
            transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.LookRotation(playerDir);
        }
        else if (isFired && !isStopped) {
            Vector3 position = transform.position + transform.forward * speed * Time.fixedDeltaTime;
            rb.MovePosition(position);
        }
    }

    public void Fire() {
        isFired = true;
    }

    private void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.CompareTag("Player") && isFired) {
            if ((excludeMask & (1 << collision.gameObject.layer)) != 0) {
                // gameobject is in excludemask
            }
            else {
                // gameobject is not in excludemask
                isStopped = true;
                if (spearCollisionParticles != null) {
                    Instantiate(spearCollisionParticles, collision.GetContact(0).point, Quaternion.identity);
                }
                Destroy(gameObject,3f);
            }
        }
    }
}
