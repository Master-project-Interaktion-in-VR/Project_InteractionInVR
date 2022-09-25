using UnityEngine;

/// <summary>
/// Code base from https://blog.immersive-insiders.com/animating-ready-player-me-lower-body-for-vr-in-unity/
/// </summary>
public class LowerBodyAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    [Range(0, 1)]
    private float leftFootPositionWeight;

    [SerializeField]
    [Range(0, 1)]
    private float rightFootPositionWeight;


    [SerializeField]
    [Range(0, 1)]
    private float leftFootRotationWeight;

    [SerializeField]
    [Range(0, 1)]
    private float rightFootRotationWeight;

    [SerializeField]
    private Vector3 footOffset;

    [SerializeField]
    private Vector3 raycastLeftOffset;

    [SerializeField]
    private Vector3 raycastRightOffset;


    private void OnAnimatorIK(int layerIndex)
    {
        Vector3 leftFootPosition = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
        Vector3 rightFootPosition = animator.GetIKPosition(AvatarIKGoal.RightFoot);

        RaycastHit hitLeftFoot;
        RaycastHit hitRightFoot;

        bool isLeftFootDown = Physics.Raycast(leftFootPosition + raycastLeftOffset, Vector3.down, out hitLeftFoot, 10, 1 <<LayerMask.NameToLayer("Drawable"));
        bool isRightFootDown = Physics.Raycast(rightFootPosition + raycastRightOffset, Vector3.down, out hitRightFoot, 10, 1 << LayerMask.NameToLayer("Drawable"));

        CalculateFoot(AvatarIKGoal.LeftFoot, isLeftFootDown, hitLeftFoot, leftFootPositionWeight, leftFootRotationWeight);

        CalculateFoot(AvatarIKGoal.RightFoot, isRightFootDown, hitRightFoot, rightFootPositionWeight, rightFootRotationWeight);

    }

    private void CalculateFoot(AvatarIKGoal goal, bool isFootDown, RaycastHit hitFoot, float footPositionWeight, float footRotationWeight)
    {
        if (isFootDown)
        {
            animator.SetIKPositionWeight(goal, footPositionWeight);
            animator.SetIKPosition(goal, hitFoot.point + footOffset);
            //Debug.Log(hitFoot.point);

            Quaternion footRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, hitFoot.normal), hitFoot.normal);
            animator.SetIKRotationWeight(goal, footRotationWeight);
            animator.SetIKRotation(goal, footRotation);
        }
        else
        {
            animator.SetIKPositionWeight(goal, 0);
        }
    }

}
