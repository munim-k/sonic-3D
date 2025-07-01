using System;
using Unity.VisualScripting;
using UnityEngine;

public class World6Boss : MonoBehaviour, BaseEnemy {
    [SerializeField] private Transform levelExit;
    [SerializeField] private int maxHealth = 100;
    private int health = 100;
    //Spinning
    [SerializeField] private float attackCooldown = 2f;
    private bool isThrowing = false;
    [SerializeField] private Transform bombPrefab;
    [SerializeField] private float bombLaunchForce = 10f;
    [SerializeField] private Transform[] bombLaunchPoints;
    [SerializeField] private float bombStep = 0.25f;
    private float bombTimer = 0f;


    //Jumping
    [SerializeField] private Transform jumpShockwave;

    //Moving
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private Transform fireworkPrefab;
    [SerializeField] private Transform fireworkLaunchPoint;
    [SerializeField] private float fireworkStep = 0.5f;
    private float fireworkTimer = 0f;


    private float cooldownTimer = 0f;

    public enum State {
        Spinning,// Spin and throw bombs
        Jumping, //Jump, slam then spawn shockwave
        Moving, //Shoot fireworks during movement state
        Dead
    }
    private State state;

    private Action onDeath;
    private Action onHit;

    public Action<State> OnStateChange;
    Action BaseEnemy.OnDeath { get => onDeath; set => onDeath = value; }
    Action IHittable.OnHit { get => onHit; set => onHit = value; }

    void Start() {
        health = maxHealth;
        state = State.Moving;
        OnStateChange?.Invoke(state);
    }

    // Update is called once per frame
    void FixedUpdate() {
        switch (state) {
            case State.Spinning:
                Spinning();
                break;
            case State.Moving:
                Moving();
                break;
            default:
                break;
        }
    }

    private void Spinning() {
        if (bombTimer >= 0) {
            bombTimer -= Time.fixedDeltaTime;
        }
        else {
            bombTimer = bombStep;
            if (isThrowing) {
                for (int i = 0; i < bombLaunchPoints.Length; i++) {
                    Transform bomb = Instantiate(bombPrefab, bombLaunchPoints[i].position, Quaternion.identity);
                    Vector3 direction = (bombLaunchPoints[i].position - transform.position).normalized;
                    bomb.GetComponent<Rigidbody>().AddForce(direction * bombLaunchForce, ForceMode.Impulse);
                }

            }
        }
    }
    private void Moving() {
        if (cooldownTimer >= 0f) {
            cooldownTimer -= Time.fixedDeltaTime;
        }
        if (fireworkTimer >= 0f) {
            fireworkTimer -= Time.fixedDeltaTime;
        }
        else {
            fireworkTimer = fireworkStep;
            Instantiate(fireworkPrefab, fireworkLaunchPoint.position, Quaternion.identity);
        }
        if (cooldownTimer < 0f) {
            if (UnityEngine.Random.Range(0f, 1f) < 0.5f) {
                state = State.Spinning;
            }
            else {
                state = State.Jumping;
            }
            OnStateChange?.Invoke(state);
        }
    }


    public void SetBombThrowing(bool isThrowing) {
        if (state == State.Spinning) {
            this.isThrowing = isThrowing;
        }
    }

    public void SpawnJumpShockwave() {
        if (state == State.Jumping) {
            // Logic to spawn a shockwave
            Instantiate(jumpShockwave, transform.position, Quaternion.identity);
        }
    }
    public void TransitionToMoving(State incoming) {
        cooldownTimer = attackCooldown;
        state = State.Moving;
        OnStateChange?.Invoke(state);
    }

    float BaseEnemy.GetHealthNormalized() {
        return (float)health / maxHealth;
    }
    void IHittable.DoHit(int damage) {
        health -= damage;
        onHit?.Invoke();
        if (health <= 0) {
            onDeath?.Invoke();
            Destroy(gameObject);
            state = State.Dead;
            levelExit.gameObject.SetActive(true);
            OnStateChange?.Invoke(state);
        }
    }

    HittableType IHittable.GetType() {
        return HittableType.Enemy;
    }
}

