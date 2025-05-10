using UnityEngine;

public class ParticlesInit : MonoBehaviour
{
    [SerializeField] private Transform particlePrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(particlePrefab,Player.CharacterInstance.playerBehaviourTree.modelTransform);
    }

    
}
