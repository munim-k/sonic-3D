using UnityEngine;

public class JumpedOn : MonoBehaviour
{
    [SerializeField] private GameObject parent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(parent);
        }
    }
}
