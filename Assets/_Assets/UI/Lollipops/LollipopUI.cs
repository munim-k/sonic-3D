using RagdollEngine;
using TMPro;
using UnityEngine;

public class LollipopUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lollipopsText;

    public void Initialize(PlayerBehaviourTree p )
    {
        foreach(PlayerBehaviour b in p.behaviours)
        {
            //If behaviour is of type LollipopCollectionPlayerBehaviour
            if (b is LollipopCollectionPlayerBehaviour lollipopBehaviour)
            {
                lollipopBehaviour.onLollipopChange += SetLollipopNum;
                break;
            }

        }

    }

    private void SetLollipopNum(int num)
    {
        lollipopsText.text = num.ToString();
    }

}
