using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class ForwardSpike : MonoBehaviour
{
    [SerializeField] private List<GameObject> spikes;

    [SerializeField] private float throwForce = 10f;

    private void Start()
    {
        // Deactivate all spikes at the start
        foreach (GameObject spike in spikes)
        {
            spike.SetActive(false);
        }
    }
    public void StartAttack()
    {
        StartCoroutine(ActivateSpikes());
        StartCoroutine(ThrowSpikes());
    }

    // a co routine to activate the spikes one by one
    private IEnumerator ActivateSpikes()
    {
        foreach (GameObject spike in spikes)
        {
            spike.SetActive(true);
            yield return new WaitForSeconds(0.25f); // wait for half a second before activating the next spike
        }
    }

    // a co routine to throw the spikes one by one in there forward direction
    private IEnumerator ThrowSpikes()
    {
        yield return new WaitForSeconds(1f); // wait for some time before throwing the first spike

        foreach (GameObject spike in spikes)
        {
            Rigidbody rb = spike.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // make the spike dynamic
                rb.AddForce(spike.transform.forward * throwForce, ForceMode.Impulse); // throw the spike in the forward direction
            }
            yield return new WaitForSeconds(0.25f); // wait for half a second before throwing the next spike
        }
    }
}
