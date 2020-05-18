using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightFoot : MonoBehaviour
{
    // Start is called before the first frame update
    public Material black;
    public Material green;
    public Material red;
    public GameObject trace;
    Material material;
    CollideCheck ccheck;
    ShowOrigin main;
    
    private void Start()
    {
       
        main = GameObject.FindGameObjectWithTag("origin").GetComponent<ShowOrigin>();
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
        if (main.right_footlist.Count > 20) main.right_footlist.RemoveAt(0);
        
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
        main.right_footlist.Add(footprints);

    }

}
