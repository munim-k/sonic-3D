using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;

public class PlayerBombAttack : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float attackRange;
    [SerializeField] private float bombTime=1f;
    [SerializeField] private LayerMask excludeLayers;
    [SerializeField] private GameObject bombParticles;

    private float bombTimer;
    private List<BaseEnemy> enemiesAttacked;
    public void Start()
    {
        enemiesAttacked = new List<BaseEnemy>();
        bombTimer = bombTime;
    }

    private void Update()
    {
        if(bombTimer > 0)
        {
            bombTimer -= Time.deltaTime;
        }
        else
        {
            Explode();
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Explode();
    }


    private void Explode()
    {
        //TODO: Add explosion effect
        //Check all layers except exclude layers
        LayerMask mask = ~excludeLayers;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange,mask);
        foreach (Collider hitCollider in hitColliders)
        {
            BaseEnemy enemy = hitCollider.GetComponent<BaseEnemy>();
            if (enemy != null && !enemiesAttacked.Contains(enemy))
            {
                enemy.DoDamageToEnemy(damage);
                enemiesAttacked.Add(enemy);
            }
        }
        Instantiate(bombParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}


