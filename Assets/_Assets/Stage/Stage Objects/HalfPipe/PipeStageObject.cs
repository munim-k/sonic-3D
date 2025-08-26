using UnityEngine;
using UnityEngine.Splines;

public class PipeStageObject : StageObject {
    public SplineContainer splineContainer;
    public SplineContainer profileSplineContainer;
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float profileScale = 1f;
    [SerializeField] private float lateralSpeed = 1f;
    public float ProfileScale {
        get => profileScale;
    }
    public float LateralSpeed {
        get => lateralSpeed;
    }

    public float SpeedMultiplier {
        get => speedMultiplier;
    }

}
