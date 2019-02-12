using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touchSensor : MonoBehaviour {

    public bool isTouched;


	// Use this for initialization
	void Start () {
        isTouched = false;
    }
	

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "floor")
        {
            //print("touching floor");
            isTouched = true;
        }

    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "floor")
        {
            //print("not touching floor");
            isTouched = false;
        }

    }

}
