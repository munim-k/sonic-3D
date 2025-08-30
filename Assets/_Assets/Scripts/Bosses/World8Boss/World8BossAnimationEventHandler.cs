using UnityEngine;

public class World8BossAnimationEventHandler : MonoBehaviour {

    [SerializeField] private World8Boss boss;

    public void MagicAttackStart() {
        boss.StartSpawningPillars();
    }

    public void TantrumShockwave1() {
        boss.SpawnTrantrumShockwave();

    }
    public void TantrumShockwave2() {
        boss.SpawnTrantrumShockwave();
    }

    public void TantrumEnd() {
        boss.EndTantrum();
    }
}
