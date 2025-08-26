using UnityEngine;
using UnityEngine.Splines;

public class SplineVectorTest : MonoBehaviour
{
    [SerializeField] private SplineContainer mainSplineComponent;
    [SerializeField] private SplineContainer profileSplineComponent;
    [SerializeField] private float t1 = 0;
    [SerializeField] private float t2 = 0;
    [SerializeField] private float profileScale = 1f;

    Vector3 mainPos;
    Vector3 worldPos;
    void Update()
    {
        //worldOffset = R_main * p_profile.x + U_main * p_profile.y(+T_main * p_profile.z if your profile has z)
        //worldPos = P_main + worldOffset
        Vector3 main_Position = SplineUtility.EvaluatePosition(mainSplineComponent.Spline, t1);
        main_Position = mainSplineComponent.transform.TransformPoint(main_Position);
        mainPos= main_Position;
        Vector3 main_Tangent = SplineUtility.EvaluateTangent(mainSplineComponent.Spline, t1);
        main_Tangent.Normalize();
        Vector3 main_Up = SplineUtility.EvaluateUpVector(mainSplineComponent.Spline, t1);
        main_Up.Normalize();
        Vector3 main_Right = Vector3.Cross(main_Tangent, main_Up).normalized;
        main_Right.Normalize();
        Vector3 profile_Position = SplineUtility.EvaluatePosition(profileSplineComponent.Spline, t2);
        //Multiply profile_Position by mainSplineComponent's scale to account for scaling
        profile_Position = profile_Position * profileScale;
        Vector3 world_Offset = main_Right * profile_Position.x + main_Up * profile_Position.z;
        Vector3 world_Position = main_Position + world_Offset;
        worldPos= world_Position;
        Debug.DrawLine(main_Position, world_Position, Color.red);
        Debug.DrawLine(main_Position, main_Position + main_Tangent, Color.blue);
        Debug.DrawLine(main_Position, main_Position + main_Up, Color.green);
        Debug.DrawLine(main_Position, main_Position + main_Right, Color.yellow);

    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(mainPos, 1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(worldPos, 1f);
    }

}
