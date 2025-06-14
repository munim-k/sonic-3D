using UnityEngine;

public class World1BossPillarTrigger : Volume {
    [SerializeField] private World1BossPillar pillar;


    public void Activate() {
        if (pillar != null) {
            pillar.Interact();
        }
    }
}
