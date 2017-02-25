using System.Collections;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AICharacterControl))]
public class IKControl : MonoBehaviour {

    protected Animator animator;

    private AICharacterControl aiCharacter; 
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
    public Transform hip;

    public Vector3 leftFootPosition, rightFootPosition, leftHandPosition;
    public Vector3 leftFootRotation, rightFootRotation, leftHandRotation;

    private bool handInit = false;

    private Vector3 hipPosition;

    #region unity callbacks
    void Start() {
        animator = GetComponent<Animator>();
        aiCharacter = GetComponent<AICharacterControl>();


        StartCoroutine(InitHandsDelayed());
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
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);

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

    }
    private void Hip() {

    }

    private IEnumerator InitHandsDelayed() {
        yield return new WaitForSeconds(1);
        handInit = true;
    }
    private void Hands() {
        //if (!handInit)
        //    return;

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
        //animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
        print("Foot " + leftFootPosition);
        print("Hand " + leftHandPosition);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPosition);
        //animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.Euler(leftHandRotation));
    }
    private void Feet() {
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, weight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, weight);
        animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPosition);
        animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.Euler(leftFootRotation));

        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, weight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, weight);
        animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPosition);
        animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.Euler(rightFootRotation));
    }
    private void Elbows() {

    }
    private void Knees() {

    }

    private void UpdateUsingVectors()
    {
        Hands();
        Feet();
        Hip();
    }

    private void UpdateUsingTransforms() {
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
    #endregion
}
