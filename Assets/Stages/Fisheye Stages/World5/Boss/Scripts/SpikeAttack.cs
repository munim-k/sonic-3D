using UnityEngine;

public class SpikeAttack : MonoBehaviour
{
    [SerializeField] private GameObject spikeObject;
    [SerializeField] private float spikeSpeed = 5f;
    [SerializeField] private FisheyeBoss5 boss;
    [SerializeField] private float spikeTime = 2f;
    private float spikeTimer = 0f;
    private GameObject player;
    private bool isAttacking = false;
    private bool isGoingUp = true;
    private Vector3 targetPosition;

    private void Start()
    {
        spikeObject.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");
        boss.OnSpikeAttack += Boss_OnSpikeAttack;
    }

    private void Boss_OnSpikeAttack()
    {
        spikeObject.transform.position = player.transform.position - new Vector3(0f, 7f, 0f);
        targetPosition = player.transform.position;
        spikeObject.SetActive(true);

        isAttacking = true;
        spikeTimer = 0f;
        isGoingUp = true;
    }

    private void Update()
    {
        if (isAttacking)
        {
            spikeTimer += Time.deltaTime;
            if (spikeTimer >= spikeTime)
            {
                isAttacking = false;
                spikeObject.SetActive(false);
            }
            spikeObject.transform.position = Vector3.Lerp(spikeObject.transform.position, targetPosition, spikeSpeed * Time.deltaTime);
            if (Vector3.Distance(spikeObject.transform.position, targetPosition) <= 0.1f)
            {
                if (isGoingUp)
                {
                    targetPosition = spikeObject.transform.position - new Vector3(0f, 7f, 0f);
                    isGoingUp = false;
                }
                else
                {
                    targetPosition = spikeObject.transform.position + new Vector3(0f, 7f, 0f);
                    isGoingUp = true;
                }
            }
        }
    }

    private void OnDestroy()
    {
        boss.OnSpikeAttack -= Boss_OnSpikeAttack;
    }
}
