using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class ProjectileAttack : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float followTime = 4f;
    [SerializeField] private FisheyeBoss5 boss;
    private GameObject player;
    private GameObject newProjectile;
    private float followTimer = 0f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        boss.OnProjectileAttack += Boss_OnProjectileAttack;
    }

    private void Boss_OnProjectileAttack()
    {
        if (newProjectile != null)
        {
            Destroy(newProjectile.gameObject);
            newProjectile = null;
        }
        newProjectile = Instantiate(projectilePrefab, projectilePrefab.transform);
        newProjectile.transform.parent = null;
        newProjectile.SetActive(true);
        followTimer = 0f;
    }

    private void Update()
    {
        if (newProjectile != null)
        {
            followTimer += Time.deltaTime;
            newProjectile.transform.position = Vector3.Lerp(newProjectile.transform.position, player.transform.position, projectileSpeed * Time.deltaTime);

            if (followTimer >= followTime)
            {
                Destroy(newProjectile.gameObject);
                newProjectile = null;
            }
        }
    }

    private void OnDestroy()
    {
        boss.OnProjectileAttack -= Boss_OnProjectileAttack;
    }
}
