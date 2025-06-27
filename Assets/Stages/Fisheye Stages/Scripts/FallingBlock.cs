using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    private Rigidbody rb;

    private bool isFalling = false;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeAmount = 0.1f;
    private float shakeTimer = 0f;
    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.localPosition;
        shakeTimer = shakeDuration;
        rb.isKinematic = true;
    }

    private void Update()
    {
        if (isFalling && shakeTimer > 0f)
        {
            Vector3 shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * shakeAmount,
                0f,
                Random.Range(-1f, 1f) * shakeAmount
            );

            transform.localPosition = originalPosition + shakeOffset;

            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                transform.localPosition = originalPosition;
                isFalling = false;
                rb.isKinematic = false;
            }
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isFalling = true;
            Debug.Log("Block is falling!");
        }
    }


}
