using System.Collections.Generic;
using UnityEngine;

public class PlayerBombAttack : MonoBehaviour {
    [SerializeField] private int damage;
    [SerializeField] private float attackRange;
    [SerializeField] private float bombTime = 1f;
    [SerializeField] private LayerMask excludeLayers;
    [SerializeField] private GameObject bombParticles;
    private Vector3 hitNormal= Vector3.zero;

    private float bombTimer;
    private bool explode = false;
    private List<IHittable> enemiesAttacked;
    public void Start() {
        enemiesAttacked = new List<IHittable>();
        bombTimer = bombTime;
    }

    private void FixedUpdate() {
        if (bombTimer > 0 && !explode) {
            bombTimer -= Time.fixedDeltaTime;
        }
        else {
            Explode();
        }
    }

    public void OnCollisionEnter(Collision collision) {
        explode = true;
        hitNormal = collision.GetContact(0).normal.normalized;
    }


    private void Explode() {
        //Check all layers except exclude layers
        LayerMask mask = ~excludeLayers;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, mask);
        foreach (Collider hitCollider in hitColliders) {
            //Do a raycast from the bomb to the enemy to check if there are no obstacles
            Vector3 direction = hitCollider.transform.position - transform.position;
            RaycastHit hit;
            if (Physics.Raycast(transform.position + hitNormal* 0.1f , direction.normalized, out hit, attackRange, mask, QueryTriggerInteraction.Ignore)) {
                //If the raycast hit the enemy, we can damage it
                if (hit.collider != hitCollider) {
                    continue; // If the raycast hit something else, skip this collider
                }
            } else {
                continue; // If the raycast didn't hit anything, skip this collider
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


