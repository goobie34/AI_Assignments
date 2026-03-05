using System;

//These are dummy classes to enable saving a SimpleNN to a json file with Unitys built in JsonUtility
[Serializable]
public class LayerData
{
    public int rows; // n_nodes
    public int cols; // n_inputs
    public float[] weightsFlattened;
    public float[] biases;
}

[Serializable]
public class NeuralNetData
{
    public int[] networkShape;
    public LayerData[] layers;
    public int fitness;
}