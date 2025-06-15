using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class SplineObjectPlacer : MonoBehaviour {
    [Header("Placement Settings")]
    public GameObject objectToPlace;
    public int numberOfObjects = 10;
    public bool alignRotation = true;
    public bool verticalOffsetFollowsCurve = false;
    public float verticalOffset = 0f;

    [Header("Placement Range (Normalized 0-1)")]
    [Range(0f, 1f)]
    public float placementStart = 0f;
    [Range(0f, 1f)]
    public float placementEnd = 1f;

    [Header("Spline Options")]
    public bool useClosedSpline = false;

    private SplineContainer splineContainer;
    private Spline spline;

    void Start() {
        splineContainer = GetComponent<SplineContainer>();
        if (splineContainer == null || splineContainer.Spline == null) {
            Debug.LogError("SplineContainer or Spline not found!");
            return;
        }

        spline = splineContainer.Spline;
        PlaceObjects();
    }

    void PlaceObjects() {
        // Validate input
        if (objectToPlace == null) {
            Debug.LogError("No object to place assigned!");
            return;
        }

        if (numberOfObjects < 1) {
            Debug.LogWarning("Number of objects must be at least 1");
            return;
        }

        // Calculate total spline length
        float splineLength = SplineUtility.CalculateLength(spline, transform.localToWorldMatrix);
        // Handle zero-length spline case
        if (Mathf.Approximately(splineLength, 0f)) {
            Debug.LogWarning("Spline has zero length. Placing single object at start position.");
            PlaceSingleObject();
            return;
        }

        // Calculate placement parameters
        int placementCount = numberOfObjects;
        float tStart = Mathf.Clamp01(Mathf.Min(placementStart, placementEnd));
        float tEnd = Mathf.Clamp01(Mathf.Max(placementStart, placementEnd));

        //float stepSize = splineLength / (useClosedSpline ? numberOfObjects : numberOfObjects - 1);

        //// Place objects along spline
        //for (int i = 0; i < placementCount; i++) {
        //    float distance = i * stepSize;
        //    PlaceObjectAtDistance(distance, splineLength);
        //}
        for (int i = 0; i < placementCount; i++) {
            float t;
            if (useClosedSpline) {
                t = (float)i / placementCount;
            }
            else {
                if (placementCount == 1) {
                    t = tStart;
                }
                else {
                    t = Mathf.Lerp(tStart, tEnd, (float)i / (placementCount - 1));
                }
            }
            PlaceObjectAtNormalizedT(t);
        }
    }
    void PlaceObjectAtNormalizedT(float t) {
        Vector3 localposition = SplineUtility.EvaluatePosition(spline, t);
        if (verticalOffsetFollowsCurve) {
            Vector3 up = SplineUtility.EvaluateUpVector(spline, t);
            localposition += up.normalized * verticalOffset;
        }
        else {
            localposition.y += verticalOffset;
        }
        Vector3 position = splineContainer.transform.TransformPoint(localposition);
        Quaternion rotation = alignRotation
            ? Quaternion.LookRotation(SplineUtility.EvaluateTangent(spline, t))
            : Quaternion.identity;
        InstantiateObject(position, rotation);
    }

    void PlaceSingleObject() {
        Vector3 position = SplineUtility.EvaluatePosition(spline, 0f);
        Quaternion rotation = alignRotation
            ? Quaternion.LookRotation(SplineUtility.EvaluateTangent(spline, 0f))
            : Quaternion.identity;
        InstantiateObject(position, rotation);
    }

    void PlaceObjectAtDistance(float distance, float splineLength) {
        float t = SplineUtility.GetNormalizedInterpolation(spline, distance, PathIndexUnit.Distance);

        Vector3 localposition = SplineUtility.EvaluatePosition(spline, t);
        if (verticalOffsetFollowsCurve) {
            // Adjust vertical offset based on spline tangent
            Vector3 up = SplineUtility.EvaluateUpVector(spline, t);
            localposition += up.normalized * verticalOffset;
        }
        else {
            // Apply fixed vertical offset
            localposition.y += verticalOffset;
        }
        Vector3 position = splineContainer.transform.TransformPoint(localposition);
        Quaternion rotation = alignRotation
            ? Quaternion.LookRotation(SplineUtility.EvaluateTangent(spline, t))
            : Quaternion.identity;
        InstantiateObject(position, rotation);
    }

    void InstantiateObject(Vector3 position, Quaternion rotation) {
        Instantiate(objectToPlace, position, rotation, transform);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected() {
        splineContainer = GetComponent<SplineContainer>();
        if (splineContainer == null || splineContainer.Spline == null)
            return;

        spline = splineContainer.Spline;

        if (numberOfObjects < 1)
            return;

        float splineLength = SplineUtility.CalculateLength(spline, transform.localToWorldMatrix);
        if (Mathf.Approximately(splineLength, 0f)) {
            Vector3 localPos = SplineUtility.EvaluatePosition(spline, 0f);
            Vector3 worldPos = splineContainer.transform.TransformPoint(localPos);
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(worldPos, 0.15f);
            return;
        }

        int placementCount = numberOfObjects;
        float tStart = Mathf.Clamp01(Mathf.Min(placementStart, placementEnd));
        float tEnd = Mathf.Clamp01(Mathf.Max(placementStart, placementEnd));

        Gizmos.color = Color.cyan;
        for (int i = 0; i < placementCount; i++) {
            float t;
            if (useClosedSpline) {
                t = (float)i / placementCount;
            }
            else {
                if (placementCount == 1) {
                    t = tStart;
                }
                else {
                    t = Mathf.Lerp(tStart, tEnd, (float)i / (placementCount - 1));
                }
            }
            Vector3 localPos = SplineUtility.EvaluatePosition(spline, t);
            if (verticalOffsetFollowsCurve) {
                Vector3 up = SplineUtility.EvaluateUpVector(spline, t);
                localPos += up.normalized * verticalOffset;
            }
            else {
                localPos.y += verticalOffset;
            }
            Vector3 worldPos = splineContainer.transform.TransformPoint(localPos);
            Gizmos.DrawSphere(worldPos, 1f);
        }
    }
#endif

}
