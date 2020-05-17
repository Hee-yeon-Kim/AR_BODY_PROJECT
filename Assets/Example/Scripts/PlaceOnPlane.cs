using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Listens for touch events and performs an AR raycast from the screen touch point.
/// AR raycasts will only hit detected trackables like feature points and planes.
///
/// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
/// and moved to the hit position.
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : MonoBehaviour
{
  
    public GameObject straignt_path;
    public GameObject circle_path;
    public Toggle straight_tog;
    public Toggle circle_tog;
    [HideInInspector] public Vector3 ccenter;
    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnedStraight{ get; private set; }
    public GameObject spawnedCircle { get; private set; }
    private bool circleon;
    private bool straighton;
    ShowOrigin Main;
    ARSessionOrigin Origin;
     
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_RaycastManager;
    void Awake()
    {
         
        m_RaycastManager = GetComponent<ARRaycastManager>();
        Main = GetComponent<ShowOrigin>();
        Origin = GetComponent<ARSessionOrigin>();
        straighton = false;
        circleon = false;
    }
    void Start()
    {
        straight_tog.onValueChanged.AddListener((bool val) => straight_listner(val));
        circle_tog.onValueChanged.AddListener((bool val) => circle_listner(val));
    }
    void straight_listner(bool val)
    {
        if (straight_tog.isOn)
        {
            straighton = true;
        }
        else straighton = false;
    }
    void circle_listner(bool val)
    {
        if(circle_tog.isOn)
        {
            circleon = true;
        }
        else circleon = false;
    }
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    void Update()
    {

        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (!IsPointOverUIObject(touchPosition) && m_RaycastManager.Raycast(touchPosition, s_Hits ))
        {
            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
           
            var hitPose = s_Hits[0].pose;
            Vector3 tmp1 = Vector3.zero;
            Vector3 tmp21 = Vector3.zero;
            if (straighton)
            {
                if (spawnedStraight == null&& Main.floorheight!=0)
                {
                    tmp1.x = hitPose.position.x;
                    tmp1.y = -1*Main.floorheight;
                    tmp1.z = hitPose.position.z;
                    tmp21 = Origin.camera.transform.rotation.eulerAngles;
                    tmp21.x = 90.0f;
                    tmp21.z += 90.0f;
                     
                    spawnedStraight = Instantiate(straignt_path, tmp1, Quaternion.Euler(tmp21));
                    spawnedStraight.transform.SetParent( Origin.transform);
                    spawnedStraight.transform.localScale = new Vector3(100f, 100f, 100f);
                }
                else
                {
                    tmp1.x = hitPose.position.x;
                    tmp1.y = -1 * Main.floorheight;
                    tmp1.z = hitPose.position.z;
                    tmp21 = Origin.camera.transform.rotation.eulerAngles;
                    tmp21.x = 90.0f;
                    tmp21.z += 90.0f;

                    spawnedStraight.transform.rotation = Quaternion.Euler(tmp21);
                    spawnedStraight.transform.position = tmp1;
                }
            }
            //¿øÇü
            if (circleon)
            {
                var camera0 = Origin.camera.transform;

                if (spawnedCircle == null&&Main.floorheight != 0)
                {
                    tmp1.x = hitPose.position.x;
                    tmp1.y = -1*Main.floorheight;
                    tmp1.z = hitPose.position.z;
                    tmp1 += camera0.right * 2.3f;
                    tmp1 += camera0.forward * 0.5f;
                    tmp21 = camera0.rotation.eulerAngles;
                    tmp21.x = 90.0f;
                    tmp21.z += 90.0f;
                    
                    spawnedCircle = Instantiate(circle_path,tmp1, Quaternion.Euler( tmp21));
                    spawnedCircle.transform.SetParent(Origin.transform);
                    spawnedCircle.transform.localScale = new Vector3(100f, 100f, 100f);
                 }
                else
                {
                    tmp1.x = hitPose.position.x;
                    tmp1.y = -1 * Main.floorheight;
                    tmp1.z = hitPose.position.z;
                    tmp1 += camera0.right * 2.3f;
                    tmp1 += camera0.forward * 0.5f;

                    tmp21 = camera0.rotation.eulerAngles;
                    tmp21.x = 90.0f;
                    tmp21.z += 90.0f;

                    spawnedCircle.transform.rotation = Quaternion.Euler(tmp21);
                    spawnedCircle.transform.position = tmp1;
                }
            }
        }
    }
    bool IsPointOverUIObject(Vector2 pos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return false;

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(pos.x, pos.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;

    }

}
