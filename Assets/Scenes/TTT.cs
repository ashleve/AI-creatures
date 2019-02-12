using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTT : MonoBehaviour {

    public GameObject[] children;

    // Use this for initialization
    void Start () {
        children = new GameObject[transform.childCount];
        for (int i = 0; i < children.Length; i++)
            children[i] = transform.GetChild(i).gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        getInputs();
        children[0].GetComponent<Rigidbody>().AddRelativeTorque(1, 1, 0);
        children[1].GetComponent<Rigidbody>().AddRelativeTorque(1, 1, 0);

        //Ray ray = new Ray(children[0].transform.position, children[0].transform.forward);
        //Debug.DrawRay(children[0].transform.position, children[0].transform.right*100, Color.red);

    }

    void getInputs()
    {
        float x = Vector3.Angle(children[0].transform.right, children[1].transform.right);
        print(x);
    }
}
