using RagdollEngine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBombUI : MonoBehaviour
{
    [SerializeField] private Image bombImage;
    private PumpkinDogAimPlayerBehaviour playerBehaviour;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private bool updateImage = false;
    public void Initialize(PumpkinDogAimPlayerBehaviour p)
    {
        playerBehaviour = p;
        playerBehaviour.OnFire += SetBombImage;

    }

    private void Update()
    {
        if (updateImage)
        {
            bombImage.fillAmount = 1f-playerBehaviour.GetCooldownNormalized();
            if(playerBehaviour.GetCooldownNormalized() <= 0)
            {
                updateImage = false;
            }
        }
    }

    public void SetBombImage()
    {
        updateImage = true;
    }
}
