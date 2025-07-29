using UnityEngine;

public class PlaneTriggerZone : MonoBehaviour
{
    public CirclingPlane plane;  // Assign the plane object in inspector

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            plane.SetPlayer(other.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            plane.SetPlayer(null);
        }
    }

    public float GetRadius()
    {
        Debug.Log(GetComponent<SphereCollider>().radius);
        return GetComponent<SphereCollider>().radius;
    }
}