using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class ShowOrigin : MonoBehaviour
{
    private ARSessionOrigin m_SessionOrigin;
    private ARPlaneManager m_ARPlaneManager;
    public GameObject gizmo;
    private GameObject prefab1;
    public GameObject human;
    public GameObject trace;
    private GameObject prefab2;
    private TrackableId floorID;
    private bool isFloor=false;
    private Plane floor;
    private float eyeheight;
    private float floorheight;
    private float distance;
    private float timer;
    private List<GameObject> footlist;
    public Text forTest1;
    // Start is called before the first frame update
    string test13 = ""; string test12 = "";
    void Awake()
    {
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
        m_ARPlaneManager=GetComponent<ARPlaneManager>();
    }
    void Start()
    {
        
        Vector3 tempp= new Vector3();
        footlist = new List<GameObject>();
        tempp = m_SessionOrigin.camera.transform.position;

        prefab1 = Instantiate(gizmo, tempp, Quaternion.identity);
        prefab1.transform.SetParent(m_SessionOrigin.transform);
        
        prefab2 =Instantiate(human, tempp, Quaternion.identity);
        prefab2.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        prefab2.transform.SetParent(m_SessionOrigin .transform);
        timer = 0;
        
      
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        //prefab1.transform.position = m_SessionOrigin.camera.transform.position;
        Vector3 direction = m_SessionOrigin.camera.transform.rotation * Vector3.forward;
        prefab1.transform.position = m_SessionOrigin.camera.transform.position + direction * 0.4f;

        // prefab1.transform.Translate(new Vector3(0,0,0.5f),m_SessionOrigin.camera.transform);계속 움직임  멀리
        //   prefab1.transform.rotation=m_SessionOrigin.camera.transform.rotation;
       
        //모델 포지션
        Vector3 vemp2 = new Vector3();
       
        Vector3 vemp3 = new Vector3();//xz 설정
         vemp3 = m_SessionOrigin.camera.transform.rotation * Vector3.back*2f;
        vemp3.y = 0;

        vemp2.x = m_SessionOrigin.camera.transform.position.x + vemp3.x;
        vemp2.z = m_SessionOrigin.camera.transform.position.z + vemp3.z;
        vemp2.y = floorheight * -1 - 0.7f;//키에 맞게 설정


        prefab2.transform.position = vemp2;
        //모델 rotation
        Quaternion vempp = new Quaternion(0, 0, 0, 0);
        vempp.y = m_SessionOrigin.camera.transform.rotation.y;
        vempp.w = m_SessionOrigin.camera.transform.rotation.w;

        prefab2.transform.rotation = vempp;
        

        

        //qemp.y = m_SessionOrigin.camera.transform.rotation.y;

        //prefab2.transform.rotation=qemp; 
        /* string test4= " Cx: " +m_SessionOrigin.trackablesParent.position.x.ToString();
         string test5= " Cy: " +m_SessionOrigin.trackablesParent.position.y.ToString();
         string test6= " Cz: " +m_SessionOrigin.trackablesParent.position.z.ToString();
         string test7= " CRx: " +m_SessionOrigin.trackablesParent.rotation.x.ToString();
         string test8= " CRy: " +m_SessionOrigin.trackablesParent.rotation.y.ToString();
         string test9= " CRz: " +m_SessionOrigin.trackablesParent.rotation.z.ToString();
         */

        string test10="카메라"+m_SessionOrigin.camera.transform.position.x.ToString()+" y: "+m_SessionOrigin.camera.transform.position.y.ToString()+" z: "+m_SessionOrigin.camera.transform.position.z.ToString();
        string test105="카메라R"+m_SessionOrigin.camera.transform.rotation.x.ToString()+" Ry: "+m_SessionOrigin.camera.transform.rotation.y.ToString()+" Rz: "+m_SessionOrigin.camera.transform.rotation.z.ToString();
        string test106="다리포지션"+prefab2.transform.position.x.ToString()+" Ry: "+prefab1.transform.position.y.ToString()+" Rz: "+prefab1.transform.position.z.ToString();
        string test11="모델R"+prefab2.transform.rotation.x.ToString()+" Ry: "+prefab2.transform.rotation.y.ToString()+" Rz: "+prefab2.transform.rotation.z.ToString();
        string test9=" 평면갯수: "+m_ARPlaneManager.trackables.count.ToString();
        
        if (isFloor == false)
        {
            foreach (ARPlane plane in m_ARPlaneManager.trackables)
            {
                //prefab2.transform.position=new Vector3(prefab2.transform.position.x, plane.center.y, prefab2.transform.position.z);
                if (plane.center.y < -1.3f)
                {
                    //prefab2.transform.position = new Vector3(prefab2.transform.position.x, plane.center.y, prefab2.transform.position.z);
                    test12 = "찾다!! Local Plane x는: " + plane.transform.localPosition.x.ToString() + " y는: " + plane.transform.localPosition.y.ToString() + " z는: " + plane.transform.localPosition.z.ToString();

                    floorID = plane.trackableId;
                    floor = m_ARPlaneManager.GetPlane(floorID).infinitePlane;
                    floorheight = floor.distance;
                    isFloor = true;
                    m_ARPlaneManager.enabled = false;
                     prefab2.transform.localScale = new Vector3(floorheight, floorheight, floorheight);
                    test12 = "야찾다!! Local Plane x는: " + plane.transform.localPosition.x.ToString() + " y는: " + plane.transform.localPosition.y.ToString() + " z는: " + plane.transform.localPosition.z.ToString();
                    
                    break;
                }

                // Do something with the ARPlane
            }
        }
        else
        {
              
           
            eyeheight=floor.GetDistanceToPoint(m_SessionOrigin.camera.transform.position);
            test13 = "눈높이 " + eyeheight.ToString()+" 바닥높이: "+floorheight.ToString();

        }

        if(timer>10)
        {
            Quaternion tmp = new Quaternion();
            tmp = m_SessionOrigin.camera.transform.rotation;
            tmp.x = 0; tmp.z = 0;
            GameObject footprints=Instantiate(trace, new Vector3(m_SessionOrigin.camera.transform.position.x, -1*floorheight-1, m_SessionOrigin.camera.transform.position.z),  tmp);
            footprints.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            footprints.transform.SetParent(m_SessionOrigin.transform);
            footlist.Add(footprints);
            timer = 0;
        }

        Quaternion temp=new Quaternion();
        Vector3 temp2= new Vector3();
        /*temp.x=m_SessionOrigin.camera.transform.rotation.x;
        temp.y=m_SessionOrigin.camera.transform.rotation.y;
        temp.z=m_SessionOrigin.camera.transform.rotation.z;
        
        prefab1.transform.rotation = temp;
        temp2.x=m_SessionOrigin.camera.transform.position.x;
        temp2.y=m_SessionOrigin.camera.transform.position.y;
        temp2.z=m_SessionOrigin.camera.transform.position.z;
        prefab2.transform.position=temp2;*/
         //prefab1.transform.Translate(new Vector3(0,0,0.5f), m_SessionOrigin.camera.transform);
         forTest1.text= /*test4+test5+test6+test7+test8+test9+*/test9+test10+test105+test106+test11+test12+test13;
        // prefab1.transform.rotation=m_SessionOrigin.camera.transform.rotation;
        


  

    }
    private void OnDisable()
    {
        foreach(GameObject i in footlist)
        {
            Destroy(i);
        }
    }
}
