using UnityEngine;

public class HeadFollow : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private float speed = 10f;

    private float timer = 0f;
    private float followTime = 10f;
    private float stayTime = 3f;

    private bool isFollowing = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (isFollowing && timer >= followTime)
        {
            timer = 0f;
            isFollowing = false;
            Debug.Log("Stop");
        }
        else if (!isFollowing && timer >= stayTime)
        {
            timer = 0f;
            isFollowing = true;
        }
        
        if (isFollowing)
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }
}
