using UnityEngine;

public class MovingEnvPiece : MonoBehaviour
{
    [SerializeField] Transform ObstacleTrigger;
    [SerializeField] private Animation animation;
    private IHittable hittableObstacle;
    void Start()
    {
        if (ObstacleTrigger != null)
        {
           hittableObstacle= ObstacleTrigger.GetComponent<IHittable>();
            if (hittableObstacle != null)
            {
                hittableObstacle.OnHit += OnHit;
            }
        }
    }

    private void OnHit()
    {
        if (animation != null)
        {
            animation.Play();
        }
    }
}
