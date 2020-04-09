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
      
    public GameObject human;
    public GameObject trace;
    private GameObject model;
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
     
    private Animator anim;
    private float animspeed;
    public Slider speedslider;
    public Slider alphaslider;
   
    public Button resetbtn;
    private bool walkstate;
     
    private float diff;
    private float dist;
   
    private Vector3 prePo;
    private Vector3 currPo;
    private int[] nocount;
 
    void Awake()
    {
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
        m_ARPlaneManager=GetComponent<ARPlaneManager>();
    }
    void Start()
    {
 
        walkstate = false;
        nocount = new int[3] { 1, 1, 1};
        prePo = Vector3.zero;
        currPo = Vector3.zero;
 
        animspeed = 0.7f;
        diff = 0f;
   
        dist = 0f;
        //테스트원
       
        footlist = new List<GameObject>();
        
        Vector3 position0 = m_SessionOrigin.camera.transform.position;
 
        timer = 0;
 
         
        model = Instantiate(human, Vector3.zero, Quaternion.identity);
       
        model.transform.localScale = new Vector3(1.0f, 1.0f,1.2f);
        model.transform.SetParent(m_SessionOrigin.camera.transform);
        Vector3 v = Vector3.zero; v.y = -0.82f; v.z = 0.9f;
        model.transform.position = v;
     
        model.transform.rotation=Quaternion.Euler(-49f,0,0);
         

        anim = model.GetComponent<Animator>();
        speedslider.onValueChanged.AddListener((float val) => SettingSpeed(val));
        alphaslider.onValueChanged.AddListener((float val) => SettingAlpha(val));
       // guideToggle.onValueChanged.AddListener((bool val) => OnGuide(val));
      
        resetbtn.onClick.AddListener(resetclick);
    }

    // Update is called once per frame
    void Update()
    {
        
        var camera0 = m_SessionOrigin.camera.transform;
        //테스트용 기즈모
        Vector3 direction = camera0.rotation * Vector3.forward;

        //모델 트랜스폼 업데이트
        Vector3 t_vec1=Vector3.zero;
   
        
        if (timer > 1)
        {
            prePo = currPo;
            currPo = m_SessionOrigin.camera.transform.position;
            prePo.y = 0;
            currPo.y = 0;
            diff= Vector3.Distance(prePo, currPo);
            dist +=diff;

            if (diff < 0.1) {
                if (nocount[0] == 1) nocount[0] = 0;
                else if (nocount[1] == 1) nocount[1] = 0;
                else if(nocount[2]==1) nocount[2] = 0;
                else
                {
                    //정지상태
                    anim.SetBool("isWalk", false);
                     
                    walkstate = false;
                }
            }
            else
            {
                for (int i = 0; i < 3; i++) nocount[i] = 1;
                //보행중인 상태
                if (!walkstate)
                {
                    
                    anim.SetBool("isWalk", true);
                    walkstate = true;
                }
               
            }
            timer = 0;
             
        }

        forTest1.text = "카메라 " + m_SessionOrigin.camera.transform.position.x.ToString() + " 초속: " + diff.ToString() + "누적거리 " + dist.ToString();
        //옴 테스트
        
       
        //옴테스트

        //string test9=" 평면갯수: "+m_ARPlaneManager.trackables.count.ToString();

        if (isFloor == false)
        {
            foreach (ARPlane plane in m_ARPlaneManager.trackables)
            {
                //model.transform.position=new Vector3(model.transform.position.x, plane.center.y, model.transform.position.z);
                if (plane.center.y < -1.3f)
                {
                    //model.transform.position = new Vector3(model.transform.position.x, plane.center.y, model.transform.position.z);
 
                    floorID = plane.trackableId;
                    floor = m_ARPlaneManager.GetPlane(floorID).infinitePlane;
                    floorheight = floor.distance;
                    isFloor = true;
                    m_ARPlaneManager.enabled = false;
                    break;
                }

            }
        }
        else
        {//바닥인식후에 
              
           
            eyeheight=floor.GetDistanceToPoint(m_SessionOrigin.camera.transform.position);
            //test13 = "눈높이 " + eyeheight.ToString()+" 바닥높이: "+floorheight.ToString();
           
            //발자국


        }
        timer += Time.deltaTime;

    }
   /* private void createObj()
    {
        var t_model = model.transform;
        Vector3 t_vec1 = t_model.position + t_model.forward * 0.5f + t_model.right * 0.3f;
        objs[0].transform.position = t_vec1;

        t_vec1 = t_model.position + t_model.forward - t_model.right * 0.3f;
        objs[1].transform.position = t_vec1;

        t_vec1 = t_model.position + t_model.forward * 1.5f + t_model.right * 0.3f;
        objs[2].transform.position = t_vec1;

        t_vec1 = t_model.position + t_model.forward * 2.0f - t_model.right * 0.3f;
        objs[3].transform.position = t_vec1;
        circle.transform.position = model.transform.position;
    }*/
    private void SettingSpeed(float value)
    {
        animspeed = speedslider.value;
        anim.SetFloat("Speed", animspeed);
    }

    private void SettingAlpha(float val)
    {
        float value = alphaslider.value;
        var list = GameObject.FindGameObjectsWithTag("body");
        for (int i=0; i< list.Length;i++)
        {
            var mat = list[i].GetComponent<Renderer>().material;
            
            mat.SetColor("_Color",new Color(mat.color.r, mat.color.g, mat.color.b, value));

        }
    }
   
    private void resetclick()
    {
        diff = 0;
        dist = 0;
        
        foreach (GameObject i in footlist)
        {
            Destroy(i);
        }
        m_ARPlaneManager.enabled = true;
        isFloor= false;
        timer = 0;
        
        
        
    }
    private void OnDisable()
    {
        foreach(GameObject i in footlist)
        {
            Destroy(i);
        }
    }
}


/*자기몸위로 + 장애물 테스트 04-08
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
    private GameObject model;
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
    public Scrollbar speedslider;
    public Scrollbar alphaslider;
    public Toggle guideToggle;
    private GameObject guide;
    private Animator anim2;
    public Button resetbtn;

    //옴
    public GameObject obj1;
    private List<GameObject> objs;
    //테스트원
    public GameObject Circle;
    private GameObject circle;
    public Slider  circleBar;
    

    void Awake()
    {
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
        m_ARPlaneManager=GetComponent<ARPlaneManager>();
    }
    void Start()
    {
        forTest1.text = "성공1";
        //테스트원
        circle=Instantiate(Circle, m_SessionOrigin.transform);
        forTest1.text = "성공11";
        footlist = new List<GameObject>();
        objs = new List <GameObject>();
        forTest1.text = "성공13";
        for (int i=0; i< 4; i++)
        {
            objs.Add(Instantiate(obj1, m_SessionOrigin.transform, false));
        }

        Vector3 position0 = m_SessionOrigin.camera.transform.position;

        prefab1 = Instantiate(gizmo, position0, Quaternion.identity);//테스트용 기즈모
        prefab1.transform.SetParent(m_SessionOrigin.transform);
 
        timer = 0;

        model = Instantiate(human, position0, Quaternion.identity);
        model.transform.localScale = new Vector3(1.4f, 1.4f,1.4f);
        model.transform.SetParent(m_SessionOrigin.transform);
        anim = model.GetComponent<Animator>();
        speedslider.onValueChanged.AddListener((float val) => SettingSpeed(val));
        alphaslider.onValueChanged.AddListener((float val) => SettingAlpha(val));
       // guideToggle.onValueChanged.AddListener((bool val) => OnGuide(val));
       circleBar.onValueChanged.AddListener((float val) => TestCircle(val));
        resetbtn.onClick.AddListener(resetclick);
    }

    // Update is called once per frame
    void Update()
    {
        
        var camera0 = m_SessionOrigin.camera.transform;
        //테스트용 기즈모
        Vector3 direction = camera0.rotation * Vector3.forward;
        prefab1.transform.position = camera0.position + direction * 0.4f;

        //모델 트랜스폼 업데이트
        Vector3 t_vec1 = camera0.forward * -1*0.5f;
        t_vec1.y = -2.3f;
        Vector3 t_vec2 = camera0.position;
        t_vec2.y = 0;
        model.transform.position = t_vec2 + t_vec1;
        Quaternion t_qtn = new Quaternion(0, 0, 0, 0);
        t_qtn.y = camera0.rotation.y;
        t_qtn.w = camera0.rotation.w;
        model.transform.rotation = t_qtn;

        //옴 테스트
        var t_model = model.transform;
        if (timer == 1.5)
        {
            
            t_vec1 = t_model.position + t_model.forward * 0.5f + t_model.right * 0.3f;
            objs[0].transform.position = t_vec1;

        }
        else if(timer==3)
        {
            t_vec1 = t_model.position + t_model.forward * 1.0f - t_model.right * 0.3f;
            objs[0].transform.position = t_vec1;
        }
        else if(timer==4.5)
        {
            t_vec1 = t_model.position + t_model.forward * 1.5f + t_model.right * 0.3f;
            objs[0].transform.position = t_vec1;
        }
        else if(timer==6)
        { 
            t_vec1 = t_model.position + t_model.forward * 2.0f - t_model.right * 0.3f;
            objs[0].transform.position = t_vec1;
            timer = 0;
        }
       
        //옴테스트

        //string test9=" 평면갯수: "+m_ARPlaneManager.trackables.count.ToString();

        if (isFloor == false)
        {
            foreach (ARPlane plane in m_ARPlaneManager.trackables)
            {
                //model.transform.position=new Vector3(model.transform.position.x, plane.center.y, model.transform.position.z);
                if (plane.center.y < -1.3f)
                {
                    //model.transform.position = new Vector3(model.transform.position.x, plane.center.y, model.transform.position.z);
                    test12 = "찾다!! Local Plane x는: " + plane.transform.localPosition.x.ToString() + " y는: " + plane.transform.localPosition.y.ToString() + " z는: " + plane.transform.localPosition.z.ToString();

                    floorID = plane.trackableId;
                    floor = m_ARPlaneManager.GetPlane(floorID).infinitePlane;
                    floorheight = floor.distance;
                    isFloor = true;
                    m_ARPlaneManager.enabled = false;
                      
                    test12 = "  Local Plane x는: " + plane.transform.localPosition.x.ToString() + " y는: " + plane.transform.localPosition.y.ToString() + " z는: " + plane.transform.localPosition.z.ToString();
                    
  
                    break;
                }

            }
        }
        else
        {//바닥인식후에 
              
           
            eyeheight=floor.GetDistanceToPoint(m_SessionOrigin.camera.transform.position);
            test13 = "눈높이 " + eyeheight.ToString()+" 바닥높이: "+floorheight.ToString();
           
            //발자국
           


        }
        timer += Time.deltaTime;

    }
    private void createObj()
    {
        var t_model = model.transform;
        Vector3 t_vec1 = t_model.position + t_model.forward * 0.5f + t_model.right * 0.3f;
        objs[0].transform.position = t_vec1;

        t_vec1 = t_model.position + t_model.forward - t_model.right * 0.3f;
        objs[1].transform.position = t_vec1;

        t_vec1 = t_model.position + t_model.forward * 1.5f + t_model.right * 0.3f;
        objs[2].transform.position = t_vec1;

        t_vec1 = t_model.position + t_model.forward * 2.0f - t_model.right * 0.3f;
        objs[3].transform.position = t_vec1;
        circle.transform.position = model.transform.position;
    }
    private void SettingSpeed(float value)
    {
        float val = speedslider.value;
        anim.SetBool("isWalk", true);
        anim.SetFloat("Speed", val);
         
        
    }

    private void SettingAlpha(float val)
    {
        float value = alphaslider.value;
        var list = GameObject.FindGameObjectsWithTag("body");
        for (int i=0; i< list.Length;i++)
        {
            var mat = list[i].GetComponent<Renderer>().material;
            
            mat.SetColor("_Color",new Color(mat.color.r, mat.color.g, mat.color.b, value));

        }
    }
    private void TestCircle(float val)
    {
        float value = circleBar.value;
        var c = circle.transform.localScale;
        c.x = value;
        c.z = value;
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
        //if(prefab1!=null) Destroy(prefab1);
        //if (model != null) Destroy(model);
       // if (guide != null) Destroy(guide);
        
        
    }
    private void OnDisable()
    {
        foreach(GameObject i in footlist)
        {
            Destroy(i);
        }
    }
}

     */
