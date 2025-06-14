using UnityEngine;
using UnityEngine.UI;

public class World3BossVisuals : MonoBehaviour {
    [SerializeField] private World3Boss boss;
    [SerializeField] private Image healthBar;
    [SerializeField] private Material coreActiveMat;
    [SerializeField] private Material coreInactiveMat;
    [SerializeField] private MeshRenderer coreRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private World3Boss.State state;
    void Start() {
        boss.OnStateChange += OnStateChange;
        ((IHittable)boss).OnHit += OnDamage;
    }


    private void OnStateChange(World3Boss.State s) {
        state = s;
        switch (state) {
            case World3Boss.State.Flamethrower:
                SetCoreState(false);
                break;
            case World3Boss.State.HomingAttacks:
                SetCoreState(true);
                break;
            case World3Boss.State.Dead:
                SetCoreState(false);
                break;
            default:
                break;
        }
    }

    private void OnDamage() {
        if (healthBar != null) {
            healthBar.fillAmount = boss.GetHealthNormalized();
        }
    }

    private void SetCoreState(bool damageable) {
        if (damageable) {
            coreRenderer.material = coreActiveMat;
        }
        else {
            coreRenderer.material = coreInactiveMat;
        }
    }
}
