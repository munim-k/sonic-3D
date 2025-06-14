using System;
using UnityEngine;

public class Obstacle : MonoBehaviour, IHittable {
    private Action OnHit;
    Action IHittable.OnHit { get => OnHit; set => OnHit = value; }

    public void DoHit(int damage) {
        OnHit?.Invoke();
        Destroy(gameObject);
    }

    HittableType IHittable.GetType() {
        return HittableType.Obstacle;
    }
}
