using UnityEngine;

public class GunLaserEnemy : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private float range = 20f;
    [SerializeField] private GameObject laserObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other.gameObject;

            laserObject.transform.localScale = new Vector3(laserObject.transform.localScale.x, laserObject.transform.localScale.y, range);
            laserObject.transform.localPosition = new Vector3(laserObject.transform.localPosition.x, laserObject.transform.localPosition.y, range / 2);
        }
    }

    private void Update()
    {
        if (player != null)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3f * Time.deltaTime);
        }
    }
}
