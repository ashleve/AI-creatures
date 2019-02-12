using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour {
    public GameObject mainBody;
    private List<GameObject> joints = new List<GameObject>();
    private List<GameObject> sensors = new List<GameObject>();
    private List<Muscle> muscles = new List<Muscle>();


    public struct spawn
    {
        public int numOfObjects;
        public Vector3[] spawnPositions;
        public Quaternion[] spawnRotations;
    }
    public spawn spawnInfo = new spawn();



    //public Material mat;

    public NeuralNet NN;
    public float[] inputs;  // input data fed to network


    public float fitnessScore;


    public static float MAX_ANGULAR_VELOCITY = 4.0f;
    //float TURN_SPEED = 90.0f;
    //float MAX_SPEED = 30.0f;


    public double time = 0;
    public double time2 = 0;
    public static double timeStep = 0.03f;
    public static double timeStep2 = 0.05f;


    public bool awake = true;
    public bool reachedTheGoal = false;



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
                joints.Add(child);
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

        for(int i = 0; i < muscles.Count; i++)
        {
            muscles[i].setMuscleJoints(joints);
            muscles[i].setMuscleDirections(joints);
        }

        SetMaxAngularVelocity(MAX_ANGULAR_VELOCITY);

        GetSpawnInfo();


        int numberOfInputs = (numOfJoints - 1) + numOfSensors + 2;  // angles between joints + number of sensors + number of timers
        inputs = new float[numberOfInputs];

        int numberOfOutputs = 2 * numOfMuscles + 2;

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

            UpdateOutputs();
            time += timeStep;
            time2 += timeStep2;

        }
    }




    private void FireMuscles()
    {
        for (int i = 0; i < muscles.Count; i++)
        {
            float output = NN.OutputLayer[i].value;
            float output2 = NN.OutputLayer[i + 1].value;

            muscles[i].UseMuscle(output);
            muscles[i].UseMuscle(-output2);

            //UseMuscle(dir1, dir2, output1, i, j);
            //UseMuscle(dir1, dir2, -output2, i, j);
            
        }

        float output3 = NN.OutputLayer[NN.OutputLayer.Count - 1].value;
        sensors[sensors.Count - 1].GetComponent<Rigidbody>().AddRelativeTorque(600f * Vector3.right * output3);
        float output4 = NN.OutputLayer[NN.OutputLayer.Count - 1].value;
        sensors[sensors.Count - 1].GetComponent<Rigidbody>().AddRelativeTorque(600f * Vector3.right * -output4);
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
        for (int i = 0; i < joints.Count; i++)
        {
            joints[i].GetComponent<Rigidbody>().maxAngularVelocity = maxAngVel; //set max angular velocity for all joint objects
            joints[i].GetComponent<Rigidbody>().maxDepenetrationVelocity = 0.2f;
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
        for (i = 0; i < joints.Count - 1; i++)
        {
            float value = Vector3.Angle(joints[i].transform.forward, joints[i + 1].transform.forward);
            inputs[i] = value / 180;    // normalise angle from 0-180 to 0-1
        }

        for (int j = 0; j < sensors.Count; j++, i++)
        {
            if (sensors[j].GetComponent<touchSensor>().isTouched)
                inputs[i] = 1;
            else
                inputs[i] = 0;  // -1
        }

        inputs[i] = Mathf.Sin((float)time);
        inputs[i + 1] = Mathf.Sin((float)time2);
    }



    //private float ValueTresholding(float output, float value)
    //{
    //    if (output > value) output = value;
    //    else if (output < -value) output = -value;

    //    return output;
    //}



    //private void ApplyForces()
    //{
    //    float engineForce = NN.OutputLayer[0].value;
    //    float turning = NN.OutputLayer[1].value;

    //    if (engineForce > 1) engineForce = 1f;
    //    else if (engineForce < -1) engineForce = -1f;

    //    if (turning > 1) turning = 1f;
    //    else if (turning < -1) turning = -1f;


    //    velocity += engineForce * ACCELERATION * Time.deltaTime;
    //    if (velocity > MAX_SPEED) velocity = MAX_SPEED;
    //    else if (velocity < -MAX_SPEED) velocity = -MAX_SPEED;

    //    rotation = transform.rotation;
    //    rotation *= Quaternion.AngleAxis(turning * TURN_SPEED * Time.deltaTime, new Vector3(0, 1, 0));

    //    transform.rotation = rotation;
    //    Vector3 direction = new Vector3(0, 0, 1);
    //    direction = rotation * direction;
    //    transform.position += direction * velocity * Time.deltaTime;
    //}


    public void UpdateOutputs()
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

        time = 0.0f;
        time2 = 0.0f;
        //time3 = 0.0f;

        awake = true;
    }


    //public void SetConstraints()
    //{
    //    for (int i = 0; i < joints.Count; i++)
    //        joints[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    //}


    //public void ReleaseConstraints()
    //{
    //    for (int i = 0; i < joints.Count; i++)
    //        joints[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    //}

    public void ForwardPropagate()
    {
        NN.ForwardPropagate(inputs);
    }

}
