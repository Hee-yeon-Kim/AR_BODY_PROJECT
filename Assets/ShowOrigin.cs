using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using System;

public class ShowOrigin : MonoBehaviour
{
    private ARSessionOrigin m_SessionOrigin;
    private ARPlaneManager m_ARPlaneManager;

    public GameObject human;
   

    public Slider alphaslider;
    public Toggle righttog;
    public Button resetbtn;
    public Slider multiply;
    //그래프 UI
    Toggle graphtog;
    [HideInInspector] public float right_pass;
    [HideInInspector] public float right_fail;

    [HideInInspector] public float left_pass;
    [HideInInspector] public float left_fail;
    bool graphshow = false;
    [HideInInspector] public  List<GameObject> left_footlist;
    [HideInInspector] public List<GameObject> right_footlist;


    [HideInInspector] public int total;
    public Image totalbar;
    public Slider leftbar;
    public Slider rightbar;
    public Text timetext;
    public Text distancetext;
    public Text steptext;
    public Text accutext;
    //

    private GameObject model;
    private TrackableId floorID;
    private bool isFloor = false;
    private Plane floor;

    [HideInInspector] public float eyeheight;
    [HideInInspector]public float floorheight;
    [HideInInspector] public float distance;
    private float timer;
 
    public Text forTest1;
    // Start is called before the first frame update

    private Animator anim;
    private float animspeed;
   
    private bool sitstate = false;

   [HideInInspector]public float diff;
    private float dist;

    private Vector3 prePo;
    private Vector3 currPo;


    private Image planecondition;
    private Text heightmonitor;
    private float minheight;
    private float maxheight;
    private Text monitor_dist;
    private Text monitor_diff;
    private float testro;
    private float testpoy;
    private float testpoz;

    private string x;
    public Toggle iktog;
    [HideInInspector] public bool ik = true;
    [HideInInspector] public float multi= 1.5f;
    [HideInInspector] public bool side = true;//오른발(false-default) 왼발(true) 구반 
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
         ik = false;
        side = true;
        multi = 1.5f;

        graphtog = GameObject.FindGameObjectWithTag("graphtog").GetComponent<Toggle>();
        right_fail = 0;
        right_pass = 0;
        left_fail = 0;
        left_pass = 0;
        graphtog.onValueChanged.AddListener((bool val) => settinggraph(val));
    }
    void Start()
    {
        floorheight = 0.0f;
        var list = GameObject.FindGameObjectsWithTag("body");
        for (int i = 0; i < list.Length; i++)
        {
            var mat = list[i].GetComponent<Renderer>().material;

            mat.SetColor("_Color", new Color(mat.color.r, mat.color.g, mat.color.b, 1.0f));

        }
        testro = -67.1418f;
        testpoy = -0.5975334f;
        testpoz = 1.28f;
     
        prePo = Vector3.zero;
        currPo = Vector3.zero;

        animspeed = 1f;
        diff = 0f;

        dist = 0f;
        //테스트원
        right_footlist = new List<GameObject>();
        left_footlist = new List<GameObject>();


        model = Instantiate(human, Vector3.zero, Quaternion.identity);

        model.transform.localScale = new Vector3(0.8f, 1.0f, 0.9f);
        model.transform.SetParent(m_SessionOrigin.camera.transform);
        Vector3 v = Vector3.zero; v.y = -0.5975334f; v.z = 1.28f;
        model.transform.position = v;

        model.transform.rotation = Quaternion.Euler(-67.1418f, 0f, 0);
 
        anim = model.GetComponent<Animator>();
        alphaslider.onValueChanged.AddListener((float val) => SettingAlpha(val));
        multiply.onValueChanged.AddListener((float val) => SettingMulti(val));

        righttog.onValueChanged.AddListener((bool val) => sidechoice(val));
        resetbtn.onClick.AddListener(resetclick);
        iktog.onValueChanged.AddListener((bool val) => setik(val));
        
    }
    public void settinggraph(bool val)
    {
        graphshow = true;
        steptext.text = total.ToString() + " 걸음";
        distancetext.text = string.Format("{0:f2}", dist) + " m";
        float time = Time.realtimeSinceStartup;
        int time2 = (int)(time / 60.0f);
        timetext.text = time2.ToString() + " 분";
        float right = right_pass + left_pass;
        if (total != 0)
        {
            float fill = right / total;
            int fill2 = (int)(fill * 100);
            totalbar.fillAmount = fill;
            accutext.text = fill2.ToString() + "%";
        }
        if (right_fail + right_pass != 0)
        {
            float rightfail = right_fail / (right_fail + right_pass);
            rightbar.value = rightfail;
        }
        if(left_fail + left_pass!=0)
        {
            float leftfail = left_fail / (left_fail + left_pass);
            leftbar.value = leftfail;
        }
        
    }
    void SettingMulti(float val)
    {
        multi = multiply.value;
    }
   
    void setik(bool val)
    {
        if (iktog.isOn)
        {
            anim.SetTrigger("Empty");
            ik = true;
            anim.SetBool("isIK", true);
            
        }
        else
        {
            ik = false;
            anim.SetBool("isIK", false);
        }

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
        

        //**초속-누적거리-이에 따른 걷기 뛰기 애니메이션 조절 파트**
        if (timer > 1)
        {
            prePo = currPo;
            currPo = m_SessionOrigin.camera.transform.position;
            prePo.y = 0;
            currPo.y = 0;
            diff = Vector3.Distance(prePo, currPo);//초속 ( 1초당 이동한 거리)
            if (diff < 0.1f)
            {
                ik = true;//거의 정지-모션캡쳐파트
              
                anim.SetBool("isIK", true);
                anim.SetTrigger("Empty");
            }
            else//보행파트-공간인식이용파트
            {
                if (!iktog.isOn)
                {
                    ik = false;
                }
                anim.SetBool("isIK", false);
                if (diff < 1.5f) animspeed = diff / 0.6f;//1이상시 뛰는 모드로 전환 그 이하는 걷기 모드//보통 0.6m/s일때 애니메이션 기본 속도
                else animspeed = 1.5f / 0.6f;//오류튐방지를 위한 최대속도 지정
                anim.SetFloat("Speed", animspeed);//캐릭터 속도 조절
                dist += diff;//거리 누적
                
            }
          
            timer = 0;
            monitor_diff.text = "초속: " + string.Format("{0:f2}", diff) + " m/s";
            monitor_dist.text = "누적거리: " + string.Format("{0:f2}", dist) + " m";

        }

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
                    floorheight = floor.distance+0.15f;
                    var bound = GameObject.FindGameObjectWithTag("mycollider").GetComponent<Collider>().bounds.center;
                    bound.y = -1*floor.GetDistanceToPoint(m_SessionOrigin.camera.transform.position);
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
            if (minheight > eyeheight) minheight = eyeheight;
            if (maxheight < eyeheight) maxheight = eyeheight;
            bent = maxheight - eyeheight;
            heightmonitor.text = "눈높이: " + eyeheight + "\n굽힘정도: " + string.Format("{0:f2}", bent);

            if (ik==false&&bent > 0.55f)
            {
                if (sitstate == false)
                {
                    anim.SetTrigger("goSit");
                    anim.SetBool("isSit", true);//앉은 모드로의 전환
                    sitstate = true;
                   // ik = true;//블루투스 연결확인필요
                }


            }
            else
            {
                if (ik == false && sitstate == true)
                {
                    anim.SetBool("isSit", false);//tpose로의 전환
                    sitstate = false;
                   
                }

            }
        }
        if(graphshow)
        {
            settinggraph(true);
        }
        timer += Time.deltaTime;
 
    }
   
    public void sidechoice(bool val)
    {
        if (righttog.isOn)
        {
            side = false;//오른발
        }
        else side = true;//왼발
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
 


        timer = 0;

        // walkstate = false;
        sitstate = false;
        maxheight = eyeheight;

        if (ik ==false) anim.Play("tpose");

        foreach (GameObject g in left_footlist)
        {
            Destroy(g);
        }
           
        foreach (GameObject i in right_footlist)
        {
            Destroy(i);
        }
    }
    
   
    private void OnDestroy()
    {
        foreach (GameObject g in left_footlist)
        {
            Destroy(g);
        }
        foreach (GameObject g in right_footlist)
        {
            Destroy(g);
        }

    }
}

 