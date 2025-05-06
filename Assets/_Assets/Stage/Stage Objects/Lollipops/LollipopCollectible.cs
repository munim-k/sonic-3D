using UnityEngine;

public class LollipopCollectible : Trigger
{

    public void Consume()
    {
        Destroy(gameObject);
    }
}
