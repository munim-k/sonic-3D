using UnityEngine;

public class ChangeWindDirection : MonoBehaviour
{
    private enum WindDirection
    {
        Left,
        Right,
        None
    }

    private WindDirection windDirection = WindDirection.None;

    private GameObject player;
    private Rigidbody playerRigidbody;
    private ParticleSystem windEffect;

    [SerializeField] private float windDurationRight = 5f;
    [SerializeField] private float windDurationLeft = 5f;
    [SerializeField] private float windDurationNone = 5f;

    [SerializeField] private float windForce = 20f;

    private float windTimer = 0f;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object not found in the scene.");
        }
        playerRigidbody = player.GetComponent<Rigidbody>();
        if (playerRigidbody == null)
        {
            Debug.LogError("Player does not have a Rigidbody component.");
        }
        windEffect = GetComponent<ParticleSystem>();
        if (windEffect == null)
        {
            Debug.LogError("Wind effect object does not have a ParticleSystem component.");
        }
    }

    private void Update()
    {
        transform.position = player.transform.position;
        if (windDirection == WindDirection.None)
        {
            windTimer += Time.deltaTime;
            if (windTimer >= windDurationNone)
            {
                ChangeWindDirectionRandomly();
                windTimer = 0f;
            }
        }
        else if (windDirection == WindDirection.Right)
        {
            windTimer += Time.deltaTime;
            if (windTimer >= windDurationRight)
            {
                ChangeWindDirectionRandomly();
                windTimer = 0f;
            }
        }
        else if (windDirection == WindDirection.Left)
        {
            windTimer += Time.deltaTime;
            if (windTimer >= windDurationLeft)
            {
                ChangeWindDirectionRandomly();
                windTimer = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (windDirection == WindDirection.Right)
        {
            playerRigidbody.AddForce(Vector3.right * windForce, ForceMode.Force);
        }
        else if (windDirection == WindDirection.Left)
        {
            playerRigidbody.AddForce(Vector3.left * windForce, ForceMode.Force);
        }
    }
    
    private void ChangeWindDirectionRandomly()
    {
        int randomValue = Random.Range(0, 3);

        var velocity = windEffect.velocityOverLifetime;
        switch (randomValue)
        {
            case 0:
                windDirection = WindDirection.Left;
                velocity.x = -20f;
                break;
            case 1:
                windDirection = WindDirection.Right;
                velocity.x = 20f;
                break;
            case 2:
                windDirection = WindDirection.None;
                velocity.x = 0f;
                break;
        }
    }
}
