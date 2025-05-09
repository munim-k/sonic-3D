using UnityEngine;

public class World1BossPillar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private World1Boss boss;
    [SerializeField] private MeshRenderer pillarRenderer;
    [SerializeField] private Material pillarMaterial;
    [SerializeField] private Material activePillarMaterial;


    private void Start()
    {

        pillarRenderer.material = pillarMaterial;
    }
    public void Activate(bool active)
    {
        
        if (active)
        {
            pillarRenderer.material = activePillarMaterial;
        }
        else
        {
            pillarRenderer.material = pillarMaterial;
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
