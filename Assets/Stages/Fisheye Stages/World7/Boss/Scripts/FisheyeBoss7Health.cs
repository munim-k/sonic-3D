using System;
using UnityEngine;
using UnityEngine.UI;

public class FisheyeBoss7Health : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth = 100f;
    private bool hasSecondPhaseStarted = false;
    [SerializeField] private GameObject headObject;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "EnemyDamage")
        {
            Debug.Log("Hello");
            currentHealth -= 10f;
            healthBar.fillAmount = currentHealth / maxHealth;

            if (currentHealth <= maxHealth / 2 && !hasSecondPhaseStarted)
            {
                hasSecondPhaseStarted = true;

                GameObject newHead = Instantiate(headObject, headObject.transform.position, headObject.transform.rotation);
                newHead.transform.parent = transform;
                newHead.transform.localScale = headObject.transform.localScale;

                headObject.transform.parent = null;
                headObject.AddComponent(typeof(HeadFollow));
            }

            if (currentHealth <= 0f)
            {
                Destroy(gameObject);
                Destroy(headObject);
            }
        }
    }
}
