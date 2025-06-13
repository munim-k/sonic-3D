using UnityEngine;

public enum HittableType
{
    Enemy,
    Obstacle
}
public interface IHittable
{

    public void DoHit(int damage);

    public HittableType GetType();

}
