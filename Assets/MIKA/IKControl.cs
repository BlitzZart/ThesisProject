using System.Collections;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AICharacterControl))]
public class IKControl : MonoBehaviour {

    protected Animator animator;

    private AICharacterControl aiCharacter;
    [Range(-1.0f, 1.0f)]
    public float hipOffset = 0;
    public bool ikActive = false;

    public bool useHead,
        useLeftElbow, useLeftHand, useRightElbow, useRightHand,
        useHip,
        useLeftKnee, useLeftFoot, useRightKnee, useRightFoot;

    public Transform hip;

    public Vector3 leftFootPosition, rightFootPosition, leftHandPosition, rightHandPosition;
    public Vector3 leftFootRotation, rightFootRotation, leftHandRotation, rightHandRotation;
    public Vector3 lookAtTarget;

    #region unity callbacks
    void Start() {
        animator = GetComponent<Animator>();
        aiCharacter = GetComponent<AICharacterControl>();
        ActivateAllIK();
    }

    private void Update() {
        Hip();
    }

    //a callback for calculating IK
    void OnAnimatorIK() {
        if (animator) {
            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive) {
                //UpdateUsingTransforms();
                UpdateUsingVectors();
            }
            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else {
                //animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
                //animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
                //animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
                //animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);

                animator.SetLookAtWeight(0);
            }
        }
        
    }
    #endregion

    #region public
    public void SetPosition(Vector3 destination)
    {
        //hip.position = destination;
        if (!ikActive)
            aiCharacter.SetDestination(destination);
        else
            transform.position = destination;
    }

    public void SetRotation(Quaternion rotation) {
        if (ikActive)
            transform.rotation = rotation;
    }

    #endregion

    #region private
    private void Head() {
        if (useHead) {
            animator.SetLookAtWeight(1);
            animator.SetLookAtPosition(lookAtTarget);
        } else {
            animator.SetLookAtWeight(0);
        }
    }
    private void Hip() {
        // ! hip is controlled by setting transform position
        if (!useHip)
            return;
    }
    private void Hands() {
        if (useLeftHand) {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            //animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPosition);
            //animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.Euler(leftHandRotation));
        } else {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        }

        if (useRightHand) {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            //animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPosition);
            //animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.Euler(rightHandRotation));
        } else {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
        }
    }
    private void Feet() {
        if (useLeftFoot) {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPosition);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.Euler(leftFootRotation));
        } else {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
        }

        if (useRightFoot) {
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPosition);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.Euler(rightFootRotation));
        } else {
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
        }
    }
    private void Elbows() {

    }
    private void Knees() {

    }

    private void UpdateUsingVectors()
    {
        Hands();
        Feet();
        Head();
    }

    private void ActivateAllIK() {
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

        animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 1);
        animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, 1);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

        animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1);
        animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);
    }

    #endregion
}
