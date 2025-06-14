using UnityEngine;

public class LollipopCollectible : Trigger {
    [SerializeField] private AnimationCurve positionCurve;
    [SerializeField] private Transform visualTransform;
    [SerializeField] private float positionSpeed = 1f;
    [SerializeField] private AnimationCurve rotationCurve;
    [SerializeField] private float rotationSpeed = 1f;
    private float positionLerp = 0f;
    private float rotationLerp = 0f;
    private Vector3 originalPos;
    private Quaternion originalRot;

    private void Awake() {
        originalPos = visualTransform.position;
        originalRot = visualTransform.rotation;
    }
    private void FixedUpdate() {
        positionLerp += Time.fixedDeltaTime * positionSpeed;
        rotationLerp += Time.fixedDeltaTime * rotationSpeed;
        positionLerp %= 1;
        rotationLerp %= 1;
        visualTransform.position = Vector3.Lerp(originalPos, originalPos + Vector3.up * 1f, positionCurve.Evaluate(positionLerp));
        visualTransform.rotation = originalRot * Quaternion.Euler(0, rotationCurve.Evaluate(rotationLerp) * 360f, 0);
    }
    public void Consume() {
        Destroy(gameObject);
    }
}
