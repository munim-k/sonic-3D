using UnityEngine;

public class FollowFixedRB : MonoBehaviour {
    [SerializeField] private Transform followObject;
    private Rigidbody rb;
    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void FixedUpdate() {
        if (followObject != null) {
            rb.MovePosition(followObject.position);
            rb.MoveRotation(followObject.rotation);
        }
    }
}
