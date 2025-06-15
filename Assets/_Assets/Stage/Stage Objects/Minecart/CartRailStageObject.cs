using UnityEngine;
using UnityEngine.Splines;

public class CartRailStageObject : StageObject {
    public SplineContainer splineContainer;
    [SerializeField] private float speedMultiplier = 1f;
    public float SpeedMultiplier {
        get => speedMultiplier;
    }

}
