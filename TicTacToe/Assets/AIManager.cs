using UnityEngine;

enum AgentType
{
    Random,
    RandomNeuralNet,
    NeuralNetFromFile
}
public class AIManager : MonoBehaviour
{
    [SerializeField] AgentType agentType = AgentType.Random;

    [HeaderAttribute("For NeuralNetFromFile:")]
    [SerializeField] string fileName;
    [SerializeField] bool blockIllegalMoves = true;

    [HeaderAttribute("For RandomNeuralNet creation:")]
    [SerializeField] int[] randomNeuralNetShape;
    [SerializeField] float mutationChance;
    [SerializeField] float mutationAmount;

    Agent agent;
    
    private void Awake()
    {
        switch(agentType)
        {
            case AgentType.Random:
                {
                    agent = new RandomAgent();
                    break;
                }
            case AgentType.RandomNeuralNet:
                {
                    agent = new NNAgent(randomNeuralNetShape, mutationChance, mutationAmount, true);
                    break;
                }
            case AgentType.NeuralNetFromFile:
                {
                    if (fileName != null)
                        agent = new NNAgent(new SimpleNN(JSONHandler.Read("/NN Evolution" + fileName)), blockIllegalMoves);

                    break;
                }
        }

        agent.SetGoingFirst(true);
    }

    public int GetMove(int[] board)
    {
        return agent.MakeMove(board);
    }
}
