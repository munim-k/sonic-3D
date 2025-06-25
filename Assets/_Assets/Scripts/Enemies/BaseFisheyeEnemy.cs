using UnityEngine;

public class BaseFisheyeEnemy : MonoBehaviour
{
    [SerializeField] private GameObject parentObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            parentObject.SetActive(false);
        }
    }
}
