using UnityEngine;
using System;
using System.Collections;


[RequireComponent(typeof(Animator))]

public class IKControl : MonoBehaviour {

    protected Animator animator;

    [Range(0.0f, 1.0f)]
    public float weight = 1;
    public bool ikActive = false;

    public bool useHead,
        useLeftElbow, useLeftHand, useRightElbow, useRightHand,
        useHip,
        UseLeftKnee, useLeftFoot, useRightKnee, useLeftKnee;

    public Transform rightFootObj = null;
    public Transform leftFootObj = null;
    public Transform lookObj = null;

    public Transform leftKneeHint = null;

    #region unity callbacks
    void Start() {
        animator = GetComponent<Animator>();
    }

    //a callback for calculating IK
    void OnAnimatorIK() {
        if (animator) {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive) {

                // Set the look target position, if one has been assigned
                if (lookObj != null) {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }

                if (leftKneeHint != null) {
                    animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 1);
                    animator.SetIKHintPosition(AvatarIKHint.LeftKnee, leftKneeHint.position);
                }
          
                // Set the right hand target position and rotation, if one has been assigned
                if (rightFootObj != null) {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, weight);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, weight);
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootObj.rotation);
                }
                if (leftFootObj != null) {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, weight);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, weight);
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootObj.rotation);
                }
            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else {
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);

                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);

                animator.SetLookAtWeight(0);
            }
        }
    }
    #endregion

    #region private
    private void Head() {

    }
    private void Hip() {

    }
    private void Hands() {

    }
    private void Feet() {

    }
    private void Elbows() {

    }
    private void Knees() {

    }
    #endregion
}
