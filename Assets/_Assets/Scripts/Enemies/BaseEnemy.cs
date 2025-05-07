using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public interface BaseEnemy
{

    public void DoDamageToEnemy(int damage);



    public float GetHealthNormalized();
   
}
