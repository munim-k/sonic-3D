using UnityEngine;

public class World6BossBomb : MonoBehaviour
{
    [SerializeField] private float timeToExplode=3f;
    private float timer = 0f;
    [SerializeField] private Transform explosionTrigger;
    [SerializeField] private float explosionLingerTime=1f;
    [SerializeField] private Transform explosionVFX;
    [SerializeField] private Transform bombVisual;

    private Material bombMat;

    private void Start() {
        bombMat = bombVisual.GetComponent<Renderer>().material;
        if (bombMat == null) {
            Debug.LogError("Bomb material not found on bomb visual.");
        }
        explosionTrigger.gameObject.SetActive(false);
        timer = 0f;
    }
    private void Update() {
        timer += Time.deltaTime;
        UpdateMat();
        if (timer >= timeToExplode) {
           Instantiate(explosionVFX,transform.position,Quaternion.identity);
            explosionTrigger.gameObject.SetActive(true);
            DetachAndDestroy(explosionTrigger);
            Destroy(gameObject,0.1f);
        }
    }

    private void DetachAndDestroy(Transform obj) {
        obj.transform.parent = null;
        Destroy(obj.gameObject, explosionLingerTime);
    }

    private void UpdateMat() {
        //Make the material blink in emission the closer it gets to the explosion
        float emissionLerp = timer / timeToExplode;
        //Get the emission value on a sin curve
        float emission = Mathf.Sin(emissionLerp * emissionLerp * Mathf.PI * 2) * 0.5f + 0.5f; // Range from 0 to 1
        bombMat.SetFloat("_Emission", emission);
    }
}
