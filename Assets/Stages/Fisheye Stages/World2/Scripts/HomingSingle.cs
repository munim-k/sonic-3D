using UnityEngine;

public class HomingSingle : MonoBehaviour
{
    [HideInInspector] public bool shouldStart = false;

    private GameObject player;

    [SerializeField] private float speed = 5f;
    [HideInInspector] public float homingTime = 1.3f;

    [HideInInspector] public float timer = 0f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        //follow the player
        if (shouldStart)
        {
            timer += Time.deltaTime;
            if (timer < homingTime)
            {
                Vector3 direction = (player.transform.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
