

using UnityEngine;

class FollowFixed : MonoBehaviour
{
    [SerializeField] private Transform followTransform;

    private void FixedUpdate()
    {
        if(followTransform != null)
        {
            transform.position = followTransform.position;
            transform.rotation = followTransform.rotation;
        }
    }
}