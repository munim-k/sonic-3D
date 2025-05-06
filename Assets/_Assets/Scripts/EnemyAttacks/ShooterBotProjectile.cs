using UnityEngine;

public class ShooterBotProjectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float timeToLive=1f;
    [SerializeField]private float speed=5f;
    private Vector3 launchDir;
    private bool destroy = false;
    private float timer = 0f;

    void Start()
    {
        timer = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(destroy)
            Destroy(gameObject);
        timer += Time.deltaTime;
        if (timer >= timeToLive)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position += launchDir * speed * Time.deltaTime;
        }
        //If projectile is overlapping with any colliders in a layer other than player layer then destroy projectile in next frame
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.4f, playerLayer);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer != playerLayer)
            {
                destroy=true;
                break;
            }
        }
    }

    public void SetLaunchDir(Vector3 l)
    {
        launchDir = l;
    }
}
