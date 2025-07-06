using UnityEngine;

public class World7BossAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private World7Boss boss;
    private void StartRings() {
        boss.StartSpawningRings();
    }

    private void ExitRings() {
        boss.ExitRingState();
    }

    private void HorizontalSlash() {
        boss.SpawnHorizontalSlash();
    }

    private void VerticalSlash() {
        boss.SpawnVerticalSlash();
    }

    private void ExitSlash() {
        boss.EndSlash();
    }

}
