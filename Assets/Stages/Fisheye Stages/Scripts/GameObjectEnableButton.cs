using UnityEngine;

public class GameObjectEnableButton : MonoBehaviour
{
    [SerializeField] GameObject targetGameObject;
    [SerializeField] bool enable = true;

    [Header("Color Changing Things")]
    [SerializeField] GameObject buttonTop;

    [SerializeField] Color enabledColor = Color.green;
    [SerializeField] Color disabledColor = Color.red;
    private void Start()
    {
        if (targetGameObject == null)
        {
            Debug.LogError("Target GameObject is not assigned in the inspector.");
        }

        if (targetGameObject.activeSelf == enable)
        {
            targetGameObject.SetActive(!enable);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            targetGameObject.SetActive(enable);

            if (buttonTop != null)
            {
                buttonTop.GetComponent<MeshRenderer>().materials[0].color = enable ? enabledColor : disabledColor;
            }
        }
    }
}
