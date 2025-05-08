using UnityEngine;

public class SnowParticlesInit : MonoBehaviour
{
    [SerializeField] private Transform SnowParticlePrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(SnowParticlePrefab,Player.CharacterInstance.playerBehaviourTree.modelTransform);
    }

    
}
