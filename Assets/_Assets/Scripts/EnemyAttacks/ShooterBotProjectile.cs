using UnityEngine;

public class ShooterBotProjectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float timeToLive=1f;
    [SerializeField]private float speed=5f;
    [SerializeField] private Rigidbody rb;
    private Vector3 launchDir;
    private bool destroy = false;
    private float timer = 0f;

    void Start()
    {
        timer = 0f;
        rb.isKinematic = false;
        rb.linearVelocity = launchDir * speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(destroy)
            Destroy(gameObject);
        timer += Time.fixedDeltaTime;
        if (timer >= timeToLive)
        {
            Destroy(gameObject);
        }
        //If projectile is overlapping with any colliders in a layer other than player layer then destroy projectile in next frame
    }

    private void OnCollisionEnter(Collision collision)
    {
        destroy = true;
    }

    public void SetLaunchDir(Vector3 l)
    {
        launchDir = l;
    }
}
