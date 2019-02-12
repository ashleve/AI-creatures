using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randMove : MonoBehaviour {

    //public GameObject Joint;
    // Use this for initialization
    int i = 0, n = 10;
    void Start () {
        //Time.timeScale = 3;

    }
	
	// Update is called once per frame
	void Update () {
        Vector3 force = new Vector3(Random.Range(-100, 101), Random.Range(100, -101), Random.Range(100, -101));
        GetComponent<Rigidbody>().AddForce(force);
        if(i % n == 0)
        {
            print(GetComponent<Transform>().localRotation);//rotation related to body
            print(GetComponent<Transform>().localPosition);//rotation related to body
            print(GetComponent<Transform>().rotation);//overall rotation
        }
        i++;

    }
}
