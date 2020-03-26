using System.Collections;
using System.Collections.Generic;
 
using UnityEngine;

public class animcontroller : MonoBehaviour
{
    Animator anim;
    float speed;
    bool walking;
    Transform leftLeg;
    bool ik;
    public Transform rightFootObj = null;
    public Transform lookObj = null;

    // Start is called before the first frame update
    void Start()
    {
        anim=GetComponent<Animator>();
        speed=0f; 
        walking=false;
        ik = false;
        leftLeg = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);

        
    }
    void OnAnimatorIK(int layerIndex)
    {
        if (ik == true)
        {
            Debug.Log("IK IS RUNNING");
            //anim.SetBoneLocalRotation(HumanBodyBones.LeftLowerLeg, Quaternion.Euler(new Vector3(30, 0, 30)));
            

            //if the IK is active, set the position and rotation directly to the goal. 
            

                // Set the look target position, if one has been assigned
                if (lookObj != null)
                {
                    anim.SetLookAtWeight(1);
                    anim.SetLookAtPosition(lookObj.position);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (rightFootObj != null)
                {
                    anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                    anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
                    anim.SetIKPosition(AvatarIKGoal.RightFoot, rightFootObj.position);
                    anim.SetIKRotation(AvatarIKGoal.RightFoot, rightFootObj.rotation);
                }
          
        }
        //if the IK is not active, set the position and rotation of the hand and head back to the original position
        else
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
            anim.SetLookAtWeight(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown("space")){
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            

            if(stateInfo.IsTag("walkanim")) walking=true; else walking=false;
            Debug.Log("space key was pressed");
            anim.SetBool("isWalk",!walking);

        }
        if (Input.GetKeyDown("w")){
            Debug.Log("W key was pressed");
            anim.SetFloat("Speed",2);
        }
        if (Input.GetKeyDown("i"))
        { ik = true; }
        if (Input.GetKeyDown("k"))
        { ik = false; }

    }
}
