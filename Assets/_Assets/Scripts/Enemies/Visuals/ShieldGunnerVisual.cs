using UnityEngine;

public class ShieldGunnerVisual : MonoBehaviour
{
    [SerializeField] private ShieldGunnerEnemy shieldGunner;
    [SerializeField] private Transform shieldGunnerGunTransform;
    [SerializeField] private Transform meshGunTransform;
    [SerializeField] private Animator muzzleAnimator;
    private bool rotateGun;
    void Start()
    {
        shieldGunner.OnStateChange += UpdateVisuals;
        shieldGunner.FiringStateChange += UpdateFiring;
        muzzleAnimator.speed = 0f;
    }

    private void UpdateFiring(bool firing) {
        if(firing) {
            muzzleAnimator.speed = 1f;
        } else {
            muzzleAnimator.speed = 0f;
        }
    }

    private void UpdateVisuals(ShieldGunnerEnemy.State state)
    {
        switch (state)
        {
            case ShieldGunnerEnemy.State.Idle:
                rotateGun = false;
                break;
            case ShieldGunnerEnemy.State.Shooting:
                rotateGun = true;
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rotateGun) {
            meshGunTransform.rotation = shieldGunnerGunTransform.rotation;
           
        }
    }
}
