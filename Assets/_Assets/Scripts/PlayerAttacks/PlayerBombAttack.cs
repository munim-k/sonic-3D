using System.Collections.Generic;
using UnityEngine;

public class PlayerBombAttack : MonoBehaviour {
    [SerializeField] private int damage;
    [SerializeField] private float attackRange;
    [SerializeField] private float bombTime = 1f;
    [SerializeField] private LayerMask overlapSphereMask;
    [SerializeField] private LayerMask raycastMask;
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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, ~overlapSphereMask);
        Collider[] insideColliders = Physics.OverlapSphere(transform.position, 0.05f, ~overlapSphereMask);
        foreach (Collider hitCollider in hitColliders) {
            //Do a raycast from the bomb to the enemy to check if there are no obstacles
            Vector3 originPos = transform.position + hitNormal * 0.1f;
            Vector3 direction = hitCollider.ClosestPoint(originPos) - originPos;
            float distance = direction.magnitude;
            direction.Normalize();
            RaycastHit hit;
            //If something other than the layers excluded is blocking raycast, skip
            if (Physics.Raycast(originPos, direction, out hit, distance, ~raycastMask, QueryTriggerInteraction.Ignore)) {
                if (hit.collider != hitCollider) {
                    print($"Bomb raycast blocked by {hit.collider.name}");
                    continue;
                }
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


