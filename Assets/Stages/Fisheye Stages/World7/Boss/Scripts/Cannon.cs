using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private Transform bombSpawnPosition;
    [SerializeField] private GameObject bombProjectile;
    [SerializeField] private float forceOfThrow = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //spawn Bomb and shoot in the forward direction

            GameObject bomb = Instantiate(bombProjectile, bombSpawnPosition);
            bomb.transform.parent = null;

            bomb.GetComponent<Rigidbody>().AddForce(bomb.transform.forward * forceOfThrow, ForceMode.Impulse);
        }
    }
}
