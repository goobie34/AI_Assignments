using UnityEngine;

public class SimpleNN
{
    private static System.Random rng = new System.Random(); //used for mutation
    private static int nextID = 0; //only used for logging

    private int[] networkShape; //eg. shape [2, 4, 4, 2] gives these layers: input (2), hidden (4), hidden(4), output (2)
    private Layer[] layers;

    public int ID { get; private set; }
    public int Fitness { get; set; }
    public int[] NetworkShape { get { return networkShape; } }
    public Layer[] Layers { get { return layers; } }

    public SimpleNN(int[] networkShape)
    {
        this.networkShape = networkShape;
        layers = new Layer[networkShape.Length - 1];

        for (int i = 0; i < layers.Length; i++)
            layers[i] = new Layer(networkShape[i], networkShape[i + 1]);

        ID = nextID;
        nextID++;
    }
    public SimpleNN(string json)
    {
        LoadFromJson(json);
        ID = nextID;
        nextID++;
    }

    public class Layer
    {
        //weight between a given node in this layer, and a given input node
        float[,] weightsArray;  //[node_index, input_index] ==> weight between them
        float[] biasArray;      //bias for given node by index
        float[] nodeArray;      //values of all nodes in this layer

        int n_nodes;
        int n_inputs;

        public float[] NodeArray { get { return nodeArray; } }

        public Layer(int n_inputs, int n_nodes)
        {
            this.n_nodes = n_nodes;
            this.n_inputs = n_inputs;

            weightsArray = new float[n_nodes, n_inputs];
            biasArray = new float[n_nodes];
            nodeArray = new float[n_nodes];
        }
        public Layer(Layer source)
        {
            //deep copy constructor
            this.n_nodes = source.n_nodes;
            this.n_inputs = source.n_inputs;

            this.weightsArray = (float[,])source.weightsArray.Clone();
            this.biasArray = (float[])source.biasArray.Clone();
            this.nodeArray = new float[n_nodes];
        }

        public void Forward(float[] inputsArray)
        {
            nodeArray = new float[n_nodes];

            for (int i = 0; i < n_nodes; i++)
            {
                for(int j = 0; j < n_inputs; j++)
                    nodeArray[i] += weightsArray[i, j] * inputsArray[j];

                nodeArray[i] += biasArray[i];
            }
        }

        public void Activation()
        {
            //Tanh
            for (int i = 0; i < n_nodes; i++)
            {
                nodeArray[i] = (float)System.Math.Tanh(NodeArray[i]);
            }

            ////ReLU
            //for(int i = 0; i < n_nodes; ++i)
            //{
            //    if (nodeArray[i] < 0)
            //        nodeArray[i] = 0;
            //}
        }

        public void Mutate(float chance, float amount)
        {
            for (int i = 0; i < n_nodes; i++)
            {
                for(int j = 0; j < n_inputs; j++)
                {
                    if (RandomFloat(0, 1) < chance)
                        weightsArray[i, j] += RandomFloat(-1, 1) * amount;
                }

                if (RandomFloat(0, 1) < chance)
                    biasArray[i] += RandomFloat(-1, 1) * amount;
            }
        }

        //min = inclusive, max = exclusive
        private float RandomFloat(float min, float max)
        {
            return (float)rng.NextDouble() * (max - min) + min;
        }

        //used to save NN to json file
        public LayerData ExportData()
        {
            LayerData data = new LayerData();
            data.rows = n_nodes;
            data.cols = n_inputs;
            data.biases = (float[])biasArray.Clone();

            // Flatten 2D weightsArray to 1D
            data.weightsFlattened = new float[n_nodes * n_inputs];
            for (int i = 0; i < n_nodes; i++)
                for (int j = 0; j < n_inputs; j++)
                    data.weightsFlattened[i * n_inputs + j] = weightsArray[i, j];

            return data;
        }

        //used to load NN from json file
        public void ImportData(LayerData data)
        {
            this.n_nodes = data.rows;
            this.n_inputs = data.cols;
            this.biasArray = (float[])data.biases.Clone();
            this.weightsArray = new float[n_nodes, n_inputs];

            // Unflatten 1D back to 2D
            for (int i = 0; i < n_nodes; i++)
                for (int j = 0; j < n_inputs; j++)
                    weightsArray[i, j] = data.weightsFlattened[i * n_inputs + j];
        }
    }

    public float[] Brain(float[] inputsArray)
    {
        if (layers.Length == 0)
            return null;

        //input layer
        layers[0].Forward(inputsArray);
        layers[0].Activation();

        //hidden layers
        for (int i = 1; i < layers.Length - 1; i++)
        {
            layers[i].Forward(layers[i - 1].NodeArray);
            layers[i].Activation();
        }
        
        //output layer, without activation
        layers[layers.Length - 1].Forward(layers[layers.Length - 2].NodeArray);

        //return values in output layer
        return layers[layers.Length - 1].NodeArray;
    }

    public void Mutate(float chance, float amount)
    {
        foreach(var layer in layers)
            layer.Mutate(chance, amount);
    }
    public SimpleNN Clone() //to make deep copies
    {
        SimpleNN newNN = new SimpleNN(this.networkShape);
        for (int i = 0; i < layers.Length; i++)
        {
            //uses copy constructor for layer to make deep copy
            newNN.layers[i] = new Layer(this.layers[i]);
        }
        return newNN;
    }

    //convert entire network to json string
    public string ToJson()
    {
        NeuralNetData data = new NeuralNetData();
        data.networkShape = this.networkShape;
        data.layers = new LayerData[this.layers.Length];
        data.fitness = Fitness;

        for (int i = 0; i < layers.Length; i++)
            data.layers[i] = layers[i].ExportData();

        return JsonUtility.ToJson(data, true);
    }

    //loads entire network form json string
    public void LoadFromJson(string json)
    {
        NeuralNetData data = JsonUtility.FromJson<NeuralNetData>(json);
        this.networkShape = data.networkShape;
        this.layers = new Layer[data.layers.Length];

        for (int i = 0; i < layers.Length; i++)
        {
            layers[i] = new Layer(data.layers[i].cols, data.layers[i].rows);
            layers[i].ImportData(data.layers[i]);
        }
    }
}
