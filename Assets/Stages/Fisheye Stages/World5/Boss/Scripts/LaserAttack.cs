using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class LaserAttack : MonoBehaviour
{
    [SerializeField] private GameObject laserObject;
    [SerializeField] private float followTime = 2f;
    [SerializeField] private FisheyeBoss5 boss;
    [SerializeField] private float followSpeed = 5f;
    private GameObject player;
    private float followTimer = 0f;
    private bool isFollowing = false;

    private void Start()
    {
        laserObject.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");
        boss.OnLaserAttack += Boss_OnLaserAttack;
    }

    private void OnDestroy()
    {
        boss.OnLaserAttack -= Boss_OnLaserAttack;
    }

    private void Boss_OnLaserAttack()
    {
        laserObject.SetActive(true);
        isFollowing = true;
        followTimer = 0f;
    }

    private void Update()
    {
        if (isFollowing)
        {
            //make the laserObject Rotation to the player
            Vector3 direction = player.transform.position - laserObject.transform.position;
            Quaternion tragetRotation = Quaternion.LookRotation(direction);
            laserObject.transform.rotation = Quaternion.Slerp(laserObject.transform.rotation, tragetRotation, followSpeed * Time.deltaTime);
            
            followTimer += Time.deltaTime;
            if (followTimer >= followTime)
            {
                isFollowing = false;
                laserObject.SetActive(false);
            }
        }
    }
}
