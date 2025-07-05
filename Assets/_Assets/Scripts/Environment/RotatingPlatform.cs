using UnityEngine;
using System.Collections;

public class RotatingPlatform : MonoBehaviour {
    [SerializeField] private float rotationSpeed = 30f; // Speed of rotation in degrees per second.
    [SerializeField] private float waitTimeBetweenRotations = 2f;
    [SerializeField] private float rotationAngle = 90f; // Angle to rotate the platform.

    private float rotationSpeedUseable = 0f;
    private float lastRotationY = 0f;
    private GameObject player = null;
    private GameObject highestParent = null;
    private float rotatedThisSegment = 0f;
    private bool isWaiting = false;
    private void Start() {
        // Initialize the rotation speed to zero.
        rotationSpeedUseable = rotationSpeed;
    }

    private void OnTriggerEnter(Collider other) {
        // Check if the object entering the trigger is a player.
        if (other.tag == "Player") {
            player = other.gameObject; // Store a reference to the player.
            while(player.transform.parent != null) {
                player = player.transform.parent.gameObject; // Traverse up the hierarchy to find the root player object.
            }

            highestParent = player; // Store the highest parent of the player.

            //set the most high parent as the child of this platform
            player.transform.SetParent(transform, true);

            Debug.Log("Player has entered the platform trigger.");
        }
    }

    private void OnTriggerExit(Collider other) {
        // Check if the object exiting the trigger is a player.
        if (other.tag == "Player") {

            player = other.gameObject; // Store a reference to the player.

            while (player != highestParent) {
                player = player.transform.parent.gameObject; // Traverse up the hierarchy to find the root player object.
            }

            //set the most high parent as the child of null
            player.transform.SetParent(null, true);

            Debug.Log("Player has exited the platform trigger.");
        }
    }

    private void FixedUpdate() {
        if (isWaiting)
            return;

        float rotationThisFrame = rotationSpeed * Time.fixedDeltaTime;

        // Calculate remaining rotation for this segment
        float remainingRotation = rotationAngle - rotatedThisSegment;

        // Clamp to avoid overshooting
        float actualRotation = Mathf.Min(rotationThisFrame, remainingRotation);

        // Rotate the platform
        transform.Rotate(Vector3.up, actualRotation);
        rotatedThisSegment += actualRotation;

        // Segment complete
        if (rotatedThisSegment >= rotationAngle) {
            rotatedThisSegment = 0f;
            StartCoroutine(WaitBeforeNextSegment());
        }
    }

    private IEnumerator WaitBeforeNextSegment() {
        isWaiting = true;
        yield return new WaitForSeconds(waitTimeBetweenRotations);
        isWaiting = false;
    }
}
