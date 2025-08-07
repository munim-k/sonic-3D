using System;
using Unity.VisualScripting;
using UnityEngine;

public class BeeEnemy : BaseEnemy,IHittable
{

    public enum State {
        Idle,   //Bee is idly hovering
        Looking, //Bee is rotating to look at player
        Charging //Bee is charging towards player
    }

    private State state;
    [SerializeField] private int maxHealth = 20;
    private int currentHealth;

    //Looking state
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float lookDuration = 2f;
    private float lookTimer = 0f;

    //Charging State
    [SerializeField] private float chargingSpeed = 5f;
    [SerializeField] private float detonationRange = 2f;
    private Vector3 chargePoint = Vector3.zero;

    private Action On_Death;
    private Action On_Hit;
    public Action OnDeath { get => On_Death; set => On_Death=value; }
    public Action OnHit { get => On_Hit; set => On_Hit=value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    float BaseEnemy.GetHealthNormalized() {
        throw new NotImplementedException();
    }

    void IHittable.DoHit(int damage) {
        throw new NotImplementedException();
    }

    HittableType IHittable.GetType() {
        throw new NotImplementedException();
    }
}
