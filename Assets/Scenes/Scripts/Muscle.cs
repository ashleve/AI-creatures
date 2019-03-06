using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muscle {
    private GameObject muscle;
    private GameObject limb1;
    private GameObject limb2;

    private static float FORCE = 1000f;
    private float surfaceAreaJ1;
    private float surfaceAreaJ2;
    private float volume1;
    private float volume2;



    public Muscle(GameObject m)    // constructor
    {
        muscle = m;
        limb1 = null;
        limb2 = null;
    }


    public void setLimbs(List<GameObject> limbs)  // finds 2 joints with the smallest distance to muscle
    {
        if(limbs.Count < 2)
        {
            Debug.Log("error: not enough limbs");
            Time.timeScale = 0;
            return;
        }

        Vector3 musclePosition = muscle.transform.position;

        GameObject tmp1 = limbs[0];
        GameObject tmp2 = limbs[1];
        float minDist1 = Vector3.Distance(musclePosition, tmp1.transform.position);
        float minDist2 = Vector3.Distance(musclePosition, tmp2.transform.position);

        for (int i = 2; i < limbs.Count; i++)
        {
            float distance = Vector3.Distance(musclePosition, limbs[i].transform.position);

            if (minDist1 <= minDist2)
            {
                if (distance < minDist1)
                {
                    minDist1 = distance;
                    tmp1 = limbs[i];
                }
            }
            else
            {
                if (distance < minDist2)
                {
                    minDist2 = distance;
                    tmp2 = limbs[i];
                }
            }
        }

        limb1 = tmp1;
        limb2 = tmp2;



        //surfaceAreaJ1 = joint1.GetComponent<Renderer>().bounds.size[0] * joint1.GetComponent<Renderer>().bounds.size[2];
        //surfaceAreaJ2  = joint2.GetComponent<Renderer>().bounds.size[0] * joint2.GetComponent<Renderer>().bounds.size[2];

        //Vector3 size1 = joint1.GetComponent<Renderer>().bounds.size;
        //volume1 = size1.x * size1.y * size1.z;
        //Vector3 size2 = joint2.GetComponent<Renderer>().bounds.size;
        //volume2 = size2.x * size2.y * size2.z;
    }


    public void UseMuscle(float value)
    {
        if (limb1 == null || limb2 == null)
        {
            Debug.Log("error: joints not assigned");
            Time.timeScale = 0;
            return;
        }
        //dir1 = joint1.transform.position - muscle.transform.position;
        //dir2 = joint2.transform.position - muscle.transform.position;

        Vector3 direction1 = limb1.transform.position - limb2.transform.position;
        Vector3 direction2 = limb2.transform.position - limb1.transform.position;

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
        limb1.GetComponent<Rigidbody>().AddForce(direction1 * a1);
        limb2.GetComponent<Rigidbody>().AddForce(direction2 * a2);
    }



}
