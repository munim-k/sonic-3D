using System;

public interface BaseEnemy : IHittable {
    public Action OnDeath { get; set; }
    public float GetHealthNormalized();

}
