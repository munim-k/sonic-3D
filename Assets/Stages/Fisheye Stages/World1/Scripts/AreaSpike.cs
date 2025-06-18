using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class AreaSpike : MonoBehaviour
{
    [SerializeField] private List<GameObject> areaSpikeSingle;

    private void Start()
    {
        // Deactivate all area spikes at the start
        foreach (GameObject spike in areaSpikeSingle)
        {
            spike.SetActive(false);
        }
    }
    public void StartAttack()
    {
        StartCoroutine(ActivateAreaSpikes());
        StartCoroutine(DeactivateAreaSpikes());
    }

    // A coroutine to activate the area spikes one by one
    private IEnumerator ActivateAreaSpikes()
    {
        foreach (GameObject spike in areaSpikeSingle)
        {
            spike.SetActive(true);
            yield return new WaitForSeconds(0.25f); // wait for half a second before activating the next spike
        }
    }

    // A coroutine to deactivate the area spikes after some time
    private IEnumerator DeactivateAreaSpikes()
    {
        yield return new WaitForSeconds(1); // wait for some time before deactivating the spikes

        foreach (GameObject spike in areaSpikeSingle)
        {
            spike.SetActive(false);
            yield return new WaitForSeconds(0.25f); // wait for some time before deactivating the next spike
        }
    }
}
