using System;
using UnityEngine;

public class BossHitbox : MonoBehaviour
{
    public int phase = 1;
    public event Action<int> OnPhaseChange;

    private void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            OnPhaseChange?.Invoke(++phase);
        }
    }
}
