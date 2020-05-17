using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightFoot : MonoBehaviour
{
    // Start is called before the first frame update
    public Material black;
    public Material green;
    public Material red;
    public GameObject trace;
    private Button resetbtn;
    private List<GameObject> right_footlist;
    Material material;
    CollideCheck ccheck;
    ShowOrigin main;
    
    private void Start()
    {
        resetbtn = GameObject.FindGameObjectWithTag("reset").GetComponent<Button>();
        resetbtn.onClick.AddListener(resetclick);
        main = GameObject.FindGameObjectWithTag("origin").GetComponent<ShowOrigin>();
        right_footlist = new List<GameObject>();
        ccheck = GameObject.FindGameObjectWithTag("mycollider").GetComponent<CollideCheck>();
    }
    void choose()
    {
        switch (ccheck.state)
        {
            case 0: material = black; main.total++; break;
            case 1:  
            case 4: material = green; main.right_pass++; main.total++; break;
            case 2:
            case 3: 
            case 5: material = red; main.right_fail++; main.total++; break;


        }

    }

    // Update is called once per frame
    public void StepR()
    {
        choose();
        if (right_footlist.Count > 20) right_footlist.RemoveAt(0);
        
        var foot1 = transform;
        Vector3 vec1 = foot1.position;
        vec1 += foot1.transform.right * 0.05f;
        vec1.y = -1.38f;
        Quaternion qua1 = foot1.rotation;
        qua1.x = 0; qua1.z = 0;
        GameObject footprints;
        footprints = Instantiate(trace, vec1, qua1);
        footprints.GetComponent<MeshRenderer>().material = material;
        footprints.transform.localScale = new Vector3(0.01f, 0.01f, 0.02f);
        right_footlist.Add(footprints);

    }

    private void OnDestroy()
    {
        foreach (GameObject i in right_footlist)
        {
            Destroy(i);
        }
    }
    private void resetclick()
    {
        foreach (GameObject g in right_footlist)
            Destroy(g);
    }
}
