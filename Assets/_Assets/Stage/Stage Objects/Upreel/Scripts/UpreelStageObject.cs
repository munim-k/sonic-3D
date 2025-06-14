using UnityEngine;

public class UpreelStageObject : StageObject {
    public float extensionSpeed;

    public float retractionSpeed;

    [SerializeField] Transform stand;

    [SerializeField] LineRenderer lineRenderer;

    [SerializeField] AudioSource audioSourceLoop1;

    [SerializeField] AudioSource audioSourceIntro2;

    [SerializeField] AudioSource audioSourceLoop2;

    [SerializeField] bool defaultExtended;

    [SerializeField] float length;

    [HideInInspector] public bool idle;

    [HideInInspector] public bool extended;

    State state = State.Retracted;

    float goalLength;

    float currentLength;

    float sign;

    enum State {
        Extended,
        Retracted,
        Extending,
        Retracting
    }

    void OnEnable() {
        lineRenderer.positionCount = 2;

        idle = true;

        if (defaultExtended) {
            currentLength = length;

            goalLength = length;
        }
        else {
            currentLength = 0;

            goalLength = 0;
        }

        UpdatePosition();
    }

    void FixedUpdate() {

        if (currentLength == goalLength)
            SetState(extended ? State.Extended : State.Retracted);
        else if (currentLength < goalLength)
            SetState(State.Extending);
        else if (currentLength > goalLength)
            SetState(State.Retracting);

        switch (state) {
            case State.Retracted:
                goalLength = length;
                break;
            case State.Extending:
                UpdatePosition();
                break;
        }

    }

    void SetState(State state) {
        if (this.state == state)
            return;

        audioSourceLoop1.Stop();

        audioSourceIntro2.Stop();

        audioSourceLoop2.Stop();

        switch (state) {
            case State.Extending:
                audioSourceLoop1.Play();
                break;
            case State.Retracting:
                audioSourceLoop1.Play();

                audioSourceIntro2.Play();

                audioSourceLoop2.PlayScheduled(AudioSettings.dspTime + audioSourceIntro2.clip.length);

                break;
        }

        this.state = state;
    }

    public void UpdatePosition() {
        if (currentLength < goalLength)
            currentLength = Mathf.Min(currentLength + (extensionSpeed * Time.fixedDeltaTime), goalLength);
        else
            currentLength = Mathf.Max(currentLength - (retractionSpeed * Time.fixedDeltaTime), goalLength);

        transform.position = transform.parent.position - (Vector3.up * currentLength);

        transform.rotation = extended ? Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.parent.forward, Vector3.up), Vector3.up) : transform.parent.rotation;

        lineRenderer.SetPosition(0, stand.position);

        lineRenderer.SetPosition(1, transform.position);
    }

    public void Retract() {
        goalLength = 0;
    }
}
