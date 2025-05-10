using UnityEngine;

public class World2BossProjectile : MonoBehaviour
{
    [SerializeField] private float descentSpeed=1f;

    void FixedUpdate()
    {
        transform.Translate(new Vector3(0, descentSpeed*Time.fixedDeltaTime, 0));
    }
}
