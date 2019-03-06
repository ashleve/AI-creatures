using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour {
    public GameObject mainBody;
    private List<GameObject> limbs = new List<GameObject>();
    private List<GameObject> sensors = new List<GameObject>();
    private List<Muscle> muscles = new List<Muscle>();

    private float massScalar = 200; // mass of limb = massScalar * limbVolume


    public struct spawn
    {
        public int numOfObjects;
        public Vector3[] spawnPositions;
        public Quaternion[] spawnRotations;
    }
    public spawn spawnInfo = new spawn();
    public bool awake = true;


    //public Material mat;

    public NeuralNet NN;
    public float[] inputs;  // input data fed to network

    public float fitnessScore;


    public static float MAX_ANGULAR_VELOCITY = 5.0f;


    private int numOfOscillators = 1;
    public double time1 = 0;
    public double time2 = 0;
    public double time3 = 0;
    public static double timeStep1 = 0.02f;
    public static double timeStep2 = 0.03f;
    public static double timeStep3 = 0.05f;


    private List<float> l = new List<float>();
    public float[] OutputLayer;



    // Use this for initialization
    void Start()
    {
        int numOfChildren = transform.childCount;

        int numOfJoints = 0;
        int numOfSensors = 0;
        int numOfMuscles = 0;
        for (int i = 0; i < numOfChildren; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.tag == "joint")
            {
                numOfJoints++;
                limbs.Add(child);
                SetMass(child);
            }
            else if (child.tag == "sensor")
            {
                numOfSensors++;
                sensors.Add(child);
            }
            else if (child.tag == "muscle")
            {
                numOfMuscles++;
                muscles.Add(new Muscle(child));
            }
            else if (child.tag == "core")
            {
                mainBody = child;
            }
        }

        for (int i = 0; i < muscles.Count; i++)
        {
            muscles[i].setLimbs(limbs);
        }

        SetMaxAngularVelocity(MAX_ANGULAR_VELOCITY);

        GetSpawnInfo();


        int numberOfInputs = (numOfJoints - 1) + numOfSensors + numOfOscillators;  // angles between joints + number of sensors + number of timers
        inputs = new float[numberOfInputs];

        int numberOfOutputs = 2 * numOfMuscles;

        NN = new NeuralNet(numberOfInputs, numberOfOutputs);
        NN.AddHiddenLayer(6);
        //NN.AddHiddenLayer(4);
    }


    // Unity method for physics updates
    void FixedUpdate()
    {
        if (awake)
        {
            GetInputs();
            ForwardPropagate();
            FireMuscles();

            UpdateOutputDisplay();
            time1 += timeStep1;
            time2 += timeStep2;
            time3 += timeStep3;
        }

    }


    private void SetMass(GameObject limb)
    {
        Vector3 size = limb.GetComponent<Renderer>().bounds.size;
        float limbVolume = size.x * size.y * size.z;
        limb.GetComponent<Rigidbody>().mass = massScalar * limbVolume;
    }


    private void FireMuscles()
    {
        for (int i = 0; i < muscles.Count; i++)
        {
            float output1 = NN.OutputLayer[i].value;
            float output2 = NN.OutputLayer[i + 1].value;

            muscles[i].UseMuscle(output1);
            muscles[i].UseMuscle(-output2);

        }

        //float output3 = NN.OutputLayer[NN.OutputLayer.Count - 1].value;
        //sensors[sensors.Count - 1].GetComponent<Rigidbody>().AddRelativeTorque(200f * Vector3.right * output3);
        //float output4 = NN.OutputLayer[NN.OutputLayer.Count - 1].value;
        //sensors[sensors.Count - 1].GetComponent<Rigidbody>().AddRelativeTorque(200f * Vector3.right * -output4);
    }


    private void GetSpawnInfo()
    {
        int numOfChildren = transform.childCount;
        spawnInfo.numOfObjects = numOfChildren;

        spawnInfo.spawnPositions = new Vector3[numOfChildren];
        spawnInfo.spawnRotations = new Quaternion[numOfChildren];

        for (int i = 0; i < numOfChildren; i++)
            spawnInfo.spawnPositions[i] = transform.GetChild(i).gameObject.transform.position;   //get spawn cords of children relative to parent

        for (int i = 0; i < numOfChildren; i++)
            spawnInfo.spawnRotations[i] = transform.GetChild(i).gameObject.transform.rotation; //get spwan rotations of children relative to parent
    }


    private void SetMaxAngularVelocity(float maxAngVel)
    {
        for (int i = 0; i < limbs.Count; i++)
        {
            limbs[i].GetComponent<Rigidbody>().maxAngularVelocity = maxAngVel; //set max angular velocity for all joint objects
            limbs[i].GetComponent<Rigidbody>().maxDepenetrationVelocity = 0.2f; //set this idk why just in case
        }
        for (int i = 0; i < sensors.Count; i++)
        {
            sensors[i].GetComponent<Rigidbody>().maxAngularVelocity = maxAngVel; //set max angular velocity for all sensor objects
            sensors[i].GetComponent<Rigidbody>().maxDepenetrationVelocity = 0.2f;
        }
    }


    private void GetInputs()
    {
        int i;
        for (i = 0; i < limbs.Count - 1; i++)
        {
            float value = Vector3.Angle(limbs[i].transform.forward, limbs[i + 1].transform.forward);
            inputs[i] = value / 180;    // normalise angle from 0-180 to 0-1
        }

        for (int j = 0; j < sensors.Count; j++, i++)
        {
            if (sensors[j].GetComponent<touchSensor>().isTouched)
                inputs[i] = 1;
            else
                inputs[i] = 0;  // -1
        }

        inputs[i] = Mathf.Sin((float)time1);
        //inputs[i + 1] = Mathf.Sin((float)time2);
        //inputs[i + 2] = Mathf.Sin((float)time3);
    }


    public void UpdateOutputDisplay()
    {
        l.Clear();
        foreach (var Neuron in NN.OutputLayer)
            l.Add(Neuron.value);
        OutputLayer = l.ToArray();
    }


    public void Respawn()
    {
        for (int i = 0; i < spawnInfo.numOfObjects; i++)
        {
            transform.GetChild(i).gameObject.transform.position = spawnInfo.spawnPositions[i];
            transform.GetChild(i).gameObject.transform.rotation = spawnInfo.spawnRotations[i];
        }

        time1 = 0.0f;
        time2 = 0.0f;
        time3 = 0.0f;

        awake = true;
    }


    public void ForwardPropagate()
    {
        NN.ForwardPropagate(inputs);
    }

}
