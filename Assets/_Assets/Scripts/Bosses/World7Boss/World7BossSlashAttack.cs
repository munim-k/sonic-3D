using UnityEngine;

public class World7BossSlashAttack : MonoBehaviour
{
    [SerializeField] private float speed=1f;
    

    private void Update() {
      transform.position += transform.forward * speed * Time.deltaTime;
    }
}
