    using UnityEngine;
    using UnityEngine.Splines;

    public class SlideStageObject : StageObject {
        public SplineContainer splineContainer;
        [SerializeField] private float speedMultiplier = 1f;
        public float SpeedMultiplier {
            get => speedMultiplier;
        }

    }
