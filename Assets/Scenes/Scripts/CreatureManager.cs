using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureManager : MonoBehaviour {

    public GameObject Eve = null;      // The creature used to create new creatures
    //public GameObject Target;
    //public CreatureController Champion;

    public static int NUMBER_OF_CREATURES = 48;
    private List<CreatureController> creatures;

    public static float MUTATION_RATE = 0.02f;
    public static int PULL_OF_PARENTS = 6;
    private float fitnessSum;

    public int time = 0;
    public static int TIME_LIMIT = 2000;
    public int generation = 0;

    public bool spawnInOnePlace = false;


    // Use this for initialization
    void Start()
    {
        if(!Eve)
        {
            Debug.Log("error: no initial creature selected");
            this.enabled = false; //turn off script
        }

        creatures = new List<CreatureController>();
        InitialiseCreatures();
    }



    void FixedUpdate()
    {
        if (AllCrashed() || time > TIME_LIMIT)
        {
            NaturalSelection();
            StartCoroutine(Pause());

            time = 0;
        }

        time++;
    }


    IEnumerator Pause()
    {
        print(generation++);
        SleepAll();

        enabled = false;    //turn off update function
        yield return new WaitForSeconds(1f);  //pause
        enabled = true;     //turn on update function

        RespawnGeneration();
    }


    void SleepAll()
    {
        for (int i = 0; i < NUMBER_OF_CREATURES; i++)
            creatures[i].awake = false;
    }


    bool AllCrashed()
    {
        for(int i = 0; i < NUMBER_OF_CREATURES; i++)
        {
            if (creatures[i].awake) return false;
        }
        return true;
    }


    void InitialiseCreatures()
    {
        if (!Eve) return;

        int x = 0, z = 0, add = 15;
        if (spawnInOnePlace) add = 0;

        for (int i = 0; i < NUMBER_OF_CREATURES; i++)
        {
            Vector3 position = Eve.transform.position;
            position[0] += x;
            position[2] += z;
            GameObject creatureCopy = Instantiate(Eve, position, Eve.transform.rotation);

            CreatureController controllerCopy = creatureCopy.GetComponent<CreatureController>();

            string[] name_tmp = { "snake", (i + 1).ToString() };
            name = string.Join("", name_tmp, 0, 2);
            controllerCopy.gameObject.name = name;

            creatures.Add(controllerCopy);
            if (x < add * 7) x += add;
            else
            {
                z += add;
                x = 0;
            }
        }
    }


    void RespawnGeneration()
    {
        for (int i = 0; i < NUMBER_OF_CREATURES; i++)
        {
            creatures[i].Respawn();
        }
    }


    void NaturalSelection()
    {

        CalculateFitness();
        CalculateFitnessSum();

        SortCreaturesByFitnessValue();
        //show();

        for (int i = NUMBER_OF_CREATURES - 1; i > 1 ; i--)
        {
            Crossover(creatures[i]);
            Mutate(creatures[i]);
        }

    }


    void CalculateFitness()
    {
        for (int i = 0; i < NUMBER_OF_CREATURES; i++)
        {
            //float DistanceToTarget = Vector3.Distance(creatures[i].joints[0].transform.position, Target.transform.position);
            float distance = Vector3.Distance(creatures[i].mainBody.transform.position, creatures[i].spawnInfo.spawnPositions[0]);
            //creatures[i].distToTarget = DistanceToTarget;
            //float time = creatures[i].time;

            creatures[i].fitnessScore = distance;

        }
    }


    void CalculateFitnessSum()
    {
        fitnessSum = 0;
        for (int i = 0; i < NUMBER_OF_CREATURES; i++)
        {
            fitnessSum += creatures[i].fitnessScore;
        }
    }


    //void SetChampion()
    //{
    //    float bestScore = creatures[0].fitnessScore;
    //    Champion = creatures[0];

    //    for (int i = 1; i < NUMBER_OF_CREATURES; i++)
    //    {
    //        if (creatures[i].fitnessScore > bestScore)
    //        {
    //            bestScore = creatures[i].fitnessScore;
    //            Champion = creatures[i];
    //        }
    //    }
    //    print(Champion.name);
    //}


    void Crossover(CreatureController c)
    {
        int rand1 = Random.Range(0, PULL_OF_PARENTS);
        int rand2 = Random.Range(0, PULL_OF_PARENTS);
        while (rand2 == rand1)
            rand2 = Random.Range(0, PULL_OF_PARENTS);

        CreatureController parent1 = creatures[rand1];
        CreatureController parent2 = creatures[rand2];

        for (int i = 0; i < c.NN.HiddenLayers.Count; i++)
        {
            List<Neuron> HiddenLayer = c.NN.HiddenLayers[i];
            for(int j = 0; j < HiddenLayer.Count; j++)
            {
                List<float> weights = HiddenLayer[j].inputWeights;
                if (weights == null) continue;  // bias neuron has no weights so let's skip it
                for (int k = 0; k < weights.Count; k++)
                {
                    if (Random.Range(0, 2) == 0)
                        weights[k] = parent1.NN.HiddenLayers[i][j].inputWeights[k];
                    else
                        weights[k] = parent2.NN.HiddenLayers[i][j].inputWeights[k];
                }
            }
        }

        List<Neuron> OutputLayer = c.NN.OutputLayer;
        for (int i = 0; i < OutputLayer.Count; i++)
        {
            List<float> weights = OutputLayer[i].inputWeights;
            if (weights == null) continue;
            for (int j = 0; j < weights.Count; j++)
            {
                if (Random.Range(0, 2) == 0)
                    weights[j] = parent1.NN.OutputLayer[i].inputWeights[j];
                else
                    weights[j] = parent2.NN.OutputLayer[i].inputWeights[j];
            }
        }
    }


    void SortCreaturesByFitnessValue()
    {
        for(int i = 1; i < creatures.Count; i++)
        {
            for(int j = i; j > 0; j--)
            {
                if(creatures[j].fitnessScore > creatures[j-1].fitnessScore)
                {
                    CreatureController tmp = creatures[j];
                    creatures[j] = creatures[j - 1];
                    creatures[j - 1] = tmp;
                }
            }
        }
    }


    void show()
    {
        for (int i = 0; i < creatures.Count; i++)
            print(creatures[i].fitnessScore);
    }


    GameObject SelectParent()
    {
        float rand = Random.Range(0.0f, fitnessSum);
        float runningSum = 0;

        for (int i = 0; i < NUMBER_OF_CREATURES; i++)
        {
            runningSum += creatures[i].fitnessScore;
            if (runningSum >= rand)
            {
                return creatures[i].gameObject;
            }
        }

        return null;    //should never come to this
    }


    void Mutate(CreatureController controller)
    {
        var HiddenLayers = controller.NN.HiddenLayers;
        foreach (var Layer in HiddenLayers)
        {
            foreach(var neuron in Layer)
            {
                if (neuron.inputWeights == null) continue;  // bias neuron has no weights so let's skip it
                for (int i = 0; i < neuron.inputWeights.Count; i++)
                {
                    float rand = Random.Range(0.0f, 1.0f);
                    if (rand < MUTATION_RATE)
                        neuron.inputWeights[i] = Neuron.GetRandom();
                }
            }
        }

        var OutputLayer = controller.NN.OutputLayer;
        foreach (var neuron in OutputLayer)
        {
            if (neuron.inputWeights == null) continue;  // bias neuron has no weights so let's skip it
            for (int i = 0; i < neuron.inputWeights.Count; i++)
            {
                float rand = Random.Range(0.0f, 1.0f);
                if (rand < MUTATION_RATE)
                    neuron.inputWeights[i] = Neuron.GetRandom();
            }
        }
    }




}