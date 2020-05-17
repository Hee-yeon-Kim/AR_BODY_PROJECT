using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideCheck : MonoBehaviour
{
    [HideInInspector] public int state;
    [HideInInspector] public bool enter;

    ShowOrigin main;
    // Start is called before the first frame update
    void Start()
    {
        main = GameObject.FindGameObjectWithTag("origin").GetComponent<ShowOrigin>();
        state = 0;
       
    }

  
    void OnTriggerEnter(Collider other)
    {
        if (enter == false)
        {
            enter = true;//
        }
        if (other.tag == "straight")
        {
            state = 4;// 
        }
        else if (other.tag == "circle")
        {
            state = 1;// 
        }
        else if (other.tag == "incircle")
        {
            state = 2;// 
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "straight")
        {
            state = 5;// 
        }
        else if (other.tag == "circle")
        {
            state = 3;// 
        }
        else if (other.tag == "incircle")
        {
            state = 1;// =
        }
    }
}
