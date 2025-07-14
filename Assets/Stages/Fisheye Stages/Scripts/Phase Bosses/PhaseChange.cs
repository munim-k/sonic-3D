using System;
using UnityEngine;

public class PhaseChange : MonoBehaviour
{
    private int phase = 1;
    public event Action<int> OnPhaseChange;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            OnPhaseChange?.Invoke(++phase);
        }
    }
}
