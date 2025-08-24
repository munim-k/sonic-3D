using UnityEngine;

public class SquasherBotVisual : MonoBehaviour
{
    [SerializeField] private SquasherBotEnemy squasherBot;
    [SerializeField] private Transform squasherBotTransform;
    [SerializeField] private Transform visualTransform;

    private bool updateVisual = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        squasherBot.OnStateChange += UpdateVisual;
    }

    private void UpdateVisual(SquasherBotEnemy.State state) {
        switch (state) {
            case SquasherBotEnemy.State.Idle:
                updateVisual = false;
                break;
            case SquasherBotEnemy.State.Squashing:
                updateVisual = true;
                break;
            case SquasherBotEnemy.State.Squashed:
                updateVisual = false;
                break;
            case SquasherBotEnemy.State.Rising:
                updateVisual = true;
                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(updateVisual) {
            visualTransform.position = squasherBotTransform.position;
        }
    }
}
