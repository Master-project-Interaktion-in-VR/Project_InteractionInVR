using System;
using UnityEngine;

/// <summary>
/// https://blog.immersive-insiders.com/animate-avatar-for-vr-in-unity/
/// </summary>


[System.Serializable]
public class MapTransforms
{
    public Transform vrTarget;
    public Transform ikTarget;

    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public void VrMapping()
    {
        ikTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        ikTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}

public class AvatarController : MonoBehaviour
{
    [SerializeField]
    private MapTransforms head;

    [SerializeField]
    private MapTransforms leftHand;

    [SerializeField]
    private MapTransforms rightHand;

    [SerializeField]
    private float turnSmoothness;

    [SerializeField]
    private Transform ikHead;

    [SerializeField]
    private Vector3 headBodyOffset;

    [Tooltip("If the player is very tall, the avatar is scaled")]
    [SerializeField]
    private Vector3 tallHeadBodyOffset;

    [SerializeField]
    private float minHeadFloorDistance;


    private bool _isTall;


    private void LateUpdate()
    {
        Physics.Raycast(ikHead.position, Vector3.down, out RaycastHit hitFloor, 10, 1 << LayerMask.NameToLayer("Drawable"));

        if (hitFloor.distance < minHeadFloorDistance)
        {
            transform.position = ikHead.position + headBodyOffset + new Vector3(0, minHeadFloorDistance - hitFloor.distance, 0);
        }
        else if (!_isTall && hitFloor.distance > Math.Abs(headBodyOffset.y))
        {
            _isTall = true;
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
        else
        {
            Vector3 hbOffset = _isTall ? tallHeadBodyOffset : headBodyOffset;
            transform.position = ikHead.position + hbOffset;
        }
        Debug.Log(hitFloor.distance);


        transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(ikHead.forward, Vector3.up).normalized, Time.deltaTime * turnSmoothness);

        head.VrMapping();
        leftHand.VrMapping();
        rightHand.VrMapping();
    }

}
