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
        [SerializeField] private Vector3 offsetVector;
        private List<Vector3> points;// List to store points for projectile motion
        bool aiming;
        public override bool Evaluate()
        {
            aiming = inputHandler.roll.hold;
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
            if(points == null)
            {
                points = new List<Vector3>();
            }
            points.Clear();
            // Calculate the vertical angle between the camera and the player
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 playerForward = modelTransform.forward;
            float verticalAngle = Vector3.SignedAngle(Vector3.ProjectOnPlane(cameraForward, modelTransform.right), playerForward, modelTransform.right);
            //Invert the angle to make it positive
            verticalAngle = -verticalAngle;
            // Adjust the throw direction based on the vertical angle
            Vector3 adjustedThrowDirection = Quaternion.AngleAxis(verticalAngle, modelTransform.right) * modelTransform.forward;

            Vector3 newPosition = modelTransform.position + (modelTransform.rotation * offsetVector);

            //Do raycasts for projectile motion in the forward direction
            for (int i = 0; i < maxPoints; i++) {
                //Calculate the time of flight for each point
                float t = i * Time.fixedDeltaTime*scale;
                //Calculate the position of the projectile at time t
                Vector3 point = newPosition +
                      (adjustedThrowDirection * throwForce * t) +
                      (0.5f * Physics.gravity * t * t);
                points.Add(point);
            }
            //Do a raycast forward from each point to the next point
            for (int i = 0; i < points.Count - 1; i++)
            {
                RaycastHit hit;
                if (Physics.Raycast(points[i], points[i + 1] - points[i], out hit, Vector3.Distance(points[i], points[i + 1])))
                {
                   
                    //Removeall points after the hit point
                    points.RemoveRange(i + 1, points.Count - (i + 1));
                    points.Add(hit.point);
                    break;
                }
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



        


    }
}
