using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject joint1;
    public GameObject joint2;
    public GameObject sensor1;
    public GameObject sensor2;

    private float sine;

    public double age = 0.0;

    float joint_frequency;
    float joint_amplitude;
    float joint_phase;

    public Vector3 target_direction;

    // Use this for initialization
    void Start()
    {
        //joint_frequency = Random.Range(3, 20);
        //joint_amplitude = Random.Range(3, 6);
        //joint_phase = Random.Range(0, 360);

        joint1 = transform.GetChild(0).gameObject;
        joint2 = transform.GetChild(1).gameObject;

        sensor2 = transform.GetChild(3).gameObject;
        

    }

    void FixedUpdate()
    {
        sine = 1.0f;

        //Vector3 pos = x1.transform.position;
        //GetComponent<Rigidbody>().AddRelativeTorque(sine * new Vector3(0F, 0F, 1F));
        GetComponent<Rigidbody>().maxAngularVelocity = 100f;

        if (Input.GetMouseButtonDown(0))
        {
            //GetComponent<Rigidbody>().AddForceAtPosition(Vector3.right * 1000, transform.position);
            GetComponent<Rigidbody>().AddRelativeTorque(0, 1, 0);
            //GetComponent<Rigidbody>().AddTorque(Vector3.left * 20000);
        }
        if (Input.GetMouseButtonDown(1))
        {
            //GetComponent<Rigidbody>().AddForceAtPosition(Vector3.right * 1000, transform.position);
            GetComponent<Rigidbody>().AddRelativeTorque(0, 0, 1);
            //GetComponent<Rigidbody>().AddTorque(Vector3.left * 20000);
        }
        if (Input.GetKeyDown("1"))
        {
            //GetComponent<Rigidbody>().AddForceAtPosition(Vector3.right * 1000, transform.position);
            GetComponent<Rigidbody>().AddRelativeTorque(1, 0, 0);
            //GetComponent<Rigidbody>().AddTorque(Vector3.left * 20000);
        }
        //GetComponent<Rigidbody>().AddForce(sine * new Vector3(0F, 5F, 0F));

    }

    float Sine(float freq, float amplitude, float phase_shift)
    {
        return Mathf.Sin((float)age * freq + phase_shift) * amplitude;
    }

    void Update()
    {
        age += Time.deltaTime;
    }

}
