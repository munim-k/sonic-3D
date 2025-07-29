using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FisheyeWorld8Boss : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject nukeProjectile;
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private Transform missilePosition1;
    [SerializeField] private Transform missilePosition2;
    [SerializeField] private Transform runwayPosition;
    [SerializeField] private GameObject phase2Objects;
    [SerializeField] private GameObject levelTransition;

    [Header("Movement Settings")]
    [SerializeField] private float flySpeed = 15f;
    [SerializeField] private float takeoffSpeed = 10f;
    [SerializeField] private float altitude = 20f;
    [SerializeField] private float bombDropInterval = 0.5f;
    [SerializeField] private float restDuration = 5f;
    [SerializeField] private float turnDuration = 2f;
    [SerializeField] private float landingApproachDistance = 50f;
    [SerializeField] private float missileLaunchInterval = 5f;
    [SerializeField] private int maxBombRuns = 3;

    [Header("UI and Health")]
    [SerializeField] private Image healthBar;
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth = 100f;
    private bool isInSecondPhase = false;
    private GameObject player;
    private float nextBombTime;
    private float nextMissileTime;
    private int bombRunsCompleted;
    private float restTimer;
    private Vector3 landingApproachPoint;

    private enum State { TakingOff, Approaching, Turning, Returning, Landing, Resting }
    private State state = State.TakingOff;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        transform.position = runwayPosition.position;
        transform.rotation = runwayPosition.rotation;
        nextBombTime = Time.time + bombDropInterval;
        nextMissileTime = Time.time + missileLaunchInterval;
    }

    void Update()
    {
        switch (state)
        {
            case State.TakingOff:
                HandleTakeoff();
                break;
            case State.Approaching:
                HandleApproach();
                break;
            case State.Turning:
                HandleTurning();
                break;
            case State.Returning:
                HandleReturn();
                break;
            case State.Landing:
                HandleLanding();
                break;
            case State.Resting:
                HandleResting();
                break;
        }
    }

    void HandleTakeoff()
    {
        Vector3 takeoffDirection = Vector3.Lerp(transform.forward, Vector3.up, 0.1f).normalized;
        transform.position += takeoffDirection * takeoffSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(takeoffDirection), Time.deltaTime * 2f);

        if (transform.position.y >= altitude)
        {
            state = State.Approaching;
        }
    }

    void HandleApproach()
    {
        // Fly toward player's XZ position at fixed altitude
        Vector3 targetPos = new Vector3(player.transform.position.x, altitude, player.transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, flySpeed * Time.deltaTime);

        // Face movement direction
        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 3f);
        }

        // Continuous bomb dropping
        if (Time.time >= nextBombTime)
        {
            DropBomb();
            nextBombTime = Time.time + bombDropInterval;
        }

        // Missile launching
        if (Time.time >= nextMissileTime)
        {
            LaunchMissiles();
            nextMissileTime = Time.time + missileLaunchInterval;
        }

        // When close to player position, start turning
        if (Vector3.Distance(transform.position, targetPos) < 5f)
        {
            bombRunsCompleted++;
            if (bombRunsCompleted >= maxBombRuns)
            {
                state = State.Returning;
            }
            else
            {
                state = State.Turning;
                StartCoroutine(CompleteTurn());
            }
        }
    }

    IEnumerator CompleteTurn()
    {
        float elapsed = 0f;
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(-transform.forward);

        while (elapsed < turnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / turnDuration;
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            transform.position += transform.forward * flySpeed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, altitude, transform.position.z);
            yield return null;
        }

        state = State.Approaching;
    }

    void HandleTurning()
    {
        // Handled in coroutine
    }

    void HandleReturn()
    {
        landingApproachPoint = runwayPosition.position - runwayPosition.forward * landingApproachDistance;
        landingApproachPoint.y = altitude;

        transform.position = Vector3.MoveTowards(transform.position, landingApproachPoint, flySpeed * Time.deltaTime);

        Vector3 direction = (landingApproachPoint - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 3f);
        }

        if (Vector3.Distance(transform.position, landingApproachPoint) < 1f)
        {
            state = State.Landing;
        }
    }

    void HandleLanding()
    {
        Vector3 targetPos = runwayPosition.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, flySpeed * Time.deltaTime);

        Vector3 landingDirection = (runwayPosition.position - transform.position).normalized;
        if (landingDirection != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(landingDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }

        if (Vector3.Distance(transform.position, runwayPosition.position) < 0.5f)
        {
            transform.position = runwayPosition.position;
            transform.rotation = runwayPosition.rotation;
            restTimer = restDuration;
            state = State.Resting;
        }
    }

    void HandleResting()
    {
        restTimer -= Time.deltaTime;
        if (restTimer <= 0f)
        {
            bombRunsCompleted = 0;
            state = State.TakingOff;
        }
    }

    void DropBomb()
    {
        Instantiate(isInSecondPhase ? nukeProjectile : bombPrefab, transform.position + Vector3.right * 2f + Vector3.down * 2f, Quaternion.identity);
        Instantiate(isInSecondPhase ? nukeProjectile : bombPrefab, transform.position - Vector3.right * 2f + Vector3.down * 2f, Quaternion.identity);
    }

    void LaunchMissiles()
    {
        Instantiate(missilePrefab, missilePosition1.position, missilePosition1.rotation);
        Instantiate(missilePrefab, missilePosition2.position, missilePosition2.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            currentHealth -= 5f;
            healthBar.fillAmount = currentHealth / maxHealth;

            if (currentHealth <= maxHealth / 2)
            {
                isInSecondPhase = true;
                phase2Objects.SetActive(true);
            }

            if (currentHealth <= 0f)
            {
                levelTransition.SetActive(true);
                Destroy(gameObject);
            }
        }
    }
}