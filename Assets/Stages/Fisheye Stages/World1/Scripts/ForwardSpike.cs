using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class ForwardSpike : MonoBehaviour
{
    [SerializeField] private List<GameObject> spikes;
    private List<Vector3> spikePostions;
    private List<Quaternion> spikeRotations;
    [SerializeField] private float throwForce = 100f;
    private void Start()
    {
        //save the transforms of the spikes
        spikePostions = new List<Vector3>();
        spikeRotations = new List<Quaternion>();
        foreach (GameObject spike in spikes)
        {
            spikePostions.Add(spike.transform.localPosition);
            spikeRotations.Add(spike.transform.localRotation);
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
        //make the transforms of the spikes the same as the transforms of the spikeTransforms
        StartCoroutine(ResetSpikes());

        yield return new WaitForSeconds(0.1f); // wait for some time before activating the first spike
        foreach (GameObject spike in spikes)
        {
            spike.SetActive(true);
            yield return new WaitForSeconds(0.1f); // wait for half a second before activating the next spike
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
                rb.AddForce(spike.transform.up * throwForce, ForceMode.Impulse); // throw the spike in the forward direction
            }
        }
    }

    private IEnumerator ResetSpikes()
    {
        yield return null;

        for (int i = 0; i < spikes.Count; i++)
        {
            GameObject spike = spikes[i];
            Rigidbody rb = spike.GetComponent<Rigidbody>();

            // Reset transform
            spike.transform.localPosition = spikePostions[i];
            spike.transform.localRotation = spikeRotations[i];

            // Reset Rigidbody
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            // Deactivate the spike
            spike.SetActive(false);
        }
    }
}
