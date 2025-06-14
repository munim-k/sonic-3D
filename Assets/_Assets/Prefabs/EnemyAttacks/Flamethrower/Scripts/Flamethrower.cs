using UnityEngine;

public class Flamethrower : MonoBehaviour {
    [SerializeField] private FlamethrowerProjectile projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float fireStep = 1f;
    private float fireTimer = 0f;
    [SerializeField] private bool firing = false;



    private void FixedUpdate() {
        fireTimer += Time.fixedDeltaTime;
        if (firing && fireTimer >= fireStep) {
            Fire();
            fireTimer = 0f;
        }
    }

    private void Fire() {
        FlamethrowerProjectile projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        projectile.SetVelocity(transform.forward * projectileSpeed);

    }
    public void ToggleFire(bool fire) {
        firing = fire;
        if (firing) {
            fireTimer = 0f;
        }
    }
}
