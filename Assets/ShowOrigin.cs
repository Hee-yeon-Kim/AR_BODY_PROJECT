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
    private bool isFloor = false;
    private Plane floor;
    private float eyeheight;
    private float floorheight;
    private float distance;
    private float timer;
    private float foottimer;
    private List<GameObject> footlist;
    // public Text forTest1;
    // Start is called before the first frame update

    private Animator anim;
    private float animspeed;
    public Slider speedslider;
    public Slider alphaslider;

    public Button resetbtn;

    private bool sitstate = false;

    private float diff;
    private float dist;

    private Vector3 prePo;
    private Vector3 currPo;
    private int[] nocount;


    private Image planecondition;
    private Text heightmonitor;
    private float minheight;
    private float maxheight;
    private Text monitor_dist;
    private Text monitor_diff;

    private float testro;
    private float testpoy;
    private float testpoz;
    //  public Slider testRo;
    //  public Slider testPoy;
    //  public Slider testPoz;


    void Awake()
    {
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
        planecondition = GameObject.FindGameObjectWithTag("done").GetComponent<Image>();
        planecondition.enabled = false;
        heightmonitor = GameObject.FindGameObjectWithTag("heightmonitor").GetComponent<Text>();
        heightmonitor.enabled = false;
        var monitor = GameObject.FindGameObjectsWithTag("monitor");
        monitor_diff = monitor[0].GetComponent<Text>();
        monitor_dist = monitor[1].GetComponent<Text>();
    }
    void Start()
    {
        floorheight = 1.45f;
        var list = GameObject.FindGameObjectsWithTag("body");
        for (int i = 0; i < list.Length; i++)
        {
            var mat = list[i].GetComponent<Renderer>().material;

            mat.SetColor("_Color", new Color(mat.color.r, mat.color.g, mat.color.b, 1.0f));

        }
        testro = -67.1418f;
        testpoy = -0.5975334f;
        testpoz = 1.28f;

        nocount = new int[3] { 1, 1, 1 };
        prePo = Vector3.zero;
        currPo = Vector3.zero;

        animspeed = 1f;
        diff = 0f;

        dist = 0f;
        //테스트원

        footlist = new List<GameObject>();



        timer = 0;
        foottimer = 0;

        model = Instantiate(human, Vector3.zero, Quaternion.identity);

        model.transform.localScale = new Vector3(0.8f, 1.0f, 0.9f);
        model.transform.SetParent(m_SessionOrigin.camera.transform);
        Vector3 v = Vector3.zero; v.y = -0.5975334f; v.z = 1.28f;
        model.transform.position = v;

        model.transform.rotation = Quaternion.Euler(-67.1418f, 0f, 0);


        anim = model.GetComponent<Animator>();
        speedslider.onValueChanged.AddListener((float val) => SettingSpeed(val));
        alphaslider.onValueChanged.AddListener((float val) => SettingAlpha(val));
        // testPoz.onValueChanged.AddListener((float val) => Settingpoz(val));
        // testPoy.onValueChanged.AddListener((float val) => Settingpoy(val));

        // testRo.onValueChanged.AddListener((float val) => Settingro(val));

        //guideToggle.onValueChanged.AddListener((bool val) => OnGuide(val));

        resetbtn.onClick.AddListener(resetclick);
    }
    private void Settingpoz(float val)
    {
        // testpoz = testPoz.value; 
    }
    private void Settingpoy(float val)
    {
        // testpoy = testPoy.value;

    }
    private void Settingro(float val)
    {
        // testro = testRo.value;
    }

    // Update is called once per frame
    void Update()
    {

        var camera0 = m_SessionOrigin.camera.transform;
        float bent = 0;

        float angle = camera0.eulerAngles.x;
        if (angle >= 300) angle = angle - 360;
        if (angle < -60) testpoy = -0.95f;
        else if (angle > 80) testpoy = -0.45f;
        else testpoy = (angle + 30) / 220 - 0.95f;
        //모델 트랜스폼 업데이트
        Vector3 v = Vector3.zero; v.y = testpoy; v.z = testpoz;
        model.transform.localPosition = v;

        if (angle > 20 && angle < 80) testro = (angle - 20) * -2 / 15 - 67f;
        else angle = -67f;

        model.transform.localRotation = Quaternion.Euler(testro, 0f, 0);
        /*
        Vector3 t_vec1 = model.transform.localRotation.eulerAngles;
        
        t_vec1.z = testro;
        
       //model.transform.localEulerAngles = t_vec1;
        t_vec1 = model.transform.localPosition;
        t_vec1.x = testpoy;
       // model.transform.localPosition = t_vec1; 
        t_vec1.x = t_vec1.y = t_vec1.z = testpoz;
       // model.transform.localScale = t_vec1;*/


        //**초속-누적거리-이에 따른 걷기 뛰기 애니메이션 조절 파트**
        if (timer > 1)
        {
            prePo = currPo;
            currPo = m_SessionOrigin.camera.transform.position;
            prePo.y = 0;
            currPo.y = 0;
            diff = Vector3.Distance(prePo, currPo);//초속 ( 1초당 이동한 거리)
                                                   //애니메이터 속도 조절
            animspeed = diff / 0.6f;//1이상시 뛰는 모드로 전환 그 이하는 걷기 모드//보통 0.6m/s일때 애니메이션 기본 속도
            anim.SetFloat("Speed", animspeed);//속도 조절
            dist += diff;

            /* if (diff < 0.1) {//초속 0.1m미만연속 2초 초과시, 정지
                 if (nocount[0] == 1) nocount[0] = 0;
                 else if (nocount[1] == 1) nocount[1] = 0;
                 //else if(nocount[2]==1) nocount[2] = 0;
                 else
                 {
                     //정지상태
                     anim.SetBool("isWalk", false);
                     anim.SetBool("isStop", true);

                     walkstate = false;
                 }
             }
             else
             {
                 for (int i = 0; i < 2; i++) nocount[i] = 1;//조금이라도 움직이면 다시 보행모드
                 //보행중인 상태
                 if (!walkstate)
                 {

                     anim.SetBool("isWalk", true);
                     anim.SetBool("isStop", false);
                     walkstate = true;
                 }

             }*/
            timer = 0;
            monitor_diff.text = "초속: " + string.Format("{0:f2}", diff) + " m/s";
            monitor_dist.text = "누적거리: " + string.Format("{0:f2}", dist) + " m";

        }

        // forTest1.text = "회전 " + testro.ToString() + "크기  " + testpoz.ToString() + " x이동  " + testpoy.ToString()+"  카 "+camera0.rotation.eulerAngles.x.ToString()+ "  "+camera0.rotation.eulerAngles.y.ToString()+"  "+ camera0.rotation.eulerAngles.z.ToString();
        //옴 테스트


        //옴테스트

        //string test9=" 평면갯수: "+m_ARPlaneManager.trackables.count.ToString();

        //**평면인식파트**
        if (isFloor == false)
        {
            foreach (ARPlane plane in m_ARPlaneManager.trackables)
            {
                //model.transform.position=new Vector3(model.transform.position.x, plane.center.y, model.transform.position.z);
                //감지된 평면이 4m^2이상이거나 높이가 1.3m를 넘는다면 바닥으로 인지
                float size = plane.size.x * plane.size.y;
                if (size > 4f || plane.center.y < -1.3f)
                {
                    //model.transform.position = new Vector3(model.transform.position.x, plane.center.y, model.transform.position.z);

                    floorID = plane.trackableId;
                    floor = m_ARPlaneManager.GetPlane(floorID).infinitePlane;
                    floorheight = floor.distance;
                    isFloor = true;
                    planecondition.enabled = true;

                    heightmonitor.enabled = true;

                    foreach (var t_plane in m_ARPlaneManager.trackables)
                    {
                        t_plane.gameObject.SetActive(false);
                    }
                    m_ARPlaneManager.enabled = false;
                    break;
                }

            }
        }
        else
        {//바닥인식후에 


            eyeheight = floor.GetDistanceToPoint(m_SessionOrigin.camera.transform.position);
            //test13 = "눈높이 " + eyeheight.ToString()+" 바닥높이: "+floorheight.ToString();
            if (minheight > eyeheight) minheight = eyeheight;
            if (maxheight < eyeheight) maxheight = eyeheight;
            bent = maxheight - eyeheight;
            heightmonitor.text = "눈높이: " + eyeheight + "\n굽힘정도: " + string.Format("{0:f2}", bent);

            if (bent > 0.5f)
            {
                if (sitstate == false)
                {
                    anim.SetTrigger("goSit");
                    anim.SetBool("isSit", true);//앉은 모드로의 전환
                    sitstate = true;
                }


            }
            else
            {
                if (sitstate == true)
                {
                    anim.SetBool("isSit", false);//tpose로의 전환
                    sitstate = false;
                }

            }
        }

        if (foottimer > 3.5f)
        {
            if (diff > 0.2f)//보행중일때만 가장 최근의 35초 동안의 방향과 위치를 기록
            {
                if (footlist.Count == 10)
                {
                    Destroy(footlist[0]);
                    footlist.RemoveAt(0);
                }
                Quaternion tmp = camera0.rotation;
                tmp.x = 0; tmp.z = 0;
                GameObject footprints;
                footprints = Instantiate(trace, new Vector3(camera0.position.x, floorheight * -1 - 0.6f, camera0.position.z), tmp);
                footprints.transform.localScale = new Vector3(0.035f, 0.035f, 0.035f);
                footprints.transform.SetParent(m_SessionOrigin.transform);
                footlist.Add(footprints);
            }
            foottimer = 0;
        }
        timer += Time.deltaTime;
        foottimer += Time.deltaTime;




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
        for (int i = 0; i < list.Length; i++)
        {
            var mat = list[i].GetComponent<Renderer>().material;

            mat.SetColor("_Color", new Color(mat.color.r, mat.color.g, mat.color.b, value));

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



        timer = 0;

        // walkstate = false;
        sitstate = false;
        maxheight = eyeheight;

        anim.Play("tpose");



    }
    private void OnDisable()
    {
        foreach (GameObject i in footlist)
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
