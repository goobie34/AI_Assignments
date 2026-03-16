using UnityEngine;

enum AgentType
{
    RandomBot,
    RandomNeuralNet,
    NeuralNetFromFile,
    NeuralNetFromFile_DropDown
}
public class AIAgentManager : MonoBehaviour
{
    [HeaderAttribute("General:")]
    [SerializeField] AgentType agentType = AgentType.RandomBot;
    [SerializeField] bool blockIllegalMoves = true;

    [HeaderAttribute("For NeuralNetFromFile:")]
    [SerializeField] string fileName;

    [HeaderAttribute("For NeuralNetFromFile_DropDown:")]
    [SerializeField] string[] fileNames;

    [HeaderAttribute("For RandomNeuralNet creation:")]
    [SerializeField] int[] randomNeuralNetShape;
    [SerializeField] float mutationChance;
    [SerializeField] float mutationAmount;

    Agent agent;
    
    private void Awake()
    {
        LoadAgent(0);
    }

    public void LoadAgent(int index = -1)
    {
        switch (agentType)
        {
            case AgentType.RandomBot:
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
            case AgentType.NeuralNetFromFile_DropDown:
                {
                    if (index >= 0 && index < fileNames.Length && fileNames[index] != null)
                        agent = new NNAgent(new SimpleNN(JSONHandler.Read("/NN Evolution" + fileNames[index])), blockIllegalMoves);
                    else
                        Debug.Log("No neural net was loaded from dropdown. Index was outside bounds of array, or filename was null.");

                    break;
                }
        }

        agent.SetFlipBoard(true);
    }

    public int GetMove(int[] board)
    {
        return agent.MakeMove(board);
    }

    public void SetBlockIllegalMoves(bool block)
    {
        blockIllegalMoves = block;
        agent.SetBlockIllegalMoves(block);
    }
}
