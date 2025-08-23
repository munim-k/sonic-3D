using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldGunnerVisual : MonoBehaviour {
    [SerializeField] private ShieldGunnerEnemy shieldGunner;
    [SerializeField] private Transform shieldGunnerGunTransform;
    [SerializeField] private Transform meshGunTransform;
    [SerializeField] private Animator muzzleAnimator;
    [SerializeField] private ParticleSystem bulletTrailsPSPrefab;
    private ParticleSystem bulletTrailsPS;
    List<Vector4> customData = new List<Vector4>();
    uint currentParticleIndex = 1;
    private bool rotateGun;
    void Start() {
        shieldGunner.OnStateChange += UpdateVisuals;
        shieldGunner.FiringStateChange += UpdateFiring;
        shieldGunner.OnShotFired += FireShot;
        muzzleAnimator.speed = 0f;
        //Instantiate a new empty gameoject to hold the renderers in world space
        bulletTrailsPS = Instantiate(bulletTrailsPSPrefab, Vector3.zero, Quaternion.identity);
        bulletTrailsPS.transform.parent = null;
        bulletTrailsPS.name = "BulletTrailsPool";

    }


    private void FireShot(Vector3 shotOrigin, Vector3 shotDir) {
        bulletTrailsPS.transform.position = Vector3.zero; //Reset position to avoid weirdness
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        //Spawn a stationary stretched plane particle at shotOrigin/shotEnd, rotated to shotDir
        emitParams.position = shotOrigin;
        bulletTrailsPS.Emit(emitParams, 1);
        int count = bulletTrailsPS.particleCount;
        bulletTrailsPS.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
        if (customData.Count < count) {
            // Ensure the list matches particle count
            while (customData.Count < count) customData.Add(Vector4.zero);
        }
        for (int i = 0; i < count; i++) {
            if (customData[i].x == 0) {
                //This particle was just added, set its custom data
                customData[i] = new Vector4(currentParticleIndex++, 0f, 0f, 0f);
                if (currentParticleIndex >= bulletTrailsPS.main.maxParticles) {
                    currentParticleIndex = 1; //Wrap around
                }
            }

        }
        bulletTrailsPS.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
        StartCoroutine(MoveParticle(shotOrigin + shotDir, currentParticleIndex - 1));


    }

    private IEnumerator MoveParticle(Vector3 movePosition, uint particleIndex) {
        yield return null;
        //Wait one frame to ensure the particle is spawned
        int count = bulletTrailsPS.particleCount;
        var particles = new ParticleSystem.Particle[count];
        bulletTrailsPS.GetParticles(particles);
        bulletTrailsPS.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);

        for (int i = 0; i < count; i++) {
            //If difference between floats is less than threshold, we found the particle
            if (Mathf.Abs(customData[i].x - particleIndex) < 0.1f) {
                //This is the particle we want to move
                particles[i].position = movePosition;
                break;
            }
        }
        bulletTrailsPS.SetParticles(particles, count);
        bulletTrailsPS.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);

    }


    private void UpdateFiring(bool firing) {
        if (firing) {
            muzzleAnimator.speed = 1f;
        }
        else {
            muzzleAnimator.speed = 0f;
        }
    }

    private void UpdateVisuals(ShieldGunnerEnemy.State state) {
        switch (state) {
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
    void FixedUpdate() {
        if (rotateGun) {
            meshGunTransform.rotation = shieldGunnerGunTransform.rotation;

        }
    }
}
