using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muscle {
    private GameObject muscle;
    private GameObject joint1;
    private GameObject joint2;
    private Vector3 dir1;   // vector pointing towards joint1
    private Vector3 dir2;
    private static float FORCE = 600f;


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
        dir1 = joint2.transform.position - muscle.transform.position;
    }


    public void UseMuscle(float value)
    {
        if (joint1 == null || joint2 == null)
        {
            Debug.Log("error: joints not assigned");
            Time.timeScale = 0;
            return;
        }

        joint1.GetComponent<Rigidbody>().AddForce(FORCE * dir1 * value); // AddRelativeForce
        joint2.GetComponent<Rigidbody>().AddForce(FORCE * dir2 * value);

        muscle.GetComponent<Rigidbody>().AddForce(FORCE * -dir1 * value);
        muscle.GetComponent<Rigidbody>().AddForce(FORCE * -dir2 * value);
    }



}
