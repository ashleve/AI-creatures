using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cos : MonoBehaviour {

    // The target marker.
    public Transform target;
    public Transform handle;

    // Angular speed in radians per sec.
    float speed = 5f;

    void Update()
    {
        Vector3 direction = handle.position - transform.position;

        transform.RotateAround(target.position, direction, 10);


        //transform.LookAt(handle);
    }
}
