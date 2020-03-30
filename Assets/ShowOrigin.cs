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
    private Animator anim;
    public Scrollbar speedbar;
    public Scrollbar alphabar;
    public Toggle guideToggle;
    private GameObject guide;
    private Animator anim2;
    public Button resetbtn;

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
        
        
        timer = 0;

        speedbar.onValueChanged.AddListener((float val) => ScrollBarListner(val));
        alphabar.onValueChanged.AddListener((float val) => SettingAlpha(val));

        guideToggle.onValueChanged.AddListener((bool val) => OnGuide(val));
        resetbtn.onClick.AddListener(resetclick);
        forTest1.text = "시작";
    }

    // Update is called once per frame
    void Update()
    {
        //prefab1.transform.position = m_SessionOrigin.camera.transform.position;
        Vector3 direction = m_SessionOrigin.camera.transform.rotation * Vector3.forward;
        prefab1.transform.position = m_SessionOrigin.camera.transform.position + direction * 0.4f;

        

        

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
       // string test106="다리포지션"+prefab2.transform.position.x.ToString()+" Ry: "+prefab1.transform.position.y.ToString()+" Rz: "+prefab1.transform.position.z.ToString();
       // string test11="모델R"+prefab2.transform.rotation.x.ToString()+" Ry: "+prefab2.transform.rotation.y.ToString()+" Rz: "+prefab2.transform.rotation.z.ToString();
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
                      
                    test12 = "  Local Plane x는: " + plane.transform.localPosition.x.ToString() + " y는: " + plane.transform.localPosition.y.ToString() + " z는: " + plane.transform.localPosition.z.ToString();
                    prefab2 = Instantiate(human, Vector3.zero, Quaternion.identity);
                    prefab2.transform.localScale = new Vector3(floorheight, floorheight, floorheight);
                    prefab2.transform.SetParent(m_SessionOrigin.transform);
                    anim = prefab2.GetComponent<Animator>();
                    break;
                }

                // Do something with the ARPlane
            }
        }
        else
        {
              
           
            eyeheight=floor.GetDistanceToPoint(m_SessionOrigin.camera.transform.position);
            test13 = "눈높이 " + eyeheight.ToString()+" 바닥높이: "+floorheight.ToString();
            timer += Time.deltaTime;

            //모델 포지션
            Vector3 vemp2 = new Vector3();

            Vector3 vemp3 = new Vector3();//xz 설정
            vemp3 = m_SessionOrigin.camera.transform.rotation * Vector3.back * 14f;
            vemp3.y = 0;

            vemp2.x = m_SessionOrigin.camera.transform.position.x + vemp3.x;
            vemp2.z = m_SessionOrigin.camera.transform.position.z + vemp3.z;
            vemp2.y = floorheight * -1 - 0.8f;//키에 맞게 설정


            prefab2.transform.position = vemp2;
            //모델 rotation
            Quaternion vempp = new Quaternion(0, 0, 0, 0);
            vempp.y = m_SessionOrigin.camera.transform.rotation.y;
            vempp.w = m_SessionOrigin.camera.transform.rotation.w;

            prefab2.transform.rotation = vempp;
            //가이드
            if (guide != null)
            {
                Vector3 p = prefab2.transform.rotation * Vector3.forward * 60.5f + prefab2.transform.position;
                p.y = prefab2.transform.position.y;

                guide.transform.position = p;

                guide.transform.rotation = prefab2.transform.rotation;
             
            }

            if (timer > 10)
            {
                Quaternion tmp = new Quaternion();
                tmp = m_SessionOrigin.camera.transform.rotation;
                tmp.x = 0; tmp.z = 0;
                GameObject footprints = Instantiate(trace, new Vector3(m_SessionOrigin.camera.transform.position.x, -1 * floorheight - 1, m_SessionOrigin.camera.transform.position.z), tmp);
                footprints.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
                footprints.transform.SetParent(m_SessionOrigin.transform);
                footlist.Add(footprints);
                timer = 0;
            }

        }

        
 
        /*temp.x=m_SessionOrigin.camera.transform.rotation.x;
        temp.y=m_SessionOrigin.camera.transform.rotation.y;
        temp.z=m_SessionOrigin.camera.transform.rotation.z;
        
        prefab1.transform.rotation = temp;
        temp2.x=m_SessionOrigin.camera.transform.position.x;
        temp2.y=m_SessionOrigin.camera.transform.position.y;
        temp2.z=m_SessionOrigin.camera.transform.position.z;
        prefab2.transform.position=temp2;*/
         //prefab1.transform.Translate(new Vector3(0,0,0.5f), m_SessionOrigin.camera.transform);
         //forTest1.text= /*test4+test5+test6+test7+test8+test9+*/test9+test10+test105+test12+test13;
        // prefab1.transform.rotation=m_SessionOrigin.camera.transform.rotation;
        


  

    }
    private void OnGuide(bool val)
    {
        if( val == false && guide!=null)
        {
            Destroy(guide);
            return;
        }
        if (val == true)
        {
            Vector3 p = prefab2.transform.rotation * Vector3.forward * 60.5f + prefab2.transform.position;
            p.y = prefab2.transform.position.y;

            guide = Instantiate(human, p, prefab2.transform.rotation);

            guide.transform.localScale = prefab2.transform.localScale;
            guide.transform.SetParent(m_SessionOrigin.transform);

        }
        
 
    }
    private void ScrollBarListner(float value)
    {
        float val = speedbar.value;
        anim.SetBool("isWalk", true);
        anim.SetFloat("Speed", val);
        if (guide != null)
        {
            anim2.SetBool("isWalk", true);
            anim2.SetFloat("Speed", val);
        }
        
    }

    private void SettingAlpha(float val)
    {
        float value = alphabar.value;
        var list = GameObject.FindGameObjectsWithTag("body");
        for (int i=0; i< list.Length;i++)
        {
            var mat = list[i].GetComponent<Renderer>().material;
            if (value==1)
            {
                mat.SetFloat("_Mode", 0);
                mat.SetColor("_Color", new Color(mat.color.r, mat.color.g, mat.color.b, value));
                return;
            }
            mat.SetFloat("_Mode", 3);
            
            mat.SetColor("_Color",new Color(mat.color.r, mat.color.g, mat.color.b, value));

        }
    }
    private void resetclick()
    {
        foreach (GameObject i in footlist)
        {
            Destroy(i);
        }
        m_ARPlaneManager.enabled = true;
        isFloor= false;
        timer = 0;
        if(prefab1!=null) Destroy(prefab1);
        if (prefab2 != null) Destroy(prefab2);
        if (guide != null) Destroy(guide);
        
        
    }
    private void OnDisable()
    {
        foreach(GameObject i in footlist)
        {
            Destroy(i);
        }
    }
}
