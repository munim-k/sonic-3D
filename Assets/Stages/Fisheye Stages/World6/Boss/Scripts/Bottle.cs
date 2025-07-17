using System;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    [SerializeField] private GameObject areaOfEffectPrefab;
    [SerializeField] private float areaOfEffectDuration = 2f;
    private GameObject boss;
    private Rigidbody rb;
    public static Action OnBottleHitBoss;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on the bottle.");
        }
    }

    private void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");
        if (boss == null)
        {
            Debug.LogError("Boss not found in the scene.");
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Vector3 targetPos = boss.transform.position;
            float height = 3f; // arc height
            float gravity = Mathf.Abs(Physics.gravity.y);

            Vector3 displacement = targetPos - transform.position;
            Vector3 horizontalDisplacement = new Vector3(displacement.x, 0, displacement.z);
            float distance = horizontalDisplacement.magnitude;

            float verticalDisplacement = displacement.y;

            float time = Mathf.Sqrt(2 * height / gravity) + Mathf.Sqrt(2 * (height - verticalDisplacement) / gravity);

            Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * gravity * height);
            Vector3 velocityXZ = horizontalDisplacement / time;

            Vector3 finalVelocity = velocityXZ + velocityY;

            rb.AddForce(finalVelocity, ForceMode.VelocityChange);
        }
        else if (other.tag == "Ground")
        {
            // Instantiate the area of effect prefab at the bottle's position
            GameObject areaOfEffect = Instantiate(areaOfEffectPrefab, new Vector3(transform.position.x, other.transform.position.y + 0.1f, transform.position.z), Quaternion.identity);
            Destroy(areaOfEffect, areaOfEffectDuration); // Destroy the area of effect after a set duration

            // Destroy the bottle after the area of effect is created
            Destroy(gameObject);
        }
        else if(other.tag == "Boss")
        {
            OnBottleHitBoss?.Invoke();
            Destroy(gameObject); // Destroy the bottle if it hits the boss
        }
    }
}
