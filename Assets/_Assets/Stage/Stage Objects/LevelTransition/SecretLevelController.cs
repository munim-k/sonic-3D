using RagdollEngine;
using UnityEngine;

public class SecretLevelController : MonoBehaviour
{
    private LollipopCollectionPlayerBehaviour lollipopBehaviour;
    [SerializeField] private Transform secretLevelTransform;
    void Start()
    {
        foreach(PlayerBehaviour behaviour in Player.CharacterInstance.playerBehaviourTree.behaviours)
        {
            if (behaviour is LollipopCollectionPlayerBehaviour lollipop)
            {
                lollipopBehaviour = lollipop;
                break;
            }
        }
        lollipopBehaviour.onLollipopChange += OnLollipopCollected;
        OnLollipopCollected(lollipopBehaviour.GetLollipops());
    }

    private void OnLollipopCollected(int lollipops)
    {
        if (lollipops >= 50)
        {
            secretLevelTransform.gameObject.SetActive(true);
        }
        else
        {
            secretLevelTransform.gameObject.SetActive(false);
        }
    }


}
