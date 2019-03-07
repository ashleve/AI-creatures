using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron
{
    public static float MAX_WEIGHT = 1.0f;
    public float value;
    public List<float> inputWeights = null;

    public Neuron() // constructor
    {
        inputWeights = new List<float>();
        value = 0;
    }

    public Neuron(float biasValue) // constructor for bias neuron
    {
        value = biasValue;
    }

    public static float GetRandom()
    {
        return Random.Range(-MAX_WEIGHT, MAX_WEIGHT);
    }

    public void SetRandomWeights(List<Neuron> Layer)
    {
        inputWeights.Clear();
        foreach(Neuron neuron in Layer)
        {
            inputWeights.Add(GetRandom());
        }
    }

    public void CalculateValue(List<Neuron> Layer)  // calculates value of neuron using previous layer as input
    {
        if (inputWeights == null) return;   // if it's bias, it doesn't have input weights

        value = 0;
        int i = 0;
        foreach(float weight in inputWeights)
        {
            value += weight * Layer[i].value;
            i++;
        }
    }

}


public class NeuralNet
{
    private List<Neuron> InputLayer;
    public List<List<Neuron>> HiddenLayers;
    public List<Neuron> OutputLayer;
    public static float BIAS_VALUE = 1f;

    public NeuralNet(int inputSize = 2, int outputSize = 2)    // constructor
    {
        InputLayer = new List<Neuron>();
        HiddenLayers = new List<List<Neuron>>();
        OutputLayer = new List<Neuron>();

        for (int i = 0; i < inputSize; i++)
        {
            InputLayer.Add(new Neuron());
        }
        InputLayer.Add(new Neuron(BIAS_VALUE));

        for (int i = 0; i < outputSize; i++)
        {
            OutputLayer.Add(new Neuron());
        }
        SetNewRandomWeightsForOutputLayer(OutputLayer, InputLayer);
    }

    public void AddHiddenLayer(int size = 4)
    {
        HiddenLayers.Add(new List<Neuron>());
        List<Neuron> hiddenLayer = HiddenLayers[HiddenLayers.Count - 1];

        List<Neuron> previousLayer;
        if (HiddenLayers.Count == 1) previousLayer = InputLayer;
        else previousLayer = HiddenLayers[HiddenLayers.Count - 2];

        for (int i = 0; i < size; i++)
        {
            hiddenLayer.Add(new Neuron());
            hiddenLayer[i].SetRandomWeights(previousLayer);
        }
        hiddenLayer.Add(new Neuron(BIAS_VALUE));

        SetNewRandomWeightsForOutputLayer(OutputLayer, hiddenLayer);
    }

    public void ForwardPropagate(float[] inputs)
    {
        for(int i = 0; i < InputLayer.Count - 1; i++) // minus 1 for bias neuron
        {
            InputLayer[i].value = inputs[i];
        }

        if (HiddenLayers.Count != 0)
        {
            CalculateLayer(HiddenLayers[0], InputLayer);
            ApplyActivationFunction(HiddenLayers[0], "relu");

            for (int i = 1; i < HiddenLayers.Count; i++)
            {
                CalculateLayer(HiddenLayers[i], HiddenLayers[i - 1]);
                ApplyActivationFunction(HiddenLayers[i], "relu");
            }

            CalculateLayer(OutputLayer, HiddenLayers[HiddenLayers.Count - 1]);
        }
        else
        {
            CalculateLayer(OutputLayer, InputLayer);
        }

        ApplyActivationFunction(OutputLayer, "tanh");
    }

    public void SetNewRandomWeightsForOutputLayer(List<Neuron> Layer, List<Neuron> previousLayer)
    {
        for (int i = 0; i < Layer.Count; i++)
            Layer[i].SetRandomWeights(previousLayer);
    }

    public void CalculateLayer(List<Neuron> Layer, List<Neuron> previousLayer)
    {
        for (int i = 0; i < Layer.Count; i++)
        {
            Layer[i].CalculateValue(previousLayer);
        }
    }

    public void ApplyActivationFunction(List<Neuron> Layer, string activationFunction)
    {

        switch (activationFunction)
        {
            case "sigmoid":
                Layer.ForEach(neuron => neuron.value = Sigmoid(neuron.value));
                break;
            case "relu":
                Layer.ForEach(neuron => neuron.value = Relu(neuron.value));
                break;
            case "tanh":
                Layer.ForEach(neuron => neuron.value = Tanh(neuron.value));
                break;
            default:
                break;
        }

    }

    public static float Sigmoid(float value)
    {
        value = 1 / (1 + Mathf.Exp(-value));
        return value;
    }

    public static float Tanh(float value)   // sinh(x)/cosh(x)
    {
        value = (Mathf.Exp(value) - Mathf.Exp(-value)) / (Mathf.Exp(value) + Mathf.Exp(-value));
        return value;
    }

    public static float Relu(float value)
    {
        if (value < 0) value = 0;
        return value;
    }

}
