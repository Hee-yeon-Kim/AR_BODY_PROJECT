using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ikcontroller : MonoBehaviour
{
    ShowOrigin main;
    Animator anim2;
 
    BluetoothManager btmanager;

    float pitch1 =0;
    float _pitch1=0;
    float roll1=0;
    float _roll1=0;
    float gap_p1 = 0;
    float gap_r1 = 0;
    float pitch2 = 0;
    float roll2 = 0;
    float _roll2 = 0;
    float gap_p2 = 0;
    float gap_r2= 0;
    float bent = 0;
    float _bent = 0;
    float gap_bent = 0;
    bool enter;
  


    // Start is called before the first frame update
    void Start()
    {
        enter = false; 
        btmanager = GameObject.FindGameObjectWithTag("BtManager").GetComponent<BluetoothManager>();
        main = GameObject.FindGameObjectWithTag("origin").GetComponent<ShowOrigin>();
        anim2 = gameObject.GetComponent<Animator>();
       
    }
    
    
    
    private void OnAnimatorIK(int layerIndex)
    {
        if (main.ik == false | btmanager.connect1 == false | btmanager.connect2 == false)
            return;
        if (true)//왼쪽이 비마비
        {

            //허벅지
            if (btmanager.isnew1)//새로운 데이터 유입시 움직일 gap 방향 , 총 회전각 업데이트
            {
                pitch1 =  btmanager.upperleg_array[1];
                pitch1 *= main.multi;
                roll1 =  btmanager.upperleg_array[2];

                gap_p1 = pitch1 - _pitch1;//y축
                gap_r1 = roll1 - _roll1;//z축
                btmanager.isnew1 = false;// 텀 동안의 계산
            }
            else
            {
                if ((gap_p1 < 0 && _pitch1 > pitch1) || (gap_p1 > 0 && _pitch1 < pitch1))//움직일 게 남아있을 때
                {
                    _pitch1 += gap_p1 * Time.deltaTime * 2;
                }
                if ((gap_r1 < 0 && _roll1 > roll1) || (gap_r1 > 0 && _roll1 < roll1))
                {
                    _roll1 += gap_r1 * Time.deltaTime * 2;
                }
            }
            //종아리
            if (btmanager.isnew2)//새로운 데이터 유입시 움직일 gap 방향 , 총 회전각 업데이트
            {
                pitch2 =  btmanager.lowerleg_array[1]*main.multi;
                roll2 = btmanager.lowerleg_array[2];
                bent = pitch1 - pitch2;//0이 최대
                if (bent > 0) bent = 0;
                gap_bent = bent - _bent;

                btmanager.isnew2 = false;// 텀 동안의 계산
            }
            else
            {
                if ((gap_bent < 0 && _bent > bent) || (gap_bent > 0 && _bent < bent))//움직일 게 남아있을 때
                {
                    _bent += gap_bent * Time.deltaTime * 2;
                }

            }

            anim2.SetBoneLocalRotation(HumanBodyBones.LeftUpperLeg, Quaternion.Euler(180 - _pitch1, 180, -1 * _roll1));
            anim2.SetBoneLocalRotation(HumanBodyBones.LeftLowerLeg, Quaternion.Euler(_bent, 0, 0));

            if (main.diff > 0.1)
            {
                anim2.SetBoneLocalRotation(HumanBodyBones.RightUpperLeg, Quaternion.Euler(-180 + _pitch1, 180, 0));
                anim2.SetBoneLocalRotation(HumanBodyBones.RightLowerLeg, Quaternion.Euler(-25f, 0, 0));
            
            }
           
            

         }
        else//오른쪽이 비마비
        {
          
        }
        
    }

}
