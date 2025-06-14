using RagdollEngine;
using UnityEngine;

public class LollipopCounter : MonoBehaviour {
    [SerializeField] PlayerBehaviourTree playerBehaviourTree;

    [SerializeField] LollipopUI lollipopUIPrefab;

    void Update() {
        if (!playerBehaviourTree.initialized)
            return;

        LollipopUI lollipopUI = Instantiate(lollipopUIPrefab, playerBehaviourTree.canvas.transform);

        playerBehaviourTree.character.uis.Add(lollipopUI.gameObject);

        lollipopUI.Initialize(playerBehaviourTree);

        Destroy(gameObject);
    }
}
