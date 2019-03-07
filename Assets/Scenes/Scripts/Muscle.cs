using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muscle {
    private GameObject muscle;
    public GameObject limb1;
    public GameObject limb2;

    private static float STRENGTH = 100f;
    private static float limbAreaXZ;
    private static float maxStrength;   // proportional to surface area of limbs



    public Muscle(GameObject m)    // constructor
    {
        muscle = m;
        limb1 = null;
        limb2 = null;
    }


    public void setLimbs(List<GameObject> limbs)  
    {
        if(limbs.Count < 2)
        {
            Debug.Log("error: not enough limbs");
            Time.timeScale = 0;
            return;
        }



        /////////////////////////////////////////////////////////////////////////////////////////////////
        // finds 2 limbs with the smallest distance to muscle
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
        /////////////////////////////////////////////////////////////////////////////////////////////////



        float surfaceAreaXZ1 = limb1.GetComponent<Renderer>().bounds.size.x * limb1.GetComponent<Renderer>().bounds.size.z;
        float surfaceAreaXZ2 = limb2.GetComponent<Renderer>().bounds.size.x * limb2.GetComponent<Renderer>().bounds.size.z;
        limbAreaXZ = surfaceAreaXZ1 + surfaceAreaXZ2;
        maxStrength = STRENGTH * limbAreaXZ;
    }


    public void UseMuscle(float value)
    {
        if (limb1 == null || limb2 == null)
        {
            Debug.Log("error: limbs not assigned");
            Time.timeScale = 0;
            return;
        }



        Vector3 direction1 = limb1.transform.position - muscle.transform.position;
        Vector3 direction2 = limb2.transform.position - muscle.transform.position;
        //Vector3 direction1 = limb1.transform.position - limb2.transform.position;
        //Vector3 direction2 = -direction1;


        // value = <0,1>
        float FORCE = maxStrength * value;

        //Debug.Log(FORCE);

        limb1.GetComponent<Rigidbody>().AddForce(direction1 * FORCE);
        limb2.GetComponent<Rigidbody>().AddForce(direction2 * FORCE);

        muscle.GetComponent<Rigidbody>().AddForce(-direction1 * FORCE);
        muscle.GetComponent<Rigidbody>().AddForce(-direction2 * FORCE);
    }



}
