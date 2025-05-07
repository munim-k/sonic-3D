using UnityEngine;

public class World1BossPillar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private World1Boss boss;
    [SerializeField] private Material pillarMaterial;
    [SerializeField] private Material activePillarMaterial;
    private MeshRenderer pillarMesh;
        
    

    public void Activate(bool active)
    {
        if (pillarMesh == null)
        {
            pillarMesh = GetComponent<MeshRenderer>();
        }
        if (active)
        {
            pillarMesh.material = activePillarMaterial;
        }
        else
        {
            pillarMesh.material = pillarMaterial;
        }

    }

    
    public void Interact()
    {
        if (boss != null)
        {
            boss.ActivatePillar(this);
        }
    }
}
