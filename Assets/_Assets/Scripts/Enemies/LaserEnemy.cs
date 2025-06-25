using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LaserEnemy : MonoBehaviour
{
    [SerializeField] private float rotataionSpeed = 10f;
    [SerializeField] private float distance = 2f;
    [SerializeField] private float laserDuration = 2f;
    [SerializeField] private float waitTimeBetweenLasers = 1f;
    [SerializeField] private float initialDelay = 1f;
    [SerializeField] private GameObject laserPrefab;

    private bool isShooting = false;
    private float laserTimer = 0f;

    private void Start()
    {
        laserPrefab.transform.localScale = new Vector3(1, 1, distance);
        laserPrefab.transform.localPosition = new Vector3(0, 0, distance / 2 - 0.5f);
        laserPrefab.SetActive(false);
        StartCoroutine(InitialWait());
    }

    private IEnumerator InitialWait()
    {
        yield return new WaitForSeconds(initialDelay);
        laserPrefab.SetActive(true);
        isShooting = true;
    }

    private IEnumerator DelayBetweenShots()
    {
        laserPrefab.SetActive(false);
        yield return new WaitForSeconds(waitTimeBetweenLasers);
        laserPrefab.SetActive(true);
        isShooting = true;
    }

    private void Update()
    {
        if (isShooting)
        {
            laserTimer += Time.deltaTime;
            transform.Rotate(Vector3.up, rotataionSpeed * Time.deltaTime);
            if (laserTimer >= laserDuration)
            {
                laserTimer = 0f;
                isShooting = false;
                StartCoroutine(DelayBetweenShots());
            }
        }
    }
}
 