using UnityEngine;

public class World8BossBomb : MonoBehaviour {
    [SerializeField] private Transform explosionTrigger;
    [SerializeField] private float explosionLingerTime = 1f;
    [SerializeField] private Transform explosionVFX;


    private void Start() {
        explosionTrigger.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision) {
        Explode();
    }



    private void Explode() {
        Instantiate(explosionVFX, transform.position, Quaternion.identity);
        explosionTrigger.gameObject.SetActive(true);
        DetachAndDestroy(explosionTrigger);
        Destroy(gameObject, 0.1f);
    }

    private void DetachAndDestroy(Transform obj) {
        obj.transform.parent = null;
        Destroy(obj.gameObject, explosionLingerTime);
    }


}
