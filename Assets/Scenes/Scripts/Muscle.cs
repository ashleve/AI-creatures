using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muscle {
    private GameObject muscle;
    private GameObject joint1;
    private GameObject joint2;
    private Vector3 dir1;   // vector pointing towards joint1
    private Vector3 dir2;
    private static float FORCE = 1000f;
    float surfaceAreaJ1;
    float surfaceAreaJ2;
    float volumeJ1;
    float volumeJ2;



    public Muscle(GameObject m)    // constructor
    {
        muscle = m;
        joint1 = null;
        joint2 = null;
    }


    public void setMuscleJoints(List<GameObject> joints)  // finds 2 joints with the smallest distance to muscle
    {
        if(joints.Count < 2)
        {
            Debug.Log("error: not enough joints");
            Time.timeScale = 0;
            return;
        }

        Vector3 musclePosition = muscle.transform.position;

        float minDist1 = Vector3.Distance(musclePosition, joints[0].transform.position);
        float minDist2 = Vector3.Distance(musclePosition, joints[1].transform.position);
        GameObject j1 = joints[0];
        GameObject j2 = joints[1];

        for (int i = 2; i < joints.Count; i++)
        {
            float distance = Vector3.Distance(musclePosition, joints[i].transform.position);

            if (minDist1 <= minDist2)
            {
                if (distance < minDist1)
                {
                    minDist1 = distance;
                    j1 = joints[i];
                }
            }
            else
            {
                if (distance < minDist2)
                {
                    minDist2 = distance;
                    j2 = joints[i];
                }
            }
        }

        joint1 = j1;
        joint2 = j2;



        //surfaceAreaJ1 = joint1.GetComponent<Renderer>().bounds.size[0] * joint1.GetComponent<Renderer>().bounds.size[2];
        //surfaceAreaJ2 = joint2.GetComponent<Renderer>().bounds.size[0] * joint2.GetComponent<Renderer>().bounds.size[2];
        //volumeJ1 = surfaceAreaJ1 * joint1.GetComponent<Renderer>().bounds.size[1];
        //volumeJ2 = surfaceAreaJ2 * joint2.GetComponent<Renderer>().bounds.size[1];
    }

    public void setMuscleDirections(List<GameObject> joints)
    {
        if(joint1 == null || joint2 == null || joints == null)
        {
            Debug.Log("error: joints not assigned");
            Time.timeScale = 0;
            return;
        }

        dir1 = joint1.transform.position - muscle.transform.position;
        dir2 = joint2.transform.position - muscle.transform.position;

    }


    public void UseMuscle(float value)
    {
        if (joint1 == null || joint2 == null)
        {
            Debug.Log("error: joints not assigned");
            Time.timeScale = 0;
            return;
        }
        dir1 = joint1.transform.position - muscle.transform.position;
        dir2 = joint2.transform.position - muscle.transform.position;

        //float velocityJ1 = (joint1.GetComponent<Rigidbody>().velocity[0] + joint1.GetComponent<Rigidbody>().velocity[1] + joint1.GetComponent<Rigidbody>().velocity[2])/3;
        //float velocityJ2 = (joint2.GetComponent<Rigidbody>().velocity[0] + joint2.GetComponent<Rigidbody>().velocity[1] + joint2.GetComponent<Rigidbody>().velocity[2])/3;
        //if (velocityJ1 == 0) velocityJ1 = 1;
        //if (velocityJ2 == 0) velocityJ2= 1;

        float a1 = FORCE * value;
        float a2 = FORCE * value;

        //Debug.Log(a1);
        //Debug.Log(a2);

        //joint1.GetComponent<Rigidbody>().AddForce(FORCE * dir1 * value); // AddRelativeForce
        //joint2.GetComponent<Rigidbody>().AddForce(FORCE * dir2 * value);

        //Debug.Log(FORCE);
        //Debug.Log(velocityJ1);
        //Debug.Log(value);
        //Debug.Log(dir1);
        joint1.GetComponent<Rigidbody>().AddForce(dir1 * a1);
        joint2.GetComponent<Rigidbody>().AddForce(dir2 * a2);

        muscle.GetComponent<Rigidbody>().AddForce(-dir1 * a1);
        muscle.GetComponent<Rigidbody>().AddForce(-dir2 * a2);

        //muscle.GetComponent<Rigidbody>().AddForce(FORCE * -dir1 * value);
        //muscle.GetComponent<Rigidbody>().AddForce(FORCE * -dir2 * value);
    }



}
