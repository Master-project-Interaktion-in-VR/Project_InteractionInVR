using UnityEngine;

/// <summary>
/// 
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
    private float rightFootRotationnWeight;

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

        bool isLeftFoodDown = Physics.Raycast(leftFootPosition + raycastLeftOffset, Vector3.down, out hitLeftFoot);
        bool isRightFoodDown = Physics.Raycast(rightFootPosition + raycastRightOffset, Vector3.down, out hitRightFoot);

        if (isLeftFoodDown)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootPositionWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, hitLeftFoot.point + footOffset);

            Quaternion leftFootRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, hitLeftFoot.normal), hitLeftFoot.normal);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootRotationWeight);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootRotation);
        }

        // TODO CONTINUE

    }

}
