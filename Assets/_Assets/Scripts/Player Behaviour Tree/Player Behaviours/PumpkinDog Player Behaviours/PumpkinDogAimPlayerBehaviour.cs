using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollEngine
{
    public class PumpkinDogAimPlayerBehaviour : PlayerBehaviour
    {
        [SerializeField] private float throwForce; // Force applied to the projectile
        [SerializeField] private int maxPoints; // Maximum number of points for projectile motion
        [SerializeField] private float scale;
        [SerializeField] private float angleMultipler;
        [SerializeField] private float angleOffset;
        [SerializeField] private Vector3 offsetVector;
        [SerializeField] private float cooldown;
        [SerializeField] private float projectileSpeed=1f;
        [SerializeField] private Transform Projectile; // Prefab of the projectile to be fired

        [SerializeField] private Transform playerBombUI;

        private float cooldownTimer = 0;
        private List<Vector3> points;// List to store points for projectile motion
        private Vector3 launchOrigin;
        private Vector3 launchVector;

        public Action OnFire;
        bool aiming;

        private void Start()
        {
            Transform p = Instantiate(playerBombUI,character.canvas.transform);
            character.uis.Add(p.gameObject);
            p.GetComponent<PlayerBombUI>().Initialize(this);

        }
        public override bool Evaluate()
        {
            aiming = inputHandler.aim.hold;
            active = aiming;

            if(wasActive && !active)
            {
                //Reset the move velocity to the models forward direction
                moveVelocity = Vector3.ProjectOnPlane(modelTransform.forward, plane) ;
            }
            return aiming;
        }

        public override void Execute()
        {

            CalculatePath();
            //If user has left clicked then fire and start cooldown
            if (inputHandler.fire.pressed && inputHandler.aim.hold && cooldownTimer <= 0)
            {
                //Fire the projectile
                FireProjectile();
                cooldownTimer = cooldown;
            }
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
            }

        }


        private void FireProjectile()
        {
            //Instantiate projectile and set the starting velocity
            OnFire?.Invoke();
            GameObject projectile = Instantiate(Projectile.gameObject, launchOrigin, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            projectile.GetComponent<SetProjectileGravity>().SetGravityScale(projectileSpeed * projectileSpeed);
            if (rb != null)
            {
                rb.linearVelocity = launchVector;
                rb.angularVelocity = launchVector;
            }
        }

        private void CalculatePath()
        {

            // Calculate the vertical angle between the camera and the player
            SetLaunchVector();
            SetPoints();
            //Do a raycast forward from each point to the next point
            for (int i = 0; i < points.Count - 1; i++)
            {
                RaycastHit hit;
                if (Physics.Raycast(points[i], points[i + 1] - points[i], out hit, Vector3.Distance(points[i], points[i + 1])))
                {
                    points.RemoveRange(i + 1, points.Count - (i + 1));
                    //Remove all points after the hit point
                    points.Add(hit.point);
                    break;
                }
            }
        }

        private void SetLaunchVector()
        {
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 playerForward = modelTransform.forward;
            float verticalAngle = Vector3.SignedAngle(Vector3.ProjectOnPlane(cameraForward, modelTransform.right), playerForward, modelTransform.right);
            //Invert the angle to make it positive
            verticalAngle = -verticalAngle;
            verticalAngle *= angleMultipler;
            verticalAngle += angleOffset;
            // Adjust the throw direction based on the vertical angle
            Vector3 adjustedThrowDirection = Quaternion.AngleAxis(verticalAngle, modelTransform.right) * modelTransform.forward;

            launchOrigin = modelTransform.position + (modelTransform.rotation * offsetVector);
            launchVector = adjustedThrowDirection * throwForce * projectileSpeed;
        }

        private void SetPoints()
        {
            if (points == null)
            {
                points = new List<Vector3>();
            }
            points.Clear();
            float mass = Projectile.GetComponent<Rigidbody>().mass;
            Vector3 gravity = Physics.gravity * projectileSpeed * projectileSpeed;

            for (int i = 0; i < maxPoints; i++)
            {
                float t = i * scale;
               Vector3 position = launchOrigin + (launchVector * t) + (gravity * t * t / 2);
                points.Add(position);
            }
        }
        public List<Vector3> getPoints()
        {
            return points;
        }

        public Vector3 getHitPoint()
        {
            if (points.Count > 0)
            {
                return points[points.Count - 1];
            }
            else
            {
                return Vector3.zero;
            }
        }

        public float GetCooldownNormalized()
        {
            return cooldownTimer/cooldown;
        }

        


    }
}
