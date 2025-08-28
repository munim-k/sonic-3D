using System.Collections.Generic;
using UnityEngine;

public class PlayerBombAttack : MonoBehaviour {
    [SerializeField] private int damage;
    [SerializeField] private float attackRange;
    [SerializeField] private float bombTime = 1f;
    [SerializeField] private LayerMask excludeLayers;
    [SerializeField] private GameObject bombParticles;
    private Vector3 hitNormal = Vector3.zero;

    private float bombTimer;
    private bool exploded = false;
    private List<IHittable> enemiesAttacked;
    public void Awake() {
        enemiesAttacked = new List<IHittable>();
        bombTimer = bombTime;
    }

    private void FixedUpdate() {
        if (bombTimer > 0) {
            bombTimer -= Time.fixedDeltaTime;
        }
        else if (!exploded) {
            Explode();
        }
    }

    public void OnCollisionEnter(Collision collision) {
        hitNormal = collision.GetContact(0).normal.normalized;
        Explode();
    }


    private void Explode() {
        exploded = true;
        //Check all layers except exclude layers
        LayerMask mask = ~excludeLayers;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, mask);
        Collider[] insideColliders = Physics.OverlapSphere(transform.position, 0.05f, mask);
        foreach (Collider hitCollider in hitColliders) {
            //Do a raycast from the bomb to the enemy to check if there are no obstacles
            Vector3 originPos = transform.position + hitNormal * 0.1f;
            Vector3 direction = hitCollider.transform.position - originPos;
            direction.Normalize();
            RaycastHit hit;
            bool skip = false;
            if (Physics.Raycast(originPos, direction, out hit, attackRange, mask, QueryTriggerInteraction.Ignore)) {
                //If the raycast hit the enemy, we can damage it
                if (hit.collider != hitCollider) {
                    skip = true;
                }
            }
            else {
                skip = true;
            }
            if (skip) {
                //But if the enemy is very close to the bomb, raycast may have started inside them, we can still damage it
                foreach (Collider insideCollider in insideColliders) {
                    if (insideCollider == hitCollider) {
                        skip = false;
                        break;
                    }
                }
            }
            if (skip) {
                continue;
            }
            IHittable enemy = hitCollider.GetComponent<IHittable>();
            if (enemy != null && !enemiesAttacked.Contains(enemy)) {
                enemy.DoHit(damage);
                enemiesAttacked.Add(enemy);
            }
        }
        Instantiate(bombParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}


